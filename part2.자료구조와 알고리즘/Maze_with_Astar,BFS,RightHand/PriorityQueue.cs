using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure_Algorithm
{
    // 미로찾기 알고리즘은 크게 두가지로 나뉜다.1)시작과 끝위치를 안다. 2)시작위치만 안다.
    // 사실 Dijikstra는 (2)에 해당하기에, 미로의 모든 구석까지 가봐야한다. = 낭비
    // (1)의 대표는 A*(에이스타) 알고리즘이 있다.
    // 출구에 가까워질수록 가산점을 부여. 
    // Astar에 우선순위 큐가 쓰이는데, 그 이유는 얘가 '하나의 최고 케이스 찾는데 좋은' 알고리즘 이기 때문이다.

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
            _heap.RemoveAt(lastIndex);
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

        public int Count{get{return _heap.Count; } } //Count함수를 Property형태로 바꿧다.(보기좋으라고)

    }
}
