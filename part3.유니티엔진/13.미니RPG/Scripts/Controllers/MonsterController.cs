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

        //TODO :플레이어 몬스터 총괄 매니저생기면 옮길것
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        float distance = (player.transform.position - transform.position).magnitude; //플레이어와 나까지의 거리
        if(distance<=_scanRange) //플레이어가 내 애드범위에 있으면
        {
            _lockTarget = player; //타겟팅잡고

            State = Define.State.Moving; //그쪽으로 이동하겠다.

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
            if (distance <= _attackRange)  //사정거리에 들어오면
            {
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position); //사정거리안에 들어와 공격중이면 더이상 앞으로 더가지않게 (이걸로 밀치기 해결)

                State = Define.State.Skill; //스킬사용으로 상태바꿈
                return; //아래코드가 더 돌지않게 return으로 끝
            }
        }

        //이동로직
        Vector3 dir = _destPos - transform.position; //방향벡터 구하기
        if (dir.magnitude < 0.1f) // 도착했다면 //nma.Move가 정밀도가 그리높진않아서, 도착의 기준을 널널히해줬다.
            State = Define.State.Idle;
        else
        {
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();

            nma.SetDestination(_destPos); //목적지까지 찾아가는 길찾기 라이브러리이다.
            nma.speed = _stat.MoveSpeed;

            //transform.position = transform.position + dir.normalized * moveDist; //NavMeshAgent를 안쓸때, 하드코딩으로 움직이던(포지션을 바꿔주던) 코드
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
