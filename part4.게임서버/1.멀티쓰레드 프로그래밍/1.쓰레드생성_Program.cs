using System;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        static void MainThread(object state)
        {
            while(true)
                Console.WriteLine("A");
        }
        static void Main(string[] args)
        {
            Thread t = new Thread(MainThread);
            t.Name = "Thread t";
            t.IsBackground = true; //사실 백그라운드에서 돌린다는게 이해가 잘 안되네, 이렇게해도 출력은 되던데. 물론 끝나긴하더라. t가 main에종속된다는 의미인가?
            t.Start();

            //ThreadPool.QueueUserWorkItem(MainThread); //인력사무소의 쓰레드
            while(true)
                Console.WriteLine("B");            

        }
    }
}
 