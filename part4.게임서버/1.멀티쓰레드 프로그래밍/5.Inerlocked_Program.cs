using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        //단순히 number++, number--를 멀티쓰레드로 돌리면, 
        static int number = 0;

        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
                //number++;
                Interlocked.Increment(ref number); //여기서 ref를 쓴 이유는, number의 주소값을 직접 넣어준거기때문.
                                                   //그냥 int number가 안되는 이유는, number를 부르는 순간, number에 연결된 주소로 찾아가 값을 알아와야 되기때문에, 상식적으로 한번에 일어날수 없음
                                                   //즉 ref를 쓴다는건, 이값이 무슨값인지 모르겠지만 주소로가서 1을 늘려주겠다는 것
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++) 
                //number--; //위에선 10만번++을하고 아래에선 10만번--를 했으니 number=0이어야할거같지만, 그렇지않다. =>그이유는, 코드의 원자성때문
                Interlocked.Decrement(ref number); //코드의 원자성을 지켜주기 위해선, Interlocked를 사용해야한다.
        }
        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }


    }
}
 