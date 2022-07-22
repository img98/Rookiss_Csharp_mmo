using System;
using System.Collections.Generic;

namespace Algorithm_Graph
{
    // [ 스택과 큐 ]

    // 스택 : LIFO (Last In First Out)
    // Stack<int> stack = new Stack<int>();

    // 큐 : FIFO (First In FIrst Out)
    // Queue<int> queue = new Queue<int>();

    // [ 그래프 이론 ]
    // Vertex(정점, 꼭지점) : 데이터를 표현
    // Edge(간선) : 정점들을 연결
    class Program
    {
        // [ 그래프 생성 ]
        class Graph // 그래프는 행렬, 또는 리스트로 표시할수있다.
        {
            int[,] adj = new int[6, 6] // 행렬로 만들어보자.
            {
                { 0, 1, 0, 1, 0, 0 },
                { 1, 0, 1, 1, 0, 0 },
                { 0, 1, 0, 0, 0, 0 },
                { 1, 1, 0, 0, 1, 0 },
                { 0, 0, 0, 1, 0, 1 },
                { 0, 0, 0, 0, 1, 0 }, // 수업중 사용한 그래프임. 이미지는 따로 첨부안하겠음
            };

            List<int>[] adj2 = new List<int>[] //리스트. 정점간의 연결 개념만을 담았다.
            {
                new List<int>() { 1, 3 },
                new List<int>() { 0, 2, 3 },
                new List<int>() { 1 },
                new List<int>() { 0, 1, 4 },
                new List<int>() { 3, 5 },
                new List<int>() { 4 },
            };

            // [ 그래프 순회하는법 ] (DFS, BFS)
            // [ 1. DFS ] (Depth First Search / 깊이 우선 탐색)
            // 갈 수 있는데까지 쭉 돌진. 더이상 갈 수 없으면 돌아와서 가보지 않은 길 탐색.
            // 로직
            // 1) 우선 now부터 방문하고
            // 2) now와 연결된 정점들을 하나씩 확인해서, 아직 미방문 상태라면 방문한다.

            bool[] visited = new bool[6]; //내용을 안넣어주면 false로 초기설정됨
            public void DFS(int now) // DFS의 경우 무조건 시작점이 있어야한다.(일단 now라고 설정함)
                                     // 행렬로 만든 그래프 경우
            {
                Console.WriteLine(now);
                visited[now] = true; // 1) 우선 now 방문 

                for (int next = 0; next < 6; next++) 
                {
                    if (adj[now, next] == 0) // 나(now)와 상대방(next)이 연결이 안됐다면 스킵
                        continue;
                    else if (visited[next] == true) //방문했다면 스킵 (교수의 경우 ==true를 안쓰더라. 난 가독성을 위해 쓰자)
                        continue;
                    DFS(next); // next에서 위와같은 스킵여부를 다시 확인하니, 재귀함수로 쓰면 되겠다.
                }
            }
            public void DFS2 (int now) // list로 만든 그래프 경우 DFS
            {
                Console.WriteLine(now);
                visited[now] = true; // 1) 우선 now 방문 

                foreach(int next in adj2[now]) // DFS1 과는 달리 연결이 안됐다면을 체크할 필요가없음.
                {
                    if (visited[next] == true)
                        continue;
                    DFS2(next);
                }
            }
            public void SearchAll() // 만약 모든정점끼리 연결되어 있지않은경우, 방문하지 않은 정점을 찾아 거기서 DFS다시 시작.
            {
                visited = new bool[6];
                for (int now = 0; now < 6; now++)
                    if (visited[now] == false)
                        DFS(now);
            }


            // [ 2. BFS ] (Breadth First Search / 너비 우선 탐색)
            // 거리 가까운 애들 순으로 들림, ex) 거리 1인 1,3노드 보고 거리 2인 2,4노드보고...이런식
            // 예약방문 개념을 사용하면 쉽다. (예약을 FIFO로 채움) 방문하면 해당 노드의 연결된 애들을 전부 예약에 넣는다.
            public void BFS(int start)
            {
                bool[] found = new bool[6];
                Queue<int> q = new Queue<int>();
                q.Enqueue(start);
                found[start] = true;

                while(q.Count>0) // 큐에 남아있는게 있다면 
                {
                    int now = q.Dequeue();
                    Console.WriteLine(now);

                    for (int next = 0; next < 6; next++)
                    {
                        if (adj[now, next] == 0) // 연결여부 확인
                            continue;
                        else if (found[next] == true) //방문여부 확인 //로직은 visited와 같다.
                            continue;
                        q.Enqueue(next);
                        found[next] = true;
                    }
                }
                // 사실 DFS는 여러곳에서 쓰지만 BFS의 경우 최단경로 탐색의 경우에만 사용한다.
                // 최단 탐색의 경우 parent와 distance라는 추가적인 배열을 사용하면된다.
            }
        }



        static void Main(string[] args)
        {
            Graph graph = new Graph();
            graph.DFS(2);
        }
    }
}
