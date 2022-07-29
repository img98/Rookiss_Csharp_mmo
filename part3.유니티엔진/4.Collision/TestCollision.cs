using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    //1) ������ rigidbody �־�� �Ѵ�. (isKinematic : Off)
    //2) ������ Collider�� �־�� �Ѵ�. (isTrigger : Off)
    //3) ������� Collider�� �־�� �Ѵ�. (isTrigger : Off)
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
        if (Input.GetMouseButton(0)) // ��������, ���콺 �������� ����ĳ��Ʈ �۵�
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Vector3 dir = mousePos - Camera.main.transform.position; // ���⺤�� = ������ - ������ = ���θ�������� ���콺��ġ - ī�޶��� position��ǥ
            dir = dir.normalized; // dir ũ�⸦ ���ش�.

            Debug.DrawRay(Camera.main.transform.position, dir*100, Color.red, 1.0f); //����ĳ��Ʈ ������ ������ ���� ǥ��(1.0f�ʵ���)
            RaycastHit hit; //raycast���� ���� �ƿ�ǲ�� ���� ����
            if (Physics.Raycast(Camera.main.transform.position, dir, out hit)) // 1.ī�޶󿡼� 2. dir�������� ���� ����, 3.�´°� �ִٸ�(==true�����Ѱ�)
            {
                Debug.Log($"Raycast Camera @{hit.collider.gameObject.name}");
            }
        }
        */

        //��� �� �ڵ��, ���� �״�� Ǯ� ���� �ڵ��, ���� �����ϰ� ����ϴ� ����� �ִ�. Ray�� ����ϸ�ȴ�.
        if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // �̰� dir=dir.normalized ���� �ѹ濡 �ع�����
           
            Debug.DrawRay(Camera.main.transform.position, ray.direction * 100, Color.red, 1.0f); //����ĳ��Ʈ ������ ������ ���� ǥ��(1.0f�ʵ���)
            
            RaycastHit hit; //raycast���� ���� �ƿ�ǲ�� ���� ����
            if (Physics.Raycast(ray,out hit)) // 1.ī�޶󿡼� 2. dir�������� ���� ����, 3.�´°� �ִٸ�(==true�����Ѱ�)
            {
                Debug.Log($"Raycast Camera @{hit.collider.gameObject.name}");
            }
        }
        /* Ư�� ���̾ ����ʹٸ� ������ �߰������ϸ��
        int mask = (1 << 8); //��Ʈ�����ڸ� ���� 8���� Ű�ڴ�. = 8�����̾ ���ڴ�.
                             // ���⼭ | ^�� ���� ������ ����ũ ��밡��
                             // ex) 8���̳� 9���� Ű�ڴ�. int mask = (1<<8) | (1<<9); 
        if (Physics.Raycast(ray, out hit, mask))
            Debug.Log($"Raycast Camera @{hit.collider.gameObject.name}");
        
         ���� ������ ��������� �ִ�.
        LayerMask mask = LayerMask.GetMask("Monster") | LayerMask.GetMask("Wall");
         */

        

    }
}
