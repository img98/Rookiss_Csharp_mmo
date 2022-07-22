using System;

namespace _7.유용한기타문법
{

    // delegate 에도 문제가 있다. = 함수의 접근이 쉬워지다보니 원치 않는곳에서도 호출하기가 쉽다.
    // 위를 보완하기 위한 방법으로 Event를 사용한다.



    class Program
    {
        static void OnInputTest()
        {
            Console.WriteLine("input Received");
        }

        static void Main(string[] args)
        {
            InputManager inputManager = new InputManager();

            inputManager.InputKey += OnInputTest;

            while (true)
            {
                inputManager.Update();

            }

        }

    }
    
}
