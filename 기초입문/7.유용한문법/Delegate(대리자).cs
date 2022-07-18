using System;

namespace _7.유용한기타문법
{
    class Program
    {
        // delegate (대리자) 자주쓰고 중요한 문법
        // 주어진함수ex)buttonPressed의 내용을 수정하는건 쉽지않다.
        // 그렇기에 맞춤식 함수를 만들어 인자로 넘겨주고 내부함수를 호출하는식으로 해야한다.

        // ex) 비서에게 연락해 연락처/용건을 남겨 나중에 거꾸로 연락달라함

        static void ButtonPressed( /* 원하는 작동 행위를 인자로 넘기고 */ ) //이게 우리 연락처 같은거
        {
            // 함수를 호출();
        }  //내용과 인자가 거꾸로 바뀜

        //한줄요약 : delegate = 함수를 인자로 넘겨주는 방법

        delegate int Onclicked(); //delegate 가 붙으면 형식(type)이 된다. 함수자체를 인자로 넘길수 있는 형식
        // 위 경우,  반환=int / 입력=void / delegate형식의이름 = OnClicked / 라는걸 알수있다.

        static void ButtonPressed2(Onclicked clickedFunction)
        {
            clickedFunction();
        }
        // 클릭이 되면(ButtonPressed2 내용) clickedFunction의 행위가 발생한다.(인자)

        static int TestDelegate() //원하는 행위
        {
            Console.WriteLine("TestDelegate worked");
            return 0; 
        }

        static void Main(string[] args)
        {
            ButtonPressed2(TestDelegate);
        }
    }
    
    // 즉, delegate 란 추가로 원하는 행위가 있을때, 그 추가파트를 외부에서 만들어 인자로 삽입하고, 내부에서 그 파트를 호출하는 형식인거임.!!!
    // (함수 내부에서 코드를 추가시키는게 아니라 외부에서 만든 함수를 불러오기만 한다는것)
}
