using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct MyVector // 벡터3를 구현하며 이해해보자.
{
    // 게임에서 자주사용하는 벡터
    //1. 위치 벡터
    //2. 방향 벡터

    public float x;
    public float y;
    public float z;

    public float magnitude { get { return Mathf.Sqrt(x*x + y*y + z*z); } } //return the legnth of this vector (즉, 크기를 반환한다.) // 피타고라스 정리로 구할수있다.
    public MyVector normalized { get { return new MyVector(x / magnitude, y / magnitude, z / magnitude); } } //magnitude가 1인 이 벡터를 반환한다. 즉, 방향만 보겠다.

    public MyVector(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }

    public static MyVector operator +(MyVector a, MyVector b) // 두 벡터를 덧셈연산(+) 했을때 무슨 행위가 벌어지느냐
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
        pos += new MyVector(0.0f, 2.0f, 0.0f); // 이게 오퍼레이터+ 사용한것
        // 단, 지금의 경우 pos와 Transform의Position과 연결하지 않았기에, 위 코드는 딱히 효과가 없음. 그냥 설명용 코드

        MyVector to = new MyVector(10.0f, 0.0f, 0.0f);
        MyVector from = new MyVector(5.0f, 0.0f, 0.0f);
        MyVector dir = to - from; // (5.0f, 0.0f, 0.0f)

        dir = dir.normalized; // (1.0f, 0.0f, 0.0f)

        MyVector newPos = from + dir * _speed;

        // 방향 벡터 
        // 1. 거리(크기)  5 magnitude
        // 2. 실제 방향 ->  (1.0f,0.0f,0.0f)
    }

    float _yAngle = 0.0f;
    void Update()
    {
        // transform.TransformDirection() = Local -> World
        // InverseTransformDirection() = World -> Local

        if (Input.GetKey(KeyCode.W))
        {
            //transform.rotation = Quaternion.LookRotation(Vector3.forward); // 월드좌표상 forward를 보게한다.
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
            //transform.position += transform.TransformDirection(Vector3.forward * Time.deltaTime * _speed); // 회전없이 로컬방향으로 움직이게 했을때 공부했던 코드
                // Vector3.forward * Time.deltaTime * _speed = 로컬 단위로 계산
                // transform.TransformDirection = 그걸 월드단위로 변환
                // 사실 이런거 외울수는 없고, 필요할때마다 구글에 찾아쓰는게 좋다.
            //transform.Translate(Vector3.right * Time.deltaTime * _speed); // Translate도 Local단위로 움직이게하는 방법.
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
            //transform.Translate(Vector3.right * Time.deltaTime * _speed); // Translate는 Local단위로 생각하는 방법.
        }
            

        // transform.rotation 얘는 vector3 타입이 아니다.
        _yAngle += Time.deltaTime * 100.0f;
        // 절대 회전값
        //transform.eulerAngles = new Vector3(0.0f, _yAngle, 0.0f);  //다른 방법도 보기위해 얘는 비활성화
        //eulerAngles의 경우 공식document를 보면 absolute values(_yAngle처럼)로 쓰고 increment하지말라 되어있다. 360도를 초과하면 에러가 날 수 있기 때문

        //increment(덧셈뺄셈) 하고 싶으면 대신 Rotate를 쓰라고 나와있다.
        // +- delta (특정 축을 기준으로 얼마만큼 회전시키고 싶다면)
        //transform.Rotate(0.0f, Time.deltaTime * 100.0f, 0.0f); // 얘는 오히려 _yAngle쓰면 회전속도가 이상해지더라

        //transform.rotation =Quaternion.Euler(new Vector3(0.0f, _yAngle, 0.0f)); //Euler => Quaternion타입을 벡터타입으로 반환해줌

    }

}
