using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public interface IJobQueue
    {
        void Push(Action job); //job을 좀더 구체화해서 넣어줘도 되지만, 일반적으로 c#에서 행위를 Action으로 표현하는경우가 많아서, delegate로 넣어줌
    }

    public class JobQueue : IJobQueue
    {
        Queue<Action> _jobQueue = new Queue<Action>();
        object _lock = new object();
        bool _flush = false; //flush = 물내림 = 예약된것들 처리시작


        public void Push(Action job)
        {
            bool flush = false;
            lock (_lock)
            {
                _jobQueue.Enqueue(job);
                if (_flush == false) // 지금 _flush하고있지 않다면 (예약되어있는게 없다면)
                    flush = _flush = true; //_flush를 단계적으로 풀기위해 flush도 생성. 이렇게 명령만 만들어서 lock안에서의 작업을 줄일수있다.
            }
            if (flush)
                Flush();
        }

        void Flush()
        {
            while(true)
            {
                Action action = Pop();
                if (action == null)
                    return;

                action.Invoke(); //일감이 있다면 실행
            }
        }

        Action Pop()
        {
            lock (_lock) //Push의 lock으로 인해 Flush는 하나만 돌지라도, flush가 도는동안 큐에 Enqueue가 될수있기에, Pop과정에서 lock이 필요하다.
            {
                if (_jobQueue.Count == 0) //잡큐가 비어있다면 null리턴 //비어있다 = 예약된게 없다 or 전부 마쳤다.
                {
                    _flush = false; //일처리 다했으니 _flush해도 된다고 알림
                    return null;
                }
                return _jobQueue.Dequeue();
            }
        }
    }

}
