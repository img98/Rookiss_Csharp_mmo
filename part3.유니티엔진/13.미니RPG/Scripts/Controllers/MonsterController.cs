using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    Stat _stat;

    [SerializeField]
    float _scanRange = 10;

    [SerializeField]
    float _attackRange = 2;

    public override void Init()
    {
        _stat = gameObject.GetOrAddComponent<PlayerStat>();

        if (gameObject.GetComponentInChildren<UI_HPBar>() == null) 
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
    }

    protected override void UpdateIdle()
    {
        Debug.Log("monster Update idle");

        //TODO :�÷��̾� ���� �Ѱ� �Ŵ�������� �ű��
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        float distance = (player.transform.position - transform.position).magnitude; //�÷��̾�� �������� �Ÿ�
        if(distance<=_scanRange) //�÷��̾ �� �ֵ������ ������
        {
            _lockTarget = player; //Ÿ�������

            State = Define.State.Moving; //�������� �̵��ϰڴ�.

            return;
        }

    }

    protected override void UpdateMoving()
    {
        Debug.Log("monster Update Moving");

        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= _attackRange)  //�����Ÿ��� ������
            {
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position); //�����Ÿ��ȿ� ���� �������̸� ���̻� ������ �������ʰ� (�̰ɷ� ��ġ�� �ذ�)

                State = Define.State.Skill; //��ų������� ���¹ٲ�
                return; //�Ʒ��ڵ尡 �� �����ʰ� return���� ��
            }
        }

        //�̵�����
        Vector3 dir = _destPos - transform.position; //���⺤�� ���ϱ�
        if (dir.magnitude < 0.1f) // �����ߴٸ� //nma.Move�� ���е��� �׸������ʾƼ�, ������ ������ �γ��������.
            State = Define.State.Idle;
        else
        {
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();

            nma.SetDestination(_destPos); //���������� ã�ư��� ��ã�� ���̺귯���̴�.
            nma.speed = _stat.MoveSpeed;

            //transform.position = transform.position + dir.normalized * moveDist; //NavMeshAgent�� �Ⱦ���, �ϵ��ڵ����� �����̴�(�������� �ٲ��ִ�) �ڵ�
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }

    protected override void UpdateSkill()
    {
        Vector3 dir = _lockTarget.transform.position - transform.position;
        Quaternion quat = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
    }

    void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            Stat myStat = gameObject.GetComponent<Stat>();

            int damage = Mathf.Max(0, myStat.Attack - targetStat.Defense);
            targetStat.Hp -= damage;

            if(targetStat.Hp>0)
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _attackRange)
                    State = Define.State.Skill;
                else
                    State = Define.State.Moving;
            }
            else
            {
                State = Define.State.Idle;
            }

        }
        else
        {
            State = Define.State.Idle;
        }

    }
}
