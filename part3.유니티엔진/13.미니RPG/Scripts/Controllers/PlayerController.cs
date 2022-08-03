using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    PlayerStat _stat;
    Vector3 _destPos;

    Texture2D _attackIcon; //���־� �������̶� �׳� �ѹ� ����ͼ� �����Ű�ڴ�.
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

        _attackIcon= Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack"); //���־� �������̶� �׳� �ѹ� ����ͼ� �����Ű�ڴ�.
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
    PlayerState _state = PlayerState.Idle; //�ʱ� ���´� Idle�� ����
    void UpdateDie()
    {
        //�׾����� ���� ����.
    }
    void UpdateMoving()
    {
        //�̵�����
        Vector3 dir = _destPos - transform.position; //���⺤�� ���ϱ�
        if (dir.magnitude < 0.1f) // �����ߴٸ� //nma.Move�� ���е��� �׸������ʾƼ�, ������ ������ �γ��������.
            _state = PlayerState.Idle;
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
                    _state = PlayerState.Idle; //Idle�� ����� ���ִ� �ִϸ��̼����� �ٲ�
                return; //�տ� ���������� ���������ʰ� �׳ɳ�
            }

            //transform.position = transform.position + dir.normalized * moveDist; //NavMeshAgent�� �Ⱦ���, �ϵ��ڵ����� �����̴�(�������� �ٲ��ִ�) �ڵ�
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

        //�ִϸ��̼�ó��
        Animator anim = GetComponent<Animator>();
        // ���� ���ӻ��¿� ���� ������ �� anim���� �Ѱ��ִ°��̴�.
        anim.SetFloat("speed", _stat.MoveSpeed);
    }

    void UpdateIdle()
    {
        //�ִϸ��̼�ó��
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
                if (_cursorType != CursorType.Attack) //Ŀ���̹����� �ʹ� ���ֹٲ��(�����Ӹ���) �̹����� �����Ÿ�. �׷��� ���°� ����������, Ŀ���̹����� ���ϰ� ���ִ°�.
                {
                    Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 5, 0), CursorMode.Auto); //���� (�̹���, �̹����� ���۵���ǥ, ���콺��� )
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

    //GetMask�ڿ� ��� string�� ���°� ���� �ȵ� Define�� �ƿ� ���̾�� ���ڸ� ���������.
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    GameObject _lockTarget;

    void OnMouseEvent(Define.MouseEvent evt)
    {
        if (_state == PlayerState.Die) //��� ������Ʈ�� �ƴ϶� �ݹ������̶� �̷��� ���� �߰� ���ذŶ�µ�?
            return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);//����ĳ��Ʈ���ؼ� ��Ʈ�ߴٴ� ����� ����
        
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
            case Define.MouseEvent.Press: //���콺�� �������ִ� ����
                {
                    //������ ��ǥ�� ��� ������Ʈ
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
