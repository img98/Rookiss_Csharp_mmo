using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DataStructure_Algorithm
{
    class Pos // 오른손법칙으로 계산한 경로를 담는 클래스. 사실 여기다 계산로직을 담아도 됬다.
    {
        public int Y;
        public int X;
        public Pos(int y, int x) { Y = y; X = x; }
    }

    class Player
    {
        public int PosY { get; private set; }
        public int PosX { get; private set; }
        // 다른데서도 get으로 좌표는 얻을수있지만, 좌표수정은 player만 할수있게 private

        Random _random = new Random();

        Board _board; // 길찾기AI를 위해선 일단 길을 알아야하니

        enum Dir
        {
            Up = 0,
            Left = 1,
            Down = 2,
            Right = 3
        }
        int _dir = (int)Dir.Up; // int로 계산하는게 쉬워서 enum이 아니라 int형을 하나 새로 만듬
        List<Pos> _points = new List<Pos>(); //경로를 담는 리스트

        public void Initialize(int posY, int posX, Board board)
        {
            PosY = posY;
            PosX = posX;

            _board = board;

            Astar();

        }
        struct PQNode : IComparable<PQNode> // F,G 등 계산을 위한 클래스
        {
            public int F;
            public int G; // H는 F,G있으면 계산가능하니 패스
            public int X;
            public int Y;

            public int CompareTo(PQNode other)
            {
                if (F == other.F)
                    return 0;
                return F < other.F ? 1 : -1;
            }
        }
        void Astar()
        {
            // 미로찾기 알고리즘은 크게 두가지로 나뉜다.1)시작과 끝위치를 안다. 2)시작위치만 안다.
            // 사실 Dijikstra는 (2)에 해당하기에, 미로의 모든 구석까지 가봐야한다. = 낭비
            // (1)의 대표는 A*(에이스타) 알고리즘이 있다.
            // 출구에 가까워질수록 가산점을 부여. 
            // Astar에 우선순위 큐가 쓰이는데, 그 이유는 얘가 '하나의 최고 케이스 찾는데 좋은' 알고리즘이기 때문이다.

            int[] deltaY = new int[] { -1, 0, 1, 0 }; // 복붙
            int[] deltaX = new int[] { 0, -1, 0, 1 };
            int[] cost = new int[] { 1, 1, 1, 1 }; // 한칸을 이동하는데 드는 비용.
            // 점수 매기기 F = G + H
            // F = 최종 점수(작을수록 좋음, 경로에 따라 달라짐)
            // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용(작을수록 좋음, 경로에 따라 달라짐)
            // H = 해당 좌표에서 목적지까지 얼마나 가까운지 (작을수록 좋음, 고정)

            // (y,x) 이미 방문했는지 여부 (방문 = closed 상태) 그냥 astar에서 이 용어를 사용함
            bool[,] closed = new bool[_board.Size, _board.Size]; //클로스 리스트 라고부름.

            // (y,x) 가는 길을 한번이라도 발견했는지
            // 발견X => MaxValue, 발견O => F=G+H
            int[,] open = new int[_board.Size, _board.Size]; //오픈 리스트
            for (int y = 0; y < _board.Size; y++)
                for (int x = 0; x < _board.Size; x++)
                    open[y, x] = Int32.MaxValue;

            Pos[,] parent = new Pos[_board.Size, _board.Size];

            // 오픈리스트에 있는 정보들 중, 최고 후보를 빠르게 뽑아오기 위한 도구
            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            // 시작점 발견 (예약진행)
            open[PosY, PosX] = Math.Abs(_board.DestY - PosY) + Math.Abs(_board.DestX - PosX); // 얘가 H. 목적지까지 가야되는 칸수로 정의하자.
            pq.Push(new PQNode() { F = Math.Abs(_board.DestY - PosY) + Math.Abs(_board.DestX - PosX), G = 0, Y = PosY, X = PosX });
            parent[PosY, PosX] = new Pos(PosY, PosX);

            while(pq.Count>0) 
            {
                //제일 좋은 후보를 찾는다.
                PQNode node = pq.Pop();
                // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
                if (closed[node.Y, node.X])
                    continue;

                // 방문한다.
                closed[node.Y, node.X] = true;
                //목적지에 방문(도착)한거면 종료
                if (node.Y == _board.DestY && node.X == _board.DestX)
                    break;

                // 방문(이동) 후, 상하좌우 이동할 수 있는 좌표인지 확인해서 예약(open)한다.
                for(int i=0;i<deltaY.Length;i++)
                {
                    int nextY = node.Y + deltaY[i];
                    int nextX = node.X + deltaX[i];

                    //유효 범위를 벗어났는가? (로직 자체는 BFS,Dijikstra와 동일)
                    if (nextX < 0 || nextX >= _board.Size || nextY < 0 || nextY >= _board.Size) 
                        continue;
                    //벽으로 막혀있는가?
                    if (_board.Tile[nextY, nextX] == Board.TileType.Wall) 
                        continue;
                    //이미 방문한 곳인가?
                    if (closed[nextY, nextX] == true)
                        continue;

                    //비용 계산
                    int g = node.G + cost[i]; // 이동하는데 드는 비용을 계속 더해간다.
                    int h = Math.Abs(_board.DestY - nextY) + Math.Abs(_board.DestX - nextX);
                    //다른 경로에서 더 빠른 길을 찾았으면 스킵
                    if (open[nextY, nextX] < g + h)
                        continue;

                    //위를 전부 거쳤다면 가장 좋은 루트 라는 뜻 => 이제 예약하자.
                    open[nextY, nextX] = g + h;
                    pq.Push(new PQNode() { F = g + h, G = g, Y = nextY, X = nextX });
                    parent[nextY, nextX] = new Pos(node.Y, node.X);
                }
            }
            CalcPathFromParent(parent);
        }
        
        void CalcPathFromParent(Pos[,] parent) // 경로를 담는 로직이다. 계속 나와서 그냥 함수로 만들었다.
        {
            int y = _board.DestY;
            int x = _board.DestX;
            while (parent[y, x].Y != y || parent[y, x].X != x)
            {
                _points.Add(new Pos(y, x)); // _points는 player의 이동경로를 담았던 배열이다.
                Pos pos = parent[y, x];
                y = pos.Y;
                x = pos.X;
            }
            _points.Add(new Pos(y, x));
            _points.Reverse();
        }

        void BFS() // 맵만들기로 만든 미로를 BFS로 돌파해보자.
        {
            int[] deltaY = new int[] { -1, 0, 1, 0 }; // 우수법에서 보는방향에 따른 이동좌표를 설정한 형태로 만들어주자.
            int[] deltaX = new int[] { 0 , -1, 0, 1};

            bool[,] found = new bool[_board.Size, _board.Size];
            Pos[,] parent = new Pos[_board.Size, _board.Size];


            Queue<Pos> q = new Queue<Pos>();
            q.Enqueue(new Pos(PosY, PosX));
            found[PosY, PosX] = true;
            parent[PosY, PosX] = new Pos(PosY, PosX); // 처음애는 사실상 부모가 없음. 그냥 자기를 부모라고 설정하자.

            while (q.Count>0)
            {
                Pos pos = q.Dequeue();
                int nowY = pos.Y;
                int nowX = pos.X;

                for (int i = 0; i < 4; i++)
                {
                    int nextY = nowY + deltaY[i];
                    int nextX = nowX + deltaX[i]; // 가까운거리의 정점 확인

                    if (nextX < 0 || nextX >= _board.Size || nextY < 0 || nextY >= _board.Size) // 배열에 접근하는 코드는 항상 범위를 벗어나지 않는지 주의해야함.
                        continue;
                    if (_board.Tile[nextY, nextX] == Board.TileType.Wall) // 이웃점들이 벽이면 못가니까(연결되지 않았으니까)
                        continue;
                    if (found[nextY, nextX] == true )
                        continue;

                    q.Enqueue(new Pos(nextY, nextX));
                    found[nextY, nextX] = true;
                    parent[nextY, nextX] = new Pos(nowY, nowX);
                }
            }

            int y = _board.DestY;
            int x = _board.DestX;
            while (parent[y, x].Y != y || parent[y, x].X != x)  // 계속 parent를 타고올라가 시작점을 찾아야한다. 우린 위에서 시작점의 부모는 시작점이라고 설정했었다.
            {
                _points.Add(new Pos(y, x)); // _points는 player의 이동경로를 담았던 배열이다.
                Pos pos = parent[y, x];
                y = pos.Y;
                x = pos.X;
            }
            _points.Add(new Pos(y, x)); // 시초가 되는점은 while문에의해 돌지않았으니 _points에 담기지 않았을것이다. 손수 넣어주자
            _points.Reverse(); //역으로 부모를찾는 경로를 담았으니 reverse해주면 부모에서 목적지로 가는것처럼 보이게할수있다.
        }

        void RightHand() // 맵만들기에서 했던 우수법 그냥 이동시킨것.
        {
            int[] frontY = new int[] { -1, 0, 1, 0 }; // 바라보는 방향으로 전진할때의 좌표변화
            int[] frontX = new int[] { 0, -1, 0, 1 }; // 각각 up,left,down,right 를 볼때 앞으로간다면 y,x 가 어떻게 변할지 넣은것.

            int[] rightY = new int[] { 0, -1, 0, 1 };
            int[] rightX = new int[] { 1, 0, -1, 0 };   //** 배열로 좌표를 표현한다는게 좀 신박하네

            _points.Add(new Pos(PosY, PosX)); // 초기위치
            // initialize단계에서 '오른손법칙'을 통해 탈출로를 모두 계산하고 update로 그길을 보이기만 하는 법을 구현해보자
            while (PosY != _board.DestY || PosX != _board.DestX) // 답을 찾을때까지 loop
            {
                if (_board.Tile[PosY + rightY[_dir], PosX + rightX[_dir]] == Board.TileType.Empty) // 1단계 : 현재 바라보는 방향 기준으로 오른쪽으로 갈 수 있는지 확인
                {
                    // 오른쪽방향 으로 90도  회전
                    _dir = (_dir - 1 + 4) % 4; // 각 방향에 1을 뺀것이 오른쪽 보게하는것, 4더한이유는 up=0에서 -1이 안되므로 다시 맨위(3)으로 올라감을 의미, %4는 4미만 수가 나오게 보장하는 기능
                    // 한칸 전진시킨다.
                    PosY += frontY[_dir];
                    PosX += frontX[_dir];
                    _points.Add(new Pos(PosY, PosX)); // 전진했다는 것을 기록
                }

                else if (_board.Tile[PosY + frontY[_dir], PosX + frontX[_dir]] == Board.TileType.Empty) // 2단계 : 오른쪽으로 갈 수 없다면, 지금 보는방향으로는 갈 수 있는지 확인
                {
                    // 보는방향으로 1보 전진
                    PosY += frontY[_dir];
                    PosX += frontX[_dir];
                    _points.Add(new Pos(PosY, PosX)); // 전진했다는 것을 기록
                }

                else // 3단계 : 오른&앞 불가능하다면, 왼쪽으로 90도 회전. 전진은 안함
                {
                    _dir = (_dir + 1 + 4) % 4; //오른쪽 보게하는걸 응용
                }
            }
        }

        const int MOVE_TICK = 100; // 100ms마다 움직이게 기준을 잡아보자. (0.1초)
        int _sumTick = 0;
        int _lastIndex = 0;
        public void Update(int deltaTick) //deltaTick = 이전틱과 현재틱 사이의 경과시간. 보통 업데이트의 경우 시간을 넘겨주고 쓸거면 쓰라함
                                          //deltatick을 주는 이유 : 업데이트가 1/30초마다 되면 너무 빠르니까
        {
            _sumTick += deltaTick;
            if (_sumTick >= MOVE_TICK) // 단순하다. deltaTick이 우리가 정한 MOVETICK까지 쌓인다면 코드실행
            {
                if (_lastIndex >= _points.Count) // points.Count 의 count는 list에 있는 메소드
                    return;
                _sumTick = 0;

                PosY = _points[_lastIndex].Y;
                PosX = _points[_lastIndex].X;
                _lastIndex++;
            }
        }
    }
}
