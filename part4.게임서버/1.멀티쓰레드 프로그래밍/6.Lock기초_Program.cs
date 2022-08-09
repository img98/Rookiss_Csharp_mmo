using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        static int number = 0;
        static object _obj = new object();

        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                // 상호배제 Mutual Exclusive

                Monitor.Enter(_obj); //문을 잠금

                number++;

                if(number==1000)
                {
                    Monitor.Exit(_obj);
                    return;
                }

                Monitor.Exit(_obj); // 잠금 풀기
            }
                
        } //이런상황 외에도 예상치 못한 exception이 생겨 Monitor.Exit를 챙기지 못할수도 있다.->데드락 발생 ->이를 방지하기 위해 try catch나 lock(_obj) {}를 사용한다.

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                lock(_obj)
                {
                    number--;
                }
            }
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
 