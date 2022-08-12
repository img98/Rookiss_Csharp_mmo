using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
    // <락을 만들때 정해야하는 정책>
    // 1. 재귀적 락을 허용할것인지? (No)
    // 2. 스핀락 정책(5000번 -> Yield)
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000; // 비트연산자로 봤을때, 앞 15비트만 보기위해 7FFF 0000 (16비트가 아니라 15이기에 7이 들어간다.)
        const int READ_MASK = 0x0000FFFF; // 뒤 16비트만 보기위해, 앞은 0000 뒤는 FFFF //참고로 0xF=1111 로 해당 4개비트 모두 보여주겠다는뜻
        const int MAX_SPIN_COUNT = 5000;

        // [Unused(1비트)] [WriteThread Id(15비트)] [Read Count(16비트)] =int니까 32비트
        int _flag = EMPTY_FLAG;

        public void WriteLock()
        {//아무도 WriteLock or ReadLock을 획득하고 있지 않을때, 경합해서 소유권을 얻는다.
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK; //현재쓰레드 id를 얻어오고, 거기에 16비트만큼 밀어 WriteThread ID의 위치까지 보내준다.
                                                                          //그후 비트논리곱으로 마스크를 곱해준다(곱해주면 Unused와 ReadCOunt부분은 0으로 밀리게 될것임=>15비트 부분만 추출).            
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG) //시도해서 성공하면 return  
                        return;
                }
                Thread.Yield(); // 실패시 양보
            }

        }

        public void WriteUnlock()
        {
            Interlocked.Exchange(ref _flag, EMPTY_FLAG); //그냥 빈플래그로 바꿔주면 간단. 어쩌피 이동안에는 ReadLock도 안돌고있으니 신경쓸필요없다.
        }

        public void ReadLock()
        {//아무도 WriteLock을 획득하고 있지 않으면, ReadCount를 1 늘린다. => WriteThreadID가 0이면 ReadCount를 1 늘리면됨
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_MASK); // read마스크를 곱하면, Read Count부분이 추출됨과 동시에 자연스럽게 write Thread ID부분이 0이됨. 그렇기에 writeLock을 사용하지 않는 상황까지 한번에 얻어낼수있다.
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected) // if문을 통해 두가지 효과를 얻을수 있다. 1. _flag의 WriteThread Id부분이 사용중이라면, expected와는 무조건 함께할수없음.
                                                                                                    // 2. 동시에 ReadLock들이 경합하게 된다면, _flag에 expected+1을 넣음으로써 순서를 만들어줄수있음
                        return;                        
                }
                Thread.Yield(); // 실패시 양보
            }
        }

        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag); //read Count를 1 줄여주면 되는것인데, 어쩌피 1올리고 내리는게 _flag의 맨뒤 비트에서 일어나니, 그냥 decrement써줘도 된다.
        }
    }
}

