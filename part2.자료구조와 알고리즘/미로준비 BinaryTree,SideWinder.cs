using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure_Algorithm
{
    class Board //말그대로 보드판. 현재 상황(데이터)을 담는 판
    {
        public TileType[,] _tile;// 2d좌표(인덱스)를 편하기 쓰기위해 2차원 배열을쓰자
        public int _size;

        public enum TileType // 빈공간과 벽을 색깔로 구분해보자. //0,1,로 구분해도되긴하는데 보기 안좋으니까..
        {
            Empty,
            Wall,
        }

        public void Initialize(int size)
        {
            if (size % 2 == 0)  // 미로는 테두리가 벽이어야 하니까 size가 짝수면 안된다. 짝수면 길뚫기 로직이 안먹힘
                return;

            _tile = new TileType[size, size];
            _size = size;

            // Mazes for programmers 둘중 한 생성법 골라쓰자. 길을 뚫는 방법이 다르다.
            // GenerateByBinaryTree(_size); // 일단 격자꼴로 empty만들고 그점들을 기준으로 우로갈지 하로갈지
             GenerateBySideWinder(_size); // 격자로 뚫고 우하이동은 같으나, 그후 다음점이 아니라 이동한위치에서 다시 우하.
        }

        public void GenerateByBinaryTree(int size)
        {
            // 1. 길을 한칸 간격으로 전부 막아버리기
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++) 
                {
                    if (x % 2 == 0 || y % 2 == 0) // 
                        _tile[y, x] = TileType.Wall; 
                    else
                        _tile[y, x] = TileType.Empty;
                }
            }
            // 2.랜덤으로 우측 혹은 아래로 길 뚫기 (물론 위에서 남긴 초록점 기준으로 우,하)
            // 50퍼 확률로 길을 뚫어보자
            Random rand = new Random();
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    if (x % 2 == 0 || y % 2 == 0) // 1에서 뚫은 점부분은 continue
                        continue;


                    if (y == _size - 2 && x == _size - 2)
                        continue; //맨 오른쪽 하단일때 벽뚫지 않게 그냥 끝내기

                    // 외곽벽을 건드리지 않도록 해보자.
                    if (y == _size - 2) // 타일이 미로 맨아래벽 바로 위라면
                    {
                        _tile[y, x + 1] = TileType.Empty; // 무조건 우측으로 가도록 설정(아래로 가면 벽뚫리니까)
                        continue; 
                    }
                    if (x == _size - 2)
                    {
                        _tile[y + 1, x] = TileType.Empty; // 무조건 하단으로 가도록
                        continue;
                    }
                    if (rand.Next(0, 2) == 0)
                    {
                        _tile[y, x + 1] = TileType.Empty; // x+1 ?=  우측을 뚫는다
                    }
                    else // random이니 0아니면 1이 나올것
                        _tile[y+1, x] = TileType.Empty; // 하단뚫기

                }
            }
        }
        public void GenerateBySideWinder(int size)
        {
            // 1. 길을 한칸 간격으로 전부 막아버리기
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    if (x % 2 == 0 || y % 2 == 0) // 
                        _tile[y, x] = TileType.Wall;
                    else
                        _tile[y, x] = TileType.Empty;
                }
            }
            // 2. 우측으로 뚫은 애들중에서 랜덤으로 하단 뚫기
            Random rand = new Random();
            for (int y = 0; y < _size; y++)
            {
                int count = 1;
                for (int x = 0; x < _size; x++)
                {
                    if (x % 2 == 0 || y % 2 == 0)
                        continue;

                    // 외곽벽을 건드리지 않게
                    if (y == _size - 2) // 타일이 미로 맨아래벽 바로 위라면
                    {
                        _tile[y, x + 1] = TileType.Empty; // 무조건 우측으로 가도록 설정(아래로 가면 벽뚫리니까)
                        continue;
                    }
                    if (x == _size - 2)
                    {
                        _tile[y + 1, x] = TileType.Empty; // 무조건 하단으로 가도록
                        continue;
                    }

                    if (rand.Next(0, 2) == 0) // 우측으로 가는경우는 똑같이 뚫으면됨
                    {
                        _tile[y, x + 1] = TileType.Empty; // x+1 >>  우측을 뚫는다
                        count++; // 우측으로 몇개나 뚫었는지 기억
                    }
                    else // 여기가 Binary와의 차이. 무조건 아래가 아니라 지금까지 뚫은점 중에서 하나에 하단
                    {
                        int randomIndex = rand.Next(0, count);
                        _tile[y + 1, x - randomIndex * 2] = TileType.Empty; // 하단뚫기
                        count = 1;
                    }

                }
            }
        }

        const char CIRCLE = '\u25cf'; //원모양
        public void Render()
        {
            ConsoleColor prevColor = Console.ForegroundColor;

            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    
                    Console.ForegroundColor = GetTileColor(_tile[y, x]);
                    Console.Write(CIRCLE);
                }
                Console.WriteLine();
            }

            Console.ForegroundColor = prevColor; // ForegroundColor의 값을 이상하게 만들지않도록, 일 다보고 원래대로 둔다는 개념.
        }
        ConsoleColor GetTileColor(TileType type) // 타일 타입에 따라 다른색깔 칠해주기
        {
            switch(type)
            {
                case TileType.Empty:
                    return ConsoleColor.Green;
                case TileType.Wall:
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.Green; //사실 여까지 올 수가 없긴한데, 일단 그린으로두자
            }
        }
    }
}
