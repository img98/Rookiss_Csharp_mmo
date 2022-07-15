using System;

namespace cs
{
    class 코드의흐름제어
    {
        static void Main(string[] args)
        {
            //ch1. if와 else
            int hp = 100;
            bool isDead = (hp <= 0);
            if (isDead)
            {
                Console.WriteLine("You are dead!");
            }
            else
            {
                Console.WriteLine("You are alive!");
            }
            //else if도 있다. if; else if; else if 이렇게 3개쭉 사용도가능

            //ch2.switch
            //위 else if같은경우 반복부분이 많이 나올수 있다. 그럴때 switch를 사용한다.
            int choice = 0;

            switch (choice) //if보다 사용폭은 좁지만 간단하다.
            {
                case 0:
                    Console.WriteLine("0입니다.");
                    break; //break문을 사용해 case별 종료를 알려야한다.
                case 1:
                    Console.WriteLine("1입니다.");
                    break;
                case 2:
                    Console.WriteLine("2입니다.");
                    break;
                default:
                    Console.WriteLine("위 세가지 경우가 모두 아니면 나오는 디폴트값입니다.");
                       break;
            }

            //삼항연산자?
            int number = 25;
            bool isPair; //number가 짝수인지 판단하는 로직이다.
            if ((number % 2) == 0)
                isPair = true;
            else
                isPair = false;

            //위처럼 if문을쓰면 길어지니 삼항연산자로 짧게 만들수있다.

            bool isPair = ((number % 2) == 0 ? true : false); //그러나 가독성이 구리다고 안쓰는사람도 있다. 읽는법만 알아두자
            //bool isPair = (조건 ? 맞을때 : 틀릴때)

            //ch3. 가위바위보 게임 만들기
            //0=가위 1=바위 2=보

            Random rand = new Random(); //나중 클래스때 배울테니 그냥 이해해라
            int aiChoice =  rand.Next(0, 3);

            int choice = Convert.ToInt32(Console.ReadLine());

            //이밑은 그냥 if else로 돌리거나 switch로 돌리는 당연한 얘기 (생략)


            //ch4. 상수와 열거형
            //0=가위 1=바위 2=보 로 표기하는게 보기 안좋다.-하드코딩

            const int SCISSORS = 0; //const를 사용해 후에 변경되지 않는 '상수화'
            const int ROCK = 1;
            /*enum Choice
        {
            Rock = 1,
            Paper = 2,
            Scissors = 0
        }*/  //Main영역 밖에다가 열거형을 만들수있음. 이경우 그냥 Rock이라고 변수를 사용해도 1이라고 인식됨


            //ch5. while문 (반복문)
            int whileCount = 5;
            while (whileCount >0)
            {
                whileCount--;
                //while의 조건이 만족한다면 무한반복
            }

            do
            {
                whileCount++;
                //일단 실행하고 while조건을 점검. 또한 만족한다면 do다시 실행
            } while (whileCount<5);

            //ch6. for문
            //while은 반복문 내부에 조건의 변화를 신경써줘야함. for문은 위에 머리에다 다 정할수있다.
            for (int i = 0; i < 5; i++) //for(초기화식; 조건식; 반복식)
            {
                Console.WriteLine("work Well");
            }

            //ch7. break, continue
            //반복문의 조건을 만족하기 전에 탈출하고싶다. 할때 사용

            for(int i=0; i<5; i++)
            {
                Console.WriteLine("this time is {0} we need to go to 5", i + 1);
                if (i == 3)
                    break;
            }

            //break는 위와 같이 아예 종료. 하지만 continue는 여기서 루프를 끝낸걸로 취급하고 다음 루프로 가는것
            for (int i = 0; i < 5; i++)
            {
                if (i == 1)
                    continue;
                Console.WriteLine("this time is {0} we need to go to 5", i + 1); //사실 숫자 이렇게 넣지말고
                Console.WriteLine($"this time is {i+1} we need to go to 5") //이렇게 쓰는게 최신방법
            }

           
        }
        //ch8. 함수, method
        //내가 아는 그 함수. 매번 코드가 필요할때마다 칠수 없으니,
        //하나의 함수를 정의하여 함수이름을 호출하는 것으로 코드를 생략

        //메소드 함수 조금 차이가 있지만 그냥 혼용해서 써도 된다.

        //한정자 반환형식 이름(매개변수목록) 형태임 & 클래스 영역에 넣어야함.
        
        static int Add(int a, int b)
        {
            int c = a + b;
            return c; // 사실 return a+b; 로 해도된다.
        }
        
        //사용시 int result = Program.Add(2,3) 이렇게쓰면됨

        //사실 저기  int a b가 진짜 ab는 아니고 그 값인데, 이걸 다루는것이 복사와 참조다.
        //복사(짝퉁), 참조(진퉁) -후에 다시배운다.

        //ch9. ref와 out
        //앞서 복사가 아닌 실제 값을 변화시키고 싶다면 참조를 사용하라고 했다.
        static void AddOne(ref int number) //ref를 통해 갖고오는것이 참조
        {
            number++;
        }
            static int AddOneNoRef(int number)
        {
            return number++;
        }
        //위 두 함수가 같은 효과이다. 하지만 아래쪽이 좀더 효과적이라고 한다.
        
        //ref가 필요한 경우? ex) a와 b를 바꾸는 swap함수 ->함수호출후에도 a와b를 사용하여 작업할테니...

        //반환하고싶은 값이 수, 문자 등 여러 형태면 어떻게 해야될까? static int AddOneNoRef마냥 안될텐데?
        //1. ref를 사용해 실제값에 바꿔주면됨. 그런데 ref를 사용하지 않아도 오류를 모르다 보니 조금 위험함.
        //2. out을 사용
        static void Divide(int a, int b, out int result1, out int result2)
        {
            result1 = a / b;
            result2 = a % b;
        }
        //그냥 반환할 새로우 변수를 선언한다고 생각하면될듯? ->얘네들은 ref와 같이 진퉁이니 그점 유의

        //ch10. 오버로딩
        //함수이름의 재사용
        static float Add(float a, int b)//똑같은 Add이름의 함수를 쓰고싶다. //사실 int Add로 써도되는데 인자가 float라 바꾼것
            //매개변수 타입과 인자가 완전히 같지만 않으면 된다.
        {
            return a + b;
        }
        static int Add(int a, int b, int c)
        {
            return a + b;
        }
        //선택적 매개변수? 알면쓰고 모르면 디폴트값 /사실 당장 알필욘없다.
        static int AddChoice(int a, int b, int c=0)
        {
            //int c=0 이란 함수호출시 3번째 인자를 써주면 그걸 c로쓰고 아니며 0으로 취급하겠다는뜻
            //그렇기에 이경우 컴퓨터가 static int Add(int a, int b) 와 헷갈릴수가있다. 에러발생
            return a + b + c;
        }
    }
}
