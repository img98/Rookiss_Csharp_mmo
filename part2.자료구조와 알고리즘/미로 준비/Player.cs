using System;
using System.Collections.Generic;
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

            int[] frontY = new int[] { -1, 0, 1, 0 }; // 바라보는 방향으로 전진할때의 좌표변화
            int[] frontX = new int[] { 0, -1, 0, 1 }; // 각각 up,left,down,right 를 볼때 앞으로간다면 y,x 가 어떻게 변할지 넣은것.

            int[] rightY = new int[] { 0, -1, 0, 1 };
            int[] rightX = new int[] { 1, 0, -1, 0 };   //** 배열로 좌표를 표현한다는게 좀 신박하네

            _points.Add(new Pos(PosY, PosX)); // 초기위치
            // initialize단계에서 '오른손법칙'을 통해 탈출로를 모두 계산하고 update로 그길을 보이기만 하는 법을 구현해보자
            while (PosY != board.DestY || PosX != board.DestX) // 답을 찾을때까지 loop
            {
                if(_board.Tile[PosY + rightY[_dir], PosX + rightX[_dir]] == Board.TileType.Empty) // 1단계 : 현재 바라보는 방향 기준으로 오른쪽으로 갈 수 있는지 확인
                {
                    // 오른쪽방향 으로 90도  회전
                    _dir = (_dir - 1 + 4) % 4; // 각 방향에 1을 뺀것이 오른쪽 보게하는것, 4더한이유는 up=0에서 -1이 안되므로 다시 맨위(3)으로 올라감을 의미, %4는 4미만 수가 나오게 보장하는 기능
                    // 한칸 전진시킨다.
                    PosY += frontY[_dir];
                    PosX += frontX[_dir];
                    _points.Add(new Pos(PosY, PosX)); // 전진했다는 것을 기록
                }   
                
                else if(_board.Tile[PosY + frontY[_dir], PosX + frontX[_dir]] == Board.TileType.Empty) // 2단계 : 오른쪽으로 갈 수 없다면, 지금 보는방향으로는 갈 수 있는지 확인
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
