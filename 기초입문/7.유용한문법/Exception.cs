using System;

namespace _7.유용한기타문법
{
    class Program
    {
        // [Exception (예외처리)] // 게임쪽에선 잘 안쓰긴해
        //try{} catch() {} 형태로 사용됨 // try를 해보고 catch 예외가 발생하면 catch 내용대로 하겠다


        static void Main(string[] args)
        {
            try
            {
                // 오류를 일부러 만들어보자
                // 1. 0으로 나눌때
                // 2. 잘못된 메모리를 참조(null)
                // 3. 오버플로우
                int a = 10, b = 0;
                int result = a/b;

                //참고로 try도중에 예외가 나면 아래 내용은 작동하지 않음
                Console.WriteLine("No error occured!"); // 위 result에서 에러가 나오니, 이 구문은 나오지않을것

                throw new TestException(); // 이런식으로 원하는 타입의 예외를 만들어 발생시킬수도있다.
                // 이경우 맨아래 Exception e에서 타입이 test 얘로 니옴.
            }
            catch(DivideByZeroException e) // catch도 여러개 쓸수있다. 마치 if else문 마냥 작동함. 위 에러에서 걸리면 아래까지 안감
            {

            }
            catch (Exception e) // Exception은 모든 에러상황을 커버한다.
            {

            }
            finally
            {
                // 에러가 나도 무조건 나와야되는 내용이 있다면, 여기다 쓰면된다.
                Console.WriteLine("try catch worked well");
            }

            //여러 exception의 종류
            // 1.DivideByzeroException 
            // 우리가 원하는 커스텀타입의 예외를 만들수도있다.
        }
        class TestException : Exception
        {
            //원하는 타입의 exception을 만들수있다.
        }
    }

    
}
