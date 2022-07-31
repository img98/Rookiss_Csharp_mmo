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
    PlayerState _state = PlayerState.Idle; //초기 상태는 Idle로 설정
    void UpdateDie()
    {
        //죽었으니 별거 없다.
    }
    void UpdateMoving()
    {
        //이동로직
        Vector3 dir = _destPos - transform.position; //방향벡터 구하기
        if (dir.magnitude < 0.0001f) // 도착했다면
        {
            _state = PlayerState.Idle;
        }
        else
        {
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude); //이동거리의 제한을 안해주면 끝날때 도착점에 수렴하지 못하고 부들부들거릴수 있음.
                                                                                     //clamp(value,min,max) => value가 min보다 작으면 min으로 max보다 크면 max값으로 넣어준다.
            transform.position = transform.position + dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

        //애니메이션처리
        Animator anim = GetComponent<Animator>();
        // 현재 게임상태에 대한 정보를 위 anim에게 넘겨주는것이다.
        anim.SetFloat("speed", _speed);
    }

    void UpdateIdle()
    {
        //애니메이션처리
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
        if (_state == PlayerState.Die) //얘는 업데이트가 아니라 콜백형식이라 이렇게 따로 추가 해준거라는데?
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
