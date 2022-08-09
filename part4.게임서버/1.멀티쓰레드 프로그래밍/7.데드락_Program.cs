using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        class SessionManager
        {
            static object _lock = new object();

            public static void TestSession()
            {
                lock (_lock)
                {

                }
            }
            public static void Test()
            {
                lock (_lock)
                {
                    UserManager.TestUser();
                }
            }

        }
        class UserManager
        {
            static object _lock = new object();

            public static void Test()
            {
                lock (_lock)
                {
                    SessionManager.TestSession();
                }
            }
            public static void TestUser()
            {
                lock(_lock)
                {

                }
            }
        }
        // 위상황은 Session매니저에서는 User매니저를 호출하고, User매니저는 Session매니저를 호출하는데, 서로 락이 걸려있는 상태이기에, 서로 서로가 끝나기를 기다리고만 있어 deadlock이 발생한다.

        static void Thread_1()
        {
            for (int i = 0; i < 10000; i++)
            {
                SessionManager.Test();
            }
                
        } //이런상황 외에도 예상치 못한 exception이 생겨 Monitor.Exit를 챙기지 못할수도 있다.->데드락 발생 ->이를 방지하기 위해 try catch나 lock(_obj) {}를 사용한다.

        static void Thread_2()
        {
            for (int i = 0; i < 1000; i++)
            {
                UserManager.Test();
            }
        }
        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            
            t1.Start();

            //Thread.Sleep(100); //사실 이렇게 시간을 조금만 어긋나게 해줘도 deadlock이 적어지긴한다.

            t2.Start();

            Task.WaitAll(t1, t2);

        }
    }
    //결론 : 데드락은 터지면 로그보고 해결하는게 가장 편하다.
}
 