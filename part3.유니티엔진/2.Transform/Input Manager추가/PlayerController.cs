using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;


    void Start()
    {
        Managers.Input.KeyAction -= OnKeyboard; //2. Ȥ�ö� �ٸ������� ������û�� �ϰ� ������������, �������� �����ϰ� �ٽ� ��û�ϴ� ���� (���� 2���ϸ� ��ȣ�� 2���ͼ� �ڵ尡 2���� �����)
        Managers.Input.KeyAction += OnKeyboard; //1. �Ŵ���.��ǲ(=��ǲ�Ŵ���)�� Ű�׼ǿ�, ��Ű���带 ������û�Ѵ�. =��, inputManager���� �Ű�� �����ٸ�(KeyAction), ���� �Լ�(OnKeyboard)�� �����ش޶�� ��
    }

    void Update()
    {

    }

    void OnKeyboard()
    {
        if (Input.GetKey(KeyCode.W))
        {   transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
            transform.position += Vector3.forward * Time.deltaTime * _speed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
            transform.position += Vector3.back * Time.deltaTime * _speed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
            transform.position += Vector3.left * Time.deltaTime * _speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
            transform.position += Vector3.right * Time.deltaTime * _speed;
        }

    }
}
