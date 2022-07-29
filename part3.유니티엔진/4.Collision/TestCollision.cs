using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    //1) 나한테 rigidbody 있어야 한다. (isKinematic : Off)
    //2) 나한테 Collider가 있어야 한다. (isTrigger : Off)
    //3) 상대한테 Collider가 있어야 한다. (isTrigger : Off)
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision : {collision.gameObject.name}");
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger : {other.gameObject.name}");
    }

    void Start()
    {

    }

    void Update()
    {
        /*
        if (Input.GetMouseButton(0)) // 보기좋게, 마우스 누를때만 레이캐스트 작동
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Vector3 dir = mousePos - Camera.main.transform.position; // 방향벡터 = 목적지 - 시작점 = 가두리윗평면의 마우스위치 - 카메라의 position좌표
            dir = dir.normalized; // dir 크기를 없앤다.

            Debug.DrawRay(Camera.main.transform.position, dir*100, Color.red, 1.0f); //레이캐스트 레이저 나가는 방향 표시(1.0f초동안)
            RaycastHit hit; //raycast에서 나올 아웃풋을 담을 변수
            if (Physics.Raycast(Camera.main.transform.position, dir, out hit)) // 1.카메라에서 2. dir방향으로 빛을 쏴서, 3.맞는게 있다면(==true생략한거)
            {
                Debug.Log($"Raycast Camera @{hit.collider.gameObject.name}");
            }
        }
        */

        //사실 위 코드는, 원리 그대로 풀어서 만든 코드고, 더욱 간단하게 사용하는 방법이 있다. Ray를 사용하면된다.
        if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 이게 dir=dir.normalized 까지 한방에 해버린것
           
            Debug.DrawRay(Camera.main.transform.position, ray.direction * 100, Color.red, 1.0f); //레이캐스트 레이저 나가는 방향 표시(1.0f초동안)
            
            RaycastHit hit; //raycast에서 나올 아웃풋을 담을 변수
            if (Physics.Raycast(ray,out hit)) // 1.카메라에서 2. dir방향으로 빛을 쏴서, 3.맞는게 있다면(==true생략한거)
            {
                Debug.Log($"Raycast Camera @{hit.collider.gameObject.name}");
            }
        }
        /* 특정 레이어만 쓰고싶다면 다음을 추가수정하면됨
        int mask = (1 << 8); //비트연산자를 통해 8번을 키겠다. = 8번레이어만 쓰겠다.
                             // 여기서 | ^을 통해 여러개 마스크 사용가능
                             // ex) 8번이나 9번을 키겠다. int mask = (1<<8) | (1<<9); 
        if (Physics.Raycast(ray, out hit, mask))
            Debug.Log($"Raycast Camera @{hit.collider.gameObject.name}");
        
         위가 어려우면 다음방법도 있다.
        LayerMask mask = LayerMask.GetMask("Monster") | LayerMask.GetMask("Wall");
         */

        

    }
}
