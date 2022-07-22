using System;
using System.Collections.Generic;
using System.Text;

namespace TRPG2
{
    //게임 진행 관련
    public enum GameMode
    {
        None,
        Lobby,
        Town,
        Field
    }

    class Game
    {
        private GameMode mode= GameMode.Lobby;
        private Player player = null;
        private Monster monster = null;
        private Random rand = new Random(); 

        public void Process()
        {
            switch(mode)
            {
                case GameMode.Lobby:
                    ProcessLobby();
                    break;
                case GameMode.Town:
                    ProcessTown();
                    break;
                case GameMode.Field:
                    ProcessField();
                    break;
            }
        }

        private void ProcessLobby()
        {
            Console.WriteLine("직업을 선택하세요");
            Console.WriteLine("[1] 기사");
            Console.WriteLine("[2] 궁수");
            Console.WriteLine("[3] 법사");

            string input = Console.ReadLine();
            switch(input)
            {
                case "1":
                    player = new Knight();
                    mode = GameMode.Town;
                    break;
                case "2":
                    player = new Archer();
                    mode = GameMode.Town;
                    break;
                case "3":
                    player = new Mage();
                    mode = GameMode.Town;
                    break;
            }
        }

        private void ProcessTown()
        {
            Console.WriteLine("마을에 입장했습니다.");
            Console.WriteLine("[1] 필드로 나가기");
            Console.WriteLine("[2] 로비로 돌아가기");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    mode = GameMode.Field;
                    break;
                case "2":
                    mode = GameMode.Lobby;
                    break;
            }
        }

        private void ProcessField()
        {
            CreateRandomMonster();

            Console.WriteLine("필드에 입장했습니다.");
            Console.WriteLine("[1] 싸우기");
            Console.WriteLine("[2] 일정확률로 마을로 도망가기");


            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    ProcessFight();
                    break;

                case "2":
                    TryEscape();
                    break;
            }
        }

        private void CreateRandomMonster()
        {
            int randValue = rand.Next(0, 3);
            switch (randValue)
            {
                case 0:
                    monster = new Slime();
                    Console.WriteLine("슬라임이 생성되었습니다.");
                    break;

                case 1:
                    monster = new Orc();
                    Console.WriteLine("오크가 생성되었습니다.");
                    break;

                case 2:
                    monster = new Skeleton();
                    Console.WriteLine("스켈레톤이 생성되었습니다.");
                    break;
            }
        }

        private void ProcessFight()
        {
            while(true)
            {
                //여기서 attack과 hp에 따른 생존여부 로직을 짜면되지만 귀찮으니 생략
                Console.WriteLine("You Win!");
                Console.WriteLine("필드에 남아 더 싸웁니다.");
                mode = GameMode.Field; //귀찮아서 안했지만 이긴후에 마을로 가는 로직을 만들자.
                break;
            }
        }

        private void TryEscape()
        {
            int randValue = rand.Next(0, 101);
            if(randValue<33)
            {
                Console.WriteLine("도주에 실패했습니다.");
                ProcessFight();
            }
            else
            {
                mode = GameMode.Town;
            }
        }
    }
}
