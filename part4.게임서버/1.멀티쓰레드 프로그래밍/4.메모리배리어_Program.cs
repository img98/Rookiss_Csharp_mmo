using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        //메모리 재배치 = 1.코드 재배치 억제, 2. 가시성 확보
        
        int _answer;
        bool _complete;

        void A()
        {
            _answer = 123;
            Thread.MemoryBarrier();
            _complete = true;
            Thread.MemoryBarrier();
        }

        void B()
        {
            Thread.MemoryBarrier();
            if(_complete)
            {
                Thread.MemoryBarrier();
                Console.WriteLine(_answer);
            }
        }
        static void Main(string[] args)
        {
            
        }


    }
}
 