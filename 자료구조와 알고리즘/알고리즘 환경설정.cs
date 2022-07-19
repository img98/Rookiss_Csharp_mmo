using System;

namespace DataStructure_Algorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false; //커서가 0,0으로 돌아갈때 반짝이는 이펙트를 지우기위함.

            int lastTick = 0; // 프레임을 확인하기 위한, 이전전시간을 기록하는 틱
            while (true)
            {
                #region //30프레임마다 작동 구현
                // region =별건아니고 비쥬얼스튜디오에서 열고닫을수있게 해주는 코드. 쓰면편하다.
                int currentTick = System.Environment.TickCount; // 무언가 작동후의 시간을 뱉는 코드
                int elapsedTick = currentTick - lastTick; // 두 틱의 차를 통해 경과시간을 구할수있겠지
                // 만약 경과시간이 1/30초보다 작다면 
                if (elapsedTick < 1*1000 / 30) // tick의 단위가 ms이기때문에 초로 하려면 1000을 곱해줘야한다.
                    continue; // 1/30 초마다 문을 열어서 코드를 작동하겠다. 마치 30프레임 렌더링처럼
                lastTick = currentTick;

                //사실 위에도 하드코딩이 많기때문에 여러 변수설정을 통해 줄일수있다.
                // elapsedTick을 굳이 만들지 말고 그냥 if문에 넣기.
                // 1*1000/30 을 쓰지말고 변수로 만들기 ex) const int WAIT_TICK = 1000/30 ;
                #endregion

                //렌더링
                Console.SetCursorPosition(0, 0); // 무조건 커서를 0,0으로 회귀시킴. 이경우 writeline을 해도 한줄이 한줄을 지우며 무한히 출력됨.
                Console.WriteLine("Hello World!");
                
                for (int i =0; i<25; i++)
                {
                    for (int j = 0;j<25;j++)
                    {
                        Console.Write('\u25cf');
                    }
                    Console.WriteLine();
                }

            }
        }
    }
}
