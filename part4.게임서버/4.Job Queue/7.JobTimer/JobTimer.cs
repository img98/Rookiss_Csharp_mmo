using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    struct JobTImerElem : IComparable<JobTImerElem> //PriorityQueue에 넣을거니 ICOmparable 인터페이스가 있어야함
    {
        public int execTick; //실행시간
        public Action action; 

        public int CompareTo(JobTImerElem other)
        {
            return other.execTick - execTick; //상대방의 실행틱과 내 실행틱을 비교
        }
    }

    class JobTimer
    {
        PriorityQueue<JobTImerElem> _pq = new PriorityQueue<JobTImerElem>();
        object _lock = new object();

        public static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAfter = 0)
        {
            JobTImerElem job;
            job.execTick = System.Environment.TickCount + tickAfter; // 잡이 실행되어야할시간.
            job.action = action;

            lock(_lock) //_pq가 공용데이터니 락 필요
            {
                _pq.Push(job);
            }
        }

        public void Flush()
        {
            while(true)
            {
                int now = System.Environment.TickCount;

                JobTImerElem job;

                lock(_lock)
                {
                    if (_pq.Count == 0)
                        break;

                    job = _pq.Peek();
                    if (job.execTick > now) //아직 실행할때가 안됬으면 break
                        break;

                    _pq.Pop();
                }
                //위 과정을 모두 통과하면, 실행해도 된다는 된다는 뜻이니
                job.action.Invoke(); //실행해주세요
            }
        }

    }
}
