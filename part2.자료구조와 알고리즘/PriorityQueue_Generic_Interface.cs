using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PriorityQueue_Heap
{
    class Program
    {
        // heap을 사용해 만든 우선순위 큐를 좀더 범용성 있게 만들어보자. Generic 사용
        // 그러나 Generic의 경우 아래 if (_heap[now] < _heap[next]) 과 같은 대소비교 할거리가 없을수도 있음.
        // 그러니 Interface를 사용하자!
        class PriorityQueue<T> where T : IComparable<T> // T는T인데 IComparable이 가능한 T여야한다.라는뜻(IComparable은 이미있음)
        {
            List<T> _heap = new List<T>();

            public void Push(T data)
            {
                _heap.Add(data); 

                int now = _heap.Count - 1; 
                while (now > 0) 
                {
                    int next = (now - 1) / 2;
                    if (_heap[now].CompareTo(_heap[next]) < 0)  //기존 대소비교를 CompareTo형식으로 바꿔줘야됨 
                        break;
                    T temp = _heap[now]; 
                    _heap[now] = _heap[next];
                    _heap[next] = temp;

                    now = next; 
                }
            }

            public T Pop()
            {
                T ret = _heap[0];
                           
                int lastIndex = _heap.Count - 1; ;
                _heap[0] = _heap[lastIndex];
                _heap.RemoveAt(lastIndex) ;
                lastIndex--;

                int now = 0;
                while (true)
                {
                    int left = 2 * now + 1;
                    int right = 2 * now + 2;

                    int next = now;
                    if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0) 
                        next = left;
                    if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0) 
                        next = right;
                    if (next == now)
                        break;

                    T temp = _heap[now];
                    _heap[now] = _heap[next];
                    _heap[next] = temp;

                    now = next;
                }                
                return ret;
            } 

            public int Count()
            {
                return _heap.Count; //그냥 리스트에 있는 기능을 사용하면됨.
            }

        }

        class Knight : IComparable<Knight> //IComparable이 가능한 Knight //'잠재적수정사항->인터페이스 구현'을통해 기본양식을 가져올수있다.
        {
            public int Id { get; set; }

            public int CompareTo(Knight other) //CompareTo에 손올려보면 어떤식으로 만들라고 써있음
            {
                if (Id == other.Id)
                    return 0;
                return Id > other.Id ? 1 : -1; //내 id가 더크면 1 리턴이고 아니면 -1 //만약 대->소 순서를 소->대 로 하고싶으면 Id < other.Id로 부호만 바꾸면된다.

                throw new NotImplementedException(); //기본양식
            }
        }

        static void Main(string[] args)
        {
            PriorityQueue<Knight> q = new PriorityQueue<Knight>();
            q.Push(new Knight() { Id = 20 });
            q.Push(new Knight() { Id = 30 });
            q.Push(new Knight() { Id = 40 });
            q.Push(new Knight() { Id = 10 });
            q.Push(new Knight() { Id = 5 });

            while (q.Count()>0)
            {
                Console.WriteLine(q.Pop().Id);
            }
        }
    }

    // 새 문법에 대한 설명은 적었다. 그냥 Generic과 Interface의 사용법에 주목하자.
}
