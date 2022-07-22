using System;

namespace cs
{
    class 디버깅기초
    {
        static int Add(int a, int b)
        {
            int ret = a + b;
            return ret;
        }
        static void Main(string[] args)
        {
            int ret = 디버깅기초.Add(10, 20);  //f10 프로시저 단위 진행은 메소드 단위로 디버깅하는것.
            Console.WriteLine(ret);             //f11 코드 단위진행을 하면 메소드 안의 코드 한줄까지 들어감. //함수내의 함수호출이 있다면 엄청 복잡해짐
        }                                       //위와 같이 말한 메소드내의 depth를 파악할때 디버그 내 '호출스택'으로 자기 위치를 알수있다.
    }

    //breakpoint를 조건에 따라 발생하게할수도있다. 유용함.
}
