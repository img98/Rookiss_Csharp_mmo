using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;

    Vector3 _destPos;

    void Start()
    {
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
    }

    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
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
        if (dir.magnitude < 0.0001f) // �����ߴٸ�
        {
            _state = PlayerState.Idle;
        }
        else
        {
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude); //�̵��Ÿ��� ������ �����ָ� ������ �������� �������� ���ϰ� �ε�ε�Ÿ��� ����.
                                                                                     //clamp(value,min,max) => value�� min���� ������ min���� max���� ũ�� max������ �־��ش�.
            transform.position = transform.position + dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

        //�ִϸ��̼�ó��
        Animator anim = GetComponent<Animator>();
        // ���� ���ӻ��¿� ���� ������ �� anim���� �Ѱ��ִ°��̴�.
        anim.SetFloat("speed", _speed);
    }

    void UpdateIdle()
    {
        //�ִϸ��̼�ó��
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed",0);
    }

    void Update()
    {
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

    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (_state == PlayerState.Die) //��� ������Ʈ�� �ƴ϶� �ݹ������̶� �̷��� ���� �߰� ���ذŶ�µ�?
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100, Color.red, 1.0f); 

        RaycastHit hit; 
        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Wall"))) 
        {
            //Debug.Log($"Raycast Camera @{hit.collider.gameObject.name}");
            _destPos = hit.point;
            _state = PlayerState.Moving;
        }
    }
}
