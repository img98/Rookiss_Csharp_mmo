using System;
using System.Collections.Generic;

namespace Algorithm_Graph
{
    class Program
    {
        class Graph 
        {
            int[,] adj = new int[6, 6] // dijikstra를 위해 가중치가 있는 행렬그래프를 만들자
            {
                { -1, 15, 1, 35, -1, -1 },
                { 15, -1, 05, 10, 1, -1 },
                { -1, 05, -1, -1, -1, -1 },
                { 35, 10, -1, -1, 05, -1 },
                { -1, -1, -1, 05, -1, 05 },
                { -1, -1, -1, -1, 05, -1 },
            };

            public void Dijikstra(int start)
            {
                bool[] visited = new bool[6];
                int[] parent = new int[6];
                int[] distance = new int[6]; // 정점간의 거리, 가중치를 기록
                Array.Fill(distance, Int32.MaxValue); // 배열에 기록할수있는 가장큰값을 채워줌.
                                                      // 그냥 이런기능도 있다정도만 알아두자.

                distance[start] = 0; //BFS의 경우 가중치가 없기에 그냥 queue에 넣은 예약순이 최단거리가 됬지만,
                                     //dijikstra는 거리가 있으니 따로 계산해야된다.
                parent[start] = start;

                while(true)
                {
                    // Best후보찾기 (가장 가까이 있는)
                    // 가장 유력한 후보의 거리와 번호를 저장한다.
                    int closest = Int32.MaxValue;
                    int now = -1;
                    for (int i=0;i<6;i++)
                    {   //일단 후보는 맞는지부터 판별
                        if (visited[i]) //방문한 점인지?
                            continue;
                        if (distance[i] == Int32.MaxValue || distance[i] >= closest) //발견(예약)된 적은 없는지?
                                                                                     //또는 기존후보보다 멀리 있진 않은지?
                            continue;
                        closest = distance[i];
                        now = i;
                    }
                    if (now == -1) //위 후보판별을 뚫은 후보가 없다 = 다음후보가 없다.
                        break;
                    visited[now] = true; // 제일 좋은 후보를 찾았으니 방문한다.

                    //방문한 best정점과 인접한 정점들을 조사해서,
                    //상황에 따라 발견한 최단거리를 갱신한다.
                    for (int next = 0; next < 6; next++)
                    {
                        if (adj[now, next] == -1) //연결 안됬으면 스킵
                            continue;
                        if (visited[next]) //이미 방문했던 점은 스킵
                            continue;

                        // 위 둘을 거치면, 새로 조사된 정점일테니. 이 점까지의 최단거리를 계산하자.
                        int nextDist = distance[now] + adj[now, next];
                        // 만약 기존의 최단거리가 새로 발견한 최단거리보다 크면, 새걸로 갱신한다.
                        if (nextDist < distance[next])
                        {
                            distance[next] = nextDist;
                            parent[next] = now; 
                        }
                    }
                }
            }

        }



        static void Main(string[] args)
        {
            Graph graph = new Graph();
            graph.Dijikstra(0);
        }
    }
}
