using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    PlayerStat _stat;
    Vector3 _destPos;

    Texture2D _attackIcon; //자주쓸 아이콘이라 그냥 한번 갖고와서 저장시키겠다.
    Texture2D _handIcon;

    enum CursorType
    {
        None,
        Attack,
        Hand,

    }
    CursorType _cursorType = CursorType.None;

    void Start()
    {
        _stat = gameObject.GetOrAddComponent<PlayerStat>();

        _attackIcon= Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack"); //자주쓸 아이콘이라 그냥 한번 갖고와서 저장시키겠다.
        _handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

    }

    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
        Skill,
    }
    PlayerState _state = PlayerState.Idle; //초기 상태는 Idle로 설정
    void UpdateDie()
    {
        //죽었으니 별거 없다.
    }
    void UpdateMoving()
    {
        //이동로직
        Vector3 dir = _destPos - transform.position; //방향벡터 구하기
        if (dir.magnitude < 0.1f) // 도착했다면 //nma.Move가 정밀도가 그리높진않아서, 도착의 기준을 널널히해줬다.
            _state = PlayerState.Idle;
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
                    _state = PlayerState.Idle; //Idle로 만들어 서있는 애니메이션으로 바꿈
                return; //앞에 벽이있으니 움직이지않고 그냥끝
            }

            //transform.position = transform.position + dir.normalized * moveDist; //NavMeshAgent를 안쓸때, 하드코딩으로 움직이던(포지션을 바꿔주던) 코드
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

        //애니메이션처리
        Animator anim = GetComponent<Animator>();
        // 현재 게임상태에 대한 정보를 위 anim에게 넘겨주는것이다.
        anim.SetFloat("speed", _stat.MoveSpeed);
    }

    void UpdateIdle()
    {
        //애니메이션처리
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed",0);
    }

    void Update()
    {
        UpdateMouseCursor();

        switch(_state)
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
        }
    }

    void UpdateMouseCursor()
    {
        if (Input.GetMouseButton(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, _mask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Ground)
            {
                if (_cursorType != CursorType.Attack) //커서이미지가 너무 자주바뀌면(프레임마다) 이미지가 깜빡거림. 그래서 상태가 변했을때만, 커서이미지가 변하게 해주는것.
                {
                    Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 5, 0), CursorMode.Auto); //인자 (이미지, 이미지가 시작될좌표, 마우스모드 )
                    _cursorType = CursorType.Attack;
                }
            }
            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 3, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
            }
        }

    }

    //GetMask뒤에 계속 string이 들어가는게 맘에 안들어서 Define에 아예 레이어별로 숫자를 정의해줬다.
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    GameObject _lockTarget;

    void OnMouseEvent(Define.MouseEvent evt)
    {
        if (_state == PlayerState.Die) //얘는 업데이트가 아니라 콜백형식이라 이렇게 따로 추가 해준거라는데?
            return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);//레이캐스트한해서 히트했다는 결과를 저장
        
        switch(evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                    if(raycastHit)
                    {
                        _destPos = hit.point;
                        _state = PlayerState.Moving;
                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _lockTarget = hit.collider.gameObject;
                        else
                            _lockTarget = null;
                    }
                }
                break;
            case Define.MouseEvent.Press: //마우스를 누르고있는 동안
                {
                    //목적지 좌표를 계속 업데이트
                    if (_lockTarget != null)
                        _destPos = _lockTarget.transform.position;
                    else if (raycastHit)
                        _destPos = hit.point;
                }
                break;
            case Define.MouseEvent.PointerUp:
                _lockTarget = null;
                break;
        }

    }
}
