using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    { 
        static bool _stop = false; //모든 쓰레드가 공유하는 static영역에 저장하여, 한꺼번에 관리하면 어떤 모습이 되는지 살펴보자.

        static void ThreadMain()
        {
            Console.WriteLine("쓰레드 시작!");

            while(_stop==false) // 릴리스 모드에서는 if(_stop==false){ while(ture) } 로 인식해서 쓰레드 종료가 안나오더라!!
            {
                //누군가 stop신호를 해주길 기다린다.
            }

            Console.WriteLine("쓰레드 종료!");
        }

        static void Main(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000); //1000ms 동안 잠깐 중지

            Console.WriteLine("Stop호출"); 
            _stop = true;//true변환이 떨어지자마자 ThreadMain의 쓰레드종료! 가 출력되기에, 이 cw"Stop호출"이 true위에 있는지 아래있는지에 따라 총 출력다르더라
           
            Console.WriteLine("종료 대기중");

            t.Wait();

            Console.WriteLine("종료 성공");
            
        }
    }
}
 