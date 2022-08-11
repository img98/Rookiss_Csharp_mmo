using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class SpinLock //사실 이름이 스핀락일뿐, lock상태를 compareExchange로 확인한 후 아직 lock이 걸려있다면, 어떤 행동을 할것인지에 따라 이름과 개념이 갈림
                   // 곧바로 다시확인 = 스핀락, 잠깐 쉬고 딴애들부터 확인 = 컨텍스트 스위칭
    {
        volatile int _locked = 0;

        public void Acquire() 
        {

            while(true)
            {
                if (Interlocked.CompareExchange(ref _locked, 1, 0) == 0)
                    break;

                Thread.Sleep(1); //무조건 양보
                //Thread.Sleep(0); //조건부양보 = 나보다 우선순위가 낮은애들한테는 양보하지 않겠다
                //Thread.Yield(); //관대한양보 = 지금 실행가능한 쓰레드가 있으면 실행해라
            }
        }

        public void Relsease()
        {
            _locked = 0;
        }        
    }

    class Program
    {
        static int _num = 0;
        static SpinLock _lock = new SpinLock();

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
 