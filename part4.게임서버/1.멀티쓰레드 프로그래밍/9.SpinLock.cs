using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class SpinLock //스핀락이라는 클래스를 만들어보자. (존버메타)
    {
        volatile int _locked = 0;

        public void Acquire() //Monitor.Enter 마냥 들어가거나, 나가는 함수 인터페이스를 만들자.
        {
            /* 이경우, _lock을 체크하는것과 그후 _lock을 거는것의 원자성이 지켜지지 않아, 원하는데로 결과가 나오지 않는다.
            while(_locked==1)
            {
                // locked=true라면 무한루프를 돌겠다. = 잠금이 풀리기를 기다림.
            }
            _locked = true; //잠금이 풀려서 들어왔으니, 내가쓰는동안 다시 잠금
            */

            /*이버전으로 해결은 가능하지만, original을 체크하는 과정이 귀찮음=> 한번에 할수없을까? =CompareExchange
            while(true)
            {
                int original = Interlocked.Exchange(ref _locked, 1); //Exchange는 ref int에다가, value를 넣어주는기능을 하는데, 넣기전값을 return함.                                                                
                if (original == 0) //그래서 return값을 응용한다면(orignial에 저장), 원래는 0이었는데 1로 바뀐순간을 추론할 수 있다.
                    break;
            }*/

            while(true)
            {
                //이런걸 CAS Comare-And_Swap이라고함 
                int original = Interlocked.CompareExchange(ref _locked, 1, 0); //ref값을 봐서, 값이 0(마지막인자)이면, ref값에 1(중간인자)을 넣어주겠다. (그리고 exchange처럼 original value를 리턴하긴함)

                if (original == 0)
                    break;  //지금 로직같은 경우엔, 그냥exchange와 별차이 없어보이지만, 원래 lock값이 0이였는지 비교 + lock에 새로 1대입 을 한번에 할 수 있다는게 핵심이다.
            
                //위를 짧게하면, if( Interlocked.CompareExchange(ref _locked, 1, 0)== 0 ) break; //사실 이건 그냥Exchange에서도 되긴함.
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
 