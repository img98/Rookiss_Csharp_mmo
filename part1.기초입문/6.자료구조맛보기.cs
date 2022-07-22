using System;
using System.Collections.Generic;

namespace cs
{
    class Player { }
    class Monster 
    {
        public int id;   
        public Monster (int id)
        {
            this.id = id;
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            //기본적인 자료구조에 대해 다룰것 //다음파트에서 알고리즘 심화

            // [1. 배열]
            int a; // 변수는 일종의 바구니라 했다.

            int[] scores = new int[5] { 10, 20, 30, 40, 50 }; // 배열은 연속된 바구니를 말한다.
            // 배열을 초기화하는 방법을 좀 알아두자.
            // int[] scores = { 10, 20, 30, 40, 50 };  이렇게 써도 똑같긴하다. 

            for (int i = 0; i < scores.Length; i++)
            {
                Console.WriteLine(scores[i]);
            }

            //배열 개념자체는 쉽다. [ ]를 만드는것이 중요
            //그런데 위 for문 돌리는게 좀 귀찮음(c++ 초기스타일). 우아하게 사용해보자

            foreach (int score in scores) //scores에 있는 int들을 각각 뽑아내서 출력하는것
            {
                Console.WriteLine(score);
            }

            // [다차원배열] ex) list[,] ; 좌표 표시에 편하다. //c언어랑 다르게 list[][]가 아니네
            int[,] arr = new int[2, 3] { { 1, 2, 3 }, { 1, 2, 3 } };

            //보기쉽게 표현하려면
            int[,] tiles = new int[4, 3]{
                { 1, 2, 3 },
                { 1, 2, 3 },
                { 1, 2, 3 },
                { 1, 2, 3 }
            }; //이런식으로 바둑판배치가 좋다.
            //이걸 렌더링하는 방법 //수는 신경안쓰고 그냥 모양만 *로 표현하기 가능
            for (int y = 0; y < tiles.GetLength(1); y++)  //1은 [,] 이 괄호에서 0,1 즉 y축을 의미한다.
            {
                for (int x = 0; x < tiles.GetLength(0); x++) 
                {
                    Console.Write("*");
                }
                Console.WriteLine();
            }

            //강사가 말한 잘안쓰는 다차원배열 : 각층마다 배열크기가 다른 경우 //아 이게 학교에서 배운거다.
            int[][] b = new int[3][];
            b[0] = new int[3];
            b[1] = new int[6];
            b[2] = new int[3];
            //배열내에 배열을 만들었다.

            // [List]
            // 배열의 경우 처음 초기화할때 설정한 크기 이상으로 변경하기 어렵다.
            // 그렇기에 List를 사용하자 <- 동적 배열 //새로운 크기의 배열을 만들어 거기다 복사하는 로직임

            List<int> list = new List<int>(); // 그냥쓰면 에러뜨는데, 아래 빨갛게 잠재적 수정사항을 클릭하면 위에 새로운코드의 출현과 동시에 에러해결
                                              // 라이브러리 선언을 자동으로 추가해주는건듯?
            list.Add(1);    // list도 배열이긴 하나, list[0] =1; 이런식으로 추가해선 안됨(클래스라 그런가?). 해당하는 Add메소드를 사용해라
            for (int i = 0; i < 5; i++)
            {
                list.Add(i);
            }
            for(int i=0;i<5;i++) // list.Length는 쓸수없더라.
            {
                Console.WriteLine(list[i]); //출력은되네 add는 안되더니
            }
            foreach (int num in list) //배열이니 이걸로 
                Console.WriteLine(num);
            // List는 완성되어있는 클래스이니 메소드들이 잘 구비되어 있다. ex. Insert, LastIndexOf, Remove

            // [Dictionary] 
            // List도 편하지만 search 등의 동작에 시간복잡도 효율이 안좋다.
            // Key -> Value로 빠르게 작동하는 Dictionary가 효율적이다.
            Dictionary<int, Monster> dic = new Dictionary<int, Monster>();

            dic.Add(1, new Monster(1)); // 이런식으로 1 id를 가진 몬스터를 만들수있다.

            dic[5] = new Monster(5); //이렇게 배열식으로도 추가할 수 있다.

            for (int i=0;i<10000;i++)
            {
                dic.Add(i, new Monster(i));// 단 이코드의 경우 위에서 이미 id 1을 가진 value를 생성했기에,다시한번 id 1을 만드는 과정에서 에러가 생김
            }
            Monster mon = dic[5000];

            // dictionary는 해쉬테이블 기법을 이용하기에 효율적인 것임. 
            // Hashtable
            // 아주큰박스 [ 0 1 2 . . . 99999 ] 1만개의 컨텐츠. 이중 하나를 찾기란 힘듬
            // 박스 여러개로 쪼갠다 [1~10] [11~20] [ ] [ ] [ ] 1천개
            // 필요한 데이터가 있는 박스로 가면 10개만 뒤지면 된다. ->효율적
            // 단 여러 박스를 준비해야 하기에 메모리 손해가 있다.
        }

        static int GetHighestScore(int[] scores)
        {
            int Highest = -1;

            for(int i=0; i<scores.Length;i++) //이게 학교에서 배운것. foreach를 사용해도된다.
            {
                if (Highest < scores[i])
                {
                    Highest = scores[i];
                }
            }
            return Highest;
        }
        static int GetAverageScore(int[] scores)
        {
            if (scores.Length == 0)
                return 0;

            int sum = 0;
            foreach (int score in scores)
            {
                sum += score;
            }
            return sum/scores.Length;
        }
    }



}
