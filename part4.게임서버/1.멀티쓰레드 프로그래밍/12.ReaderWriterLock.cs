using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        //<복습>
        // Lock의 구현 => 1.근성(뺑뺑이) 2.양보 3.갑질 ==>결국 모두 상호배제

        static object _lock = new object();
        static SpinLock _lock2 = new SpinLock();

        //위처럼 제공되는 애들을 써도되고, 직접만들어도된다.


        static ReaderWriterLockSlim _lock3 = new ReaderWriterLockSlim();

        class Reward
        {
        }

        static Reward GetRewardByID(int id)
        {
            _lock3.EnterReadLock(); // 아무도 WriteLock을 잡고있지 않다면, 동시다발적으로 사용 허가

            _lock3.ExitReadLock();

            return null;
        }
        
        static Reward AddReward(Reward reward) //자주 변하지 않을, 하지만 수정이 있을수도 있는 코드
        {
            _lock3.EnterReadLock(); //얘네가 있으면 readLock은 접근불가
            _lock3.ExitReadLock();

            return null;
        }

    static void Main(string[] args)
        {

        }
    }
}
 