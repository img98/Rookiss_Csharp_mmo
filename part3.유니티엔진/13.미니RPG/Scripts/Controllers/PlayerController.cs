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
                    anim.CrossFade("ATTACK", 0.1f, -1, 0); //����= Ʋ���� ���, transition�Ѿ�µ� �ɸ��ð�, ���̾�(�Ű�Ⱦ��Ŵ� -1), normalizedTimeOffset(0�����ϸ� ����� ������ ó������ ���ư��� �ٽ�ư�ٴµ�?)
                    break;
            }
        }
    }

    [SerializeField]
    PlayerState _state = PlayerState.Idle; //�ʱ� ���´� Idle�� ���� //_state�� ���� �Ⱦ��µ� �̰� �ʿ��Ѱ�?=>�ʿ��ϴ�. State�� �ᱹ�� _state�� ���¸� �ٲ��ִ� �뵵�̱⿡.

    //GetMask�ڿ� ��� string�� ���°� ���� �ȵ� Define�� �ƿ� ���̾�� ���ڸ� ���������.
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
        //�׾����� ���� ����.
    }
    void UpdateMoving()
    {
        //���Ͱ� �� �����Ÿ����� ������ ����
        if(_lockTarget!=null)
        {
            _destPos = _lockTarget.transform.position; //��ǥ��ġ�� Ÿ���õ� ���� ��ġ
             float distance = (_destPos - transform.position).magnitude; //��ǥ ���ͱ����� �Ÿ�
            if (distance <= 1)  //�����Ÿ��� ������
            {
                State = PlayerState.Skill; //��ų������� ���¹ٲ�
                return; //�Ʒ��ڵ尡 �� �����ʰ� return���� ��
            }
        }

        //�̵�����
        Vector3 dir = _destPos - transform.position; //���⺤�� ���ϱ�
        if (dir.magnitude < 0.1f) // �����ߴٸ� //nma.Move�� ���е��� �׸������ʾƼ�, ������ ������ �γ��������.
            State = PlayerState.Idle;
        else
        {
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            
            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude); //�̵��Ÿ��� ������ �����ָ� ������ �������� �������� ���ϰ� �ε�ε�Ÿ��� ����.
                                                                                     //clamp(value,min,max) => value�� min���� ������ min���� max���� ũ�� max������ �־��ش�.
            nma.Move(dir.normalized * moveDist); //���� = ���������� ���⺤��

            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))  //����= ��������� ��ġ�� �� ���������(���Ѹ��� ������üũ�ϸ� �ȵǴϱ�), ����, ����������(�ھտ��ִٴ°� �˱����� ª��), ���� ���̾�
            {
                if(!Input.GetMouseButton(0)) //���콺�� �����ʾ����� ��� ���� ���� ������� �������ְ�(��ƺ��ó��) ���ǹ� �޾���
                    State = PlayerState.Idle; //Idle�� ����� ���ִ� �ִϸ��̼����� �ٲ�
                return; //�տ� ���������� ���������ʰ� �׳ɳ�
            }

            //transform.position = transform.position + dir.normalized * moveDist; //NavMeshAgent�� �Ⱦ���, �ϵ��ڵ����� �����̴�(�������� �ٲ��ִ�) �ڵ�
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
            PlayerStat myStat = gameObject.GetComponent<PlayerStat>(); //������ �÷��̾���Ʈ�ѷ��� �ް������ϱ� playerStat�� �ᵵ �ȴ�.(����� playerStat�� ���� ����ִ°� ���ٴ°� ����)

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
        if (State == PlayerState.Die) //��� ������Ʈ�� �ƴ϶� �ݹ������̶� �̷��� ���� �߰� ���ذŶ�µ�?
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
                    if (evt == Define.MouseEvent.PointerUp) //�� �������� �ѹ��̶� ������ �����ϰڴ�.
                        _stopSkill = true;
                }
                break;
        }


    }

    void OnMouseEvent_IdleRun(Define.MouseEvent evt) //�ڴ������߿��� OnMouseEvent�� ��� ���ǵ�, ���� �Ʒ� �ڵ尡 �� ���̸�, �ൿ���� ������ ���״� ����, �޸��⸸ �����ϴ� �༮�������� ���� ���´�.
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);//����ĳ��Ʈ���ؼ� ��Ʈ�ߴٴ� ����� ����

        switch (evt)
        {
            case Define.MouseEvent.PointerDown: //���콺�� ��������
                {
                    if (raycastHit)
                    {   //Moving�� ����
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
            case Define.MouseEvent.Press: //���콺�� �������ִ� ����
                {                    
                    if (_lockTarget == null && raycastHit) 
                        _destPos = hit.point; //������ ��ǥ�� ��� ������Ʈ
                }
                break;
            case Define.MouseEvent.PointerUp:
                _stopSkill = true;
                break;
        }

    }
}
