using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Lock
    {
        AutoResetEvent _available = new AutoResetEvent(true); // 커널에서의 bool이라고 생각하면 이해 편하다.
        //ManualResetEvent _available = new ManualResetEvent(true);

        public void Acquire() 
        {
            _available.WaitOne(); //입장 시도
            //말그대로 Auto이기에 하나가 들어가면 자동으로 닫힘. 그래서 코드도 위 한줄이면 끝
            //_available.Reset(); //만약 매뉴얼이벤트로 사용한다면, 직접 락을 닫아줘야됨. 대신 여러 작업이 락안으로 들어갈수있다.
        }

        public void Relsease()
        {
            _available.Set(); // Acquire에서 닫은 bool (=_available)을 다시 열어줌 , flag = true
        }        
    }

    class Program
    {
        static int _num = 0;
        static Lock _lock = new Lock();

        static void Thread_1()
        {
            for(int i=0;i<10000;i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Relsease();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 10000; i++)
            {
                _lock.Acquire();
                _num--;
                _lock.Relsease();
            }

        }
        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);

        }
    }
}
 