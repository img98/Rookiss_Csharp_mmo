using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct MyVector // ����3�� �����ϸ� �����غ���.
{
    // ���ӿ��� ���ֻ���ϴ� ����
    //1. ��ġ ����
    //2. ���� ����

    public float x;
    public float y;
    public float z;

    public float magnitude { get { return Mathf.Sqrt(x*x + y*y + z*z); } } //return the legnth of this vector (��, ũ�⸦ ��ȯ�Ѵ�.) // ��Ÿ��� ������ ���Ҽ��ִ�.
    public MyVector normalized { get { return new MyVector(x / magnitude, y / magnitude, z / magnitude); } } //magnitude�� 1�� �� ���͸� ��ȯ�Ѵ�. ��, ���⸸ ���ڴ�.

    public MyVector(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }

    public static MyVector operator +(MyVector a, MyVector b) // �� ���͸� ��������(+) ������ ���� ������ ����������
    {
        return new MyVector(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public static MyVector operator -(MyVector a, MyVector b)
    {
        return new MyVector(a.x - b.x, a.y - b.y, a.z - b.z);
    }
    public static MyVector operator *(MyVector a, float b)
    {
        return new MyVector(a.x * b, a.y * b, a.z * b);
    }
}

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;


    void Start()
    {
        MyVector pos = new MyVector(0.0f, 5.0f, 0.0f);
        pos += new MyVector(0.0f, 2.0f, 0.0f); // �̰� ���۷�����+ ����Ѱ�
        // ��, ������ ��� pos�� Transform��Position�� �������� �ʾұ⿡, �� �ڵ�� ���� ȿ���� ����. �׳� ����� �ڵ�

        MyVector to = new MyVector(10.0f, 0.0f, 0.0f);
        MyVector from = new MyVector(5.0f, 0.0f, 0.0f);
        MyVector dir = to - from; // (5.0f, 0.0f, 0.0f)

        dir = dir.normalized; // (1.0f, 0.0f, 0.0f)

        MyVector newPos = from + dir * _speed;

        // ���� ���� 
        // 1. �Ÿ�(ũ��)  5 magnitude
        // 2. ���� ���� ->  (1.0f,0.0f,0.0f)
    }

    float _yAngle = 0.0f;
    void Update()
    {
        // transform.TransformDirection() = Local -> World
        // InverseTransformDirection() = World -> Local

        if (Input.GetKey(KeyCode.W))
        {
            //transform.rotation = Quaternion.LookRotation(Vector3.forward); // ������ǥ�� forward�� �����Ѵ�.
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
            //transform.position += transform.TransformDirection(Vector3.forward * Time.deltaTime * _speed); // ȸ������ ���ù������� �����̰� ������ �����ߴ� �ڵ�
                // Vector3.forward * Time.deltaTime * _speed = ���� ������ ���
                // transform.TransformDirection = �װ� ��������� ��ȯ
                // ��� �̷��� �ܿ���� ����, �ʿ��Ҷ����� ���ۿ� ã�ƾ��°� ����.
            //transform.Translate(Vector3.right * Time.deltaTime * _speed); // Translate�� Local������ �����̰��ϴ� ���.
            transform.position += Vector3.forward * Time.deltaTime * _speed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            //transform.rotation = Quaternion.LookRotation(Vector3.back);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
            transform.position += Vector3.back * Time.deltaTime * _speed;
        }
            
        if (Input.GetKey(KeyCode.A))
        {
            //transform.rotation = Quaternion.LookRotation(Vector3.left);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
            transform.position += Vector3.left * Time.deltaTime * _speed;
        }
            
        if (Input.GetKey(KeyCode.D))
        {
            //transform.rotation = Quaternion.LookRotation(Vector3.right);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
            transform.position += Vector3.right * Time.deltaTime * _speed;
            //transform.Translate(Vector3.right * Time.deltaTime * _speed); // Translate�� Local������ �����ϴ� ���.
        }
            

        // transform.rotation ��� vector3 Ÿ���� �ƴϴ�.
        _yAngle += Time.deltaTime * 100.0f;
        // ���� ȸ����
        //transform.eulerAngles = new Vector3(0.0f, _yAngle, 0.0f);  //�ٸ� ����� �������� ��� ��Ȱ��ȭ
        //eulerAngles�� ��� ����document�� ���� absolute values(_yAngleó��)�� ���� increment�������� �Ǿ��ִ�. 360���� �ʰ��ϸ� ������ �� �� �ֱ� ����

        //increment(��������) �ϰ� ������ ��� Rotate�� ����� �����ִ�.
        // +- delta (Ư�� ���� �������� �󸶸�ŭ ȸ����Ű�� �ʹٸ�)
        //transform.Rotate(0.0f, Time.deltaTime * 100.0f, 0.0f); // ��� ������ _yAngle���� ȸ���ӵ��� �̻���������

        //transform.rotation =Quaternion.Euler(new Vector3(0.0f, _yAngle, 0.0f)); //Euler => QuaternionŸ���� ����Ÿ������ ��ȯ����

    }

}
