using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BaseController
{
    PlayerStat _stat;

    bool _stopSkill = false;

    //GetMask�ڿ� ��� string�� ���°� ���� �ȵ� Define�� �ƿ� ���̾�� ���ڸ� ���������.
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;
        _stat = gameObject.GetOrAddComponent<PlayerStat>();

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

    }

    protected override void UpdateMoving()
    {
        //���Ͱ� �� �����Ÿ����� ������ ����
        if(_lockTarget!=null)
        {
            _destPos = _lockTarget.transform.position; //��ǥ��ġ�� Ÿ���õ� ���� ��ġ
             float distance = (_destPos - transform.position).magnitude; //��ǥ ���ͱ����� �Ÿ�
            if (distance <= 1)  //�����Ÿ��� ������
            {
                State = Define.State.Skill; //��ų������� ���¹ٲ�
                return; //�Ʒ��ڵ尡 �� �����ʰ� return���� ��
            }
        }

        //�̵�����
        Vector3 dir = _destPos - transform.position; //���⺤�� ���ϱ�
        dir.y = 0;//�ٸ� �ݶ��̴�Ÿ�� �ö����ʰ� 0���� ��������
        if (dir.magnitude < 0.1f) // �����ߴٸ� //nma.Move�� ���е��� �׸������ʾƼ�, ������ ������ �γ��������.
            State = Define.State.Idle;
        else
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))  //����= ��������� ��ġ�� �� ���������(���Ѹ��� ������üũ�ϸ� �ȵǴϱ�), ����, ����������(�ھտ��ִٴ°� �˱����� ª��), ���� ���̾�
            {
                if(!Input.GetMouseButton(0)) //���콺�� �����ʾ����� ��� ���� ���� ������� �������ְ�(��ƺ��ó��) ���ǹ� �޾���
                    State = Define.State.Idle; //Idle�� ����� ���ִ� �ִϸ��̼����� �ٲ�
                return; //�տ� ���������� ���������ʰ� �׳ɳ�
            }

            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude); //�̵��Ÿ��� ������ �����ָ� ������ �������� �������� ���ϰ� �ε�ε�Ÿ��� ����.
                                                                                              //clamp(value,min,max) => value�� min���� ������ min���� max���� ũ�� max������ �־��ش�.

            transform.position = transform.position + dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

    }
    protected override void UpdateSkill()
    {
        if (_lockTarget != null) //�� ���ǹ��� �ʼ���. ���� ���Ͱ� poolable���°� �ƴϱ⿡, ���͸� despawn�ϸ� �Ʒ�_lockTarget���� null�� �Ǿ���� ũ��������
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    void OnHitEvent()
    {
                
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            targetStat.OnAttacked(_stat);
        }

        if (_stopSkill)
        {
            State = Define.State.Idle;
        }
        else
        {
            State = Define.State.Skill;
        }
    }


    void OnMouseEvent(Define.MouseEvent evt)
    {
        if (State == Define.State.Die) //��� ������Ʈ�� �ƴ϶� �ݹ������̶� �̷��� ���� �߰� ���ذŶ�µ�?
            return;
        switch(State)
        {
            case Define.State.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Skill:
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
                        State = Define.State.Moving;
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
