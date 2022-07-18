using System;
using System.Collections.Generic;
using System.Text;

namespace _7.유용한기타문법
{
    // 이 클래스는 사용자의 입력을 캐치해서 로직에다가 보내는 중간매개 클래스 가 될것임.
    class InputManager
    {
        public delegate void OnInputKey();
        public event OnInputKey InputKey;

        public void Update() //사용자가 A를 입력했다면 모두에게 알리는 함수를 만들어보자.
        {
            if (Console.KeyAvailable == false)
                return;

            ConsoleKeyInfo info = Console.ReadKey();
            if (info.Key == ConsoleKey.A)
            {
                //모두에게 알린다.
                //여기다가 코드를 직접 짜는 것은 비효율적임. => 그렇기에 delegate를 사용하자.
                InputKey();
            }

        }
    }
}
