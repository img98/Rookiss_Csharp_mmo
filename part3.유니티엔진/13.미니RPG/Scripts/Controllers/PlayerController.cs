using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    PlayerStat _stat;
    Vector3 _destPos;

    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
        Skill,
    }

    public PlayerState State
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();
            switch(_state)
            {
                case PlayerState.Die:
                    break;
                case PlayerState.Idle:
                    anim.CrossFade("WAIT", 0.1f);
                    break;
                case PlayerState.Moving:
                    anim.CrossFade("RUN", 0.1f);
                    break;
                case PlayerState.Skill:
                    anim.CrossFade("ATTACK", 0.1f, -1, 0); //인자= 틀어줄 모션, transition넘어가는데 걸릴시간, 레이어(신경안쓸거니 -1), normalizedTimeOffset(0으로하면 모션이 끝나도 처음으로 돌아가서 다시튼다는데?)
                    break;
            }
        }
    }

    [SerializeField]
    PlayerState _state = PlayerState.Idle; //초기 상태는 Idle로 설정 //_state를 이제 안쓰는데 이거 필요한가?=>필요하다. State가 결국은 _state의 상태를 바꿔주는 용도이기에.

    //GetMask뒤에 계속 string이 들어가는게 맘에 안들어서 Define에 아예 레이어별로 숫자를 정의해줬다.
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    void Start()
    {
        _stat = gameObject.GetOrAddComponent<PlayerStat>();


        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

    }

    void UpdateDie()
    {
        //죽었으니 별거 없다.
    }
    void UpdateMoving()
    {
        //몬스터가 내 사정거리보다 가까우면 공격
        if(_lockTarget!=null)
        {
            _destPos = _lockTarget.transform.position; //목표위치는 타겟팅된 몬스터 위치
             float distance = (_destPos - transform.position).magnitude; //목표 몬스터까지의 거리
            if (distance <= 1)  //사정거리에 들어오면
            {
                State = PlayerState.Skill; //스킬사용으로 상태바꿈
                return; //아래코드가 더 돌지않게 return으로 끝
            }
        }

        //이동로직
        Vector3 dir = _destPos - transform.position; //방향벡터 구하기
        if (dir.magnitude < 0.1f) // 도착했다면 //nma.Move가 정밀도가 그리높진않아서, 도착의 기준을 널널히해줬다.
            State = PlayerState.Idle;
        else
        {
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            
            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude); //이동거리의 제한을 안해주면 끝날때 도착점에 수렴하지 못하고 부들부들거릴수 있음.
                                                                                     //clamp(value,min,max) => value가 min보다 작으면 min으로 max보다 크면 max값으로 넣어준다.
            nma.Move(dir.normalized * moveDist); //인자 = 목적지로의 방향벡터

            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))  //인자= 레이저쏘는 위치가 한 배꼽쯤으로(돌뿌리도 못간다체크하면 안되니까), 방향, 레이저길이(코앞에있다는걸 알기위해 짧게), 막힐 레이어
            {
                if(!Input.GetMouseButton(0)) //마우스를 떼지않았으면 계속 벽에 비비는 모션으로 남을수있게(디아블로처럼) 조건문 달아줌
                    State = PlayerState.Idle; //Idle로 만들어 서있는 애니메이션으로 바꿈
                return; //앞에 벽이있으니 움직이지않고 그냥끝
            }

            //transform.position = transform.position + dir.normalized * moveDist; //NavMeshAgent를 안쓸때, 하드코딩으로 움직이던(포지션을 바꿔주던) 코드
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

    }

    void UpdateIdle()
    {
    }
    void UpdateSkill()
    {
        Vector3 dir = _lockTarget.transform.position - transform.position;
        Quaternion quat = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
    }

    void OnHitEvent()
    {

        if(_lockTarget !=null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>(); 
            PlayerStat myStat = gameObject.GetComponent<PlayerStat>(); //지금은 플레이어컨트롤러에 달고있으니까 playerStat을 써도 된다.(참고로 playerStat에 좀더 들어있는게 많다는걸 유의)

            int damage = Mathf.Max(0, myStat.Attack - targetStat.Defense);
            Debug.Log(damage);
            targetStat.Hp -= damage;
        }
        
        if(_stopSkill)
        { 
            State = PlayerState.Idle;
        }
        else
        {
            State = PlayerState.Skill;
        }

    }

    void Update()
    {

        switch(State)
        {
            case PlayerState.Die:
                UpdateDie();
                break;
            case PlayerState.Moving:
                UpdateMoving();
                break;
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Skill:
                UpdateSkill();
                break;
        }
    }



    GameObject _lockTarget;
    bool _stopSkill = false;
    void OnMouseEvent(Define.MouseEvent evt)
    {
        if (State == PlayerState.Die) //얘는 업데이트가 아니라 콜백형식이라 이렇게 따로 추가 해준거라는데?
            return;
        switch(State)
        {
            case PlayerState.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case PlayerState.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case PlayerState.Skill:
                {
                    if (evt == Define.MouseEvent.PointerUp) //꾹 누르던게 한번이라도 떼지면 정지하겠다.
                        _stopSkill = true;
                }
                break;
        }


    }

    void OnMouseEvent_IdleRun(Define.MouseEvent evt) //꾹누르는중에도 OnMouseEvent는 계속 들어갈건데, 만약 아래 코드가 좀 꼬이면, 행동도중 에러가 날테니 서기, 달리기만 관리하는 녀석버전으로 따로 빼냈다.
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);//레이캐스트한해서 히트했다는 결과를 저장

        switch (evt)
        {
            case Define.MouseEvent.PointerDown: //마우스를 누른순간
                {
                    if (raycastHit)
                    {   //Moving을 시작
                        _destPos = hit.point;
                        State = PlayerState.Moving;
                        _stopSkill = false;
                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _lockTarget = hit.collider.gameObject;
                        else
                            _lockTarget = null;
                    }
                }
                break;
            case Define.MouseEvent.Press: //마우스를 누르고있는 동안
                {                    
                    if (_lockTarget == null && raycastHit) 
                        _destPos = hit.point; //목적지 좌표를 계속 업데이트
                }
                break;
            case Define.MouseEvent.PointerUp:
                _stopSkill = true;
                break;
        }

    }
}
