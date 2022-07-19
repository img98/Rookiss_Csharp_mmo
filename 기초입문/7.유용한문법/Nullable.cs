using System;
using System.Reflection;

namespace _7.유용한기타문법
{
    // [Nullable] 
    // null 로 리턴이 가능하게 하다.
    //int형의 경우 null로 리턴이 안됨. 그런데 꼭 Null을 쓰고싶다면
    // int? number = null; 
    // ?(물음표)를 붙이면 된다.

    class Program
    {
        static void Main(string[] args)
        {
            int? number = null;
            number = 3;

            // int a = number; //이 경우 number는 nullalbe 타입이기에 int a 에 넣을 수 없음.
            int a = number.Value; //메소드로 넣어줘야한다.

            //그래서 앞으론 number가 null 인지 확인하면서 사용가능하다.
            if(number != null)
            {
                Console.WriteLine(number.Value);
            }
            if(number.HasValue)
            {
                Console.WriteLine(number.Value);
            } //위 두 if문은 모두 number가 null인지 확인하는 방법임

            //위 방법(null인지 확인) 조차 귀찮다면?
            int b = number ?? 0; // number가 null이 아니라면 b에 number.value를 넣어주고 null이라면 ?? 뒷 값을 넣어준다는뜻
        }
    }

    // 즉, Nullable은 null 이 가능케해주는 타입
}
