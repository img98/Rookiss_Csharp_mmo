using System;
using System.Collections.Generic;

namespace PriorityQueue_Heap
{
    class Program
    {
        // 힙 이론
        // 부모노드value는 항상 자식노드보다 크다.
        // 모든 레벨을 위 그리고 왼쪽부터 차곡차곡 채워가야 된다.
        // 위 특징들로 인해 '트리의 형태'를 쉽게 알수있다. 그리고 이것은 코드를 짤때 엄청난 이점이 된다.
        
        class PriorityQueue // 우선순위가 있는 큐
        {
            List<int> _heap = new List<int>();

            public void Push(int data)
            {
                _heap.Add(data); // 리스트 Add.(힙이란 함수가 있는게아니라, 지금 리스트로 힙개념을 구현하는거임)

                int now = _heap.Count - 1; // now = 지금노드의 인덱스
                while (now > 0) // root까지 올라가기
                {
                    int next = (now - 1) / 2; // 힙노드의 부모찾는 공식 (코드짤때 이점이 이런거임)
                    if (_heap[now] < _heap[next]) //부모와 자식 크기 재정렬(도장깨기)
                        break;
                    int temp = _heap[now]; // 도장깨기 성공이면 교체
                    _heap[now] = _heap[next];
                    _heap[next] = temp;

                    now = next; // 이제 올라간 위치에서 다시한번 도장깨기
                }
            } // O(log N) //엄밀히는log2 N

            public int Pop()
            {
                int ret = _heap[0]; //힙은 무조건 루트가 가장큰값이니, return = 히프의 인덱스0의 value

                // 힙 구조의 root를 없애면, 가장 마지막(우하)에 있는 node를 root로 데려와서 역으로 재정렬 한다.
                int lastIndex = _heap.Count - 1; ;// 사실 lastIndex 안만들어도 되는데, 가독성을 위해 만듬
                _heap[0] = _heap[lastIndex];
                _heap.RemoveAt(lastIndex) ;
                lastIndex--;

                int now = 0; //역으로 내려가는 도장깨기. 시작점은 root(Index=0)
                while (true)
                {
                    int left = 2 * now + 1; //왼쪽자식
                    int right = 2 * now + 2; //오른쪽자식

                    int next = now;
                    // left<=lastIndex 는 인덱스가 left가 트리안에 있다는뜻. //현재값이(next=now) 왼쪽값보다 작으면, 왼쪽으로 이동
                    if (left <= lastIndex && _heap[next] < _heap[left]) 
                        next = left;
                    // 현재값(왼쪽이동 포함)이 오른쪽보다 작으면 오른쪽으로 이동
                    if (right <= lastIndex && _heap[next] < _heap[right])
                        next = right;
                    // 현재가 양쪽보다 크다면(두if문 작동안했다면)
                    if (next == now)
                        break;

                    //두 값을 교체한다.
                    int temp = _heap[now];
                    _heap[now] = _heap[next];
                    _heap[next] = temp;

                    now = next; // 다음 검사 위치로
                }                
                return ret;
            } // O(log N) 

            public int Count()
            {
                return _heap.Count; //그냥 리스트에 있는 기능을 사용하면됨.
            }

            // 만약 queue를 작은순서대로 보고싶다면?
            // 위 Pop() 의 비교조건을 바꿔주는 방법이 있고 (이 경우 트리 자체가 root가 작고 leaf가 커짐)
            // 입력 출력단계에서 -를 붙여주는 방법이 있다.
        }


        static void Main(string[] args)
        {
            PriorityQueue q = new PriorityQueue();
            q.Push(20);
            q.Push(10);
            q.Push(30);
            q.Push(90);
            q.Push(40);

            while(q.Count()>0)
            {
                Console.WriteLine(q.Pop());
            }

            /*PriorityQueue q = new PriorityQueue();
            q.Push(-20);
            q.Push(-10);
            q.Push(-30);
            q.Push(-90);
            q.Push(-40); //push에 -를 붙이면 -10,-20... 순으로 나열될것이고

            while (q.Count() > 0)
            {
                Console.WriteLine(-q.Pop()); // -q.Pop을 사용하면 -10이 10으로 나와, 10,20...출력가능
            }*/
        }
    }
}
