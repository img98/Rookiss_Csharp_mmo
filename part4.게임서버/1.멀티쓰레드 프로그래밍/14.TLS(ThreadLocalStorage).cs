using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>(); // static = 전역메모리, 고치면 다른 쓰레드에도 영향감. => TLS에 넣어서(ThreadLocal 사용), 고쳐도 다른 쓰레드는 영향안가게해보자.

        static void WhoAmI()
        {
            ThreadName.Value = $"name is {Thread.CurrentThread.ManagedThreadId}";

            Thread.Sleep(1000);

            Console.WriteLine(ThreadName.Value);
        }

        static void Main(string[] args)
        {
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI); // Task만들어서 실행시키기 귀찮을때 쓰는방법, 인자로 들어간 업무들을 차례로 실행함.
        }
    }
    // 즉, ThreadName이라는 개인 공간을 만들어서 미리 옮겨담는 식으로, 다른 쓰레드에게 영향을 받지 않는다는 말인듯.
}
    