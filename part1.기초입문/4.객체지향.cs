using System;

namespace cs
{

    //그동안 하던건 절차(procedure)지향 =함수기반 =함수자체가 코드 순서에 종속적임

    //객체 지향(OOP, Object Oriented Programming)
    //객체는 속성과 기능으로 분류됨

    //Kinht 속성 : hp, attack, pos (데이터들)
    //      기능 : Move, Attack, Die (기능)

    class Knight //객체를 묘사할때 쓰는 문법이 class
        //붕어빵 틀에 해당하는 '설계도'임
    {
        public int hp; //public을 붙이지 않으면 외부에서 해당 변수에 접근 불가능
        public int attack;

        static public int id; // 후에 나올 static의 정체 관련 //오로지 하나만 존재

        public void Move()
        {
            Console.WriteLine("Knight Move");
        }
        public void Attack()
        {
            Console.WriteLine("Knight Attack");
        }

        public Knight Clone() //이게 나중에 후술될 deep copy의 코드
            // 솔직히 걍 메소드 하나 만든거임
        {
            Knight knight = new Knight();
            knight.hp = hp;
            knight.attack = attack;
            return knight;
        }

        public Knight() //이게 생성자. 생성시 해당 객체의 속성을 할당하는것인듯.
            //물론 main처럼 후에 수정도 가능할것으로 추측됨
        {
            hp = 100;
            attack = 10;
        }
        public Knight(int hp, int attack = 10) //전시간에 말한 오버로딩 개념으로 여러 조건에서의 생성자를 만들수있다.
        {
            this.hp = hp;   //매개변수 hp가 아닌 생성자의 hp라는 것을 명시하기위해 this를사용
            this.attack = attack;
        }

        //위처럼 선택적 매개변수를 쓰는게아니라 그냥 default값을 가져오는 방법도 있다.
        public Knight(int hp) : this()  //:this() 를 통해 attack을 명시하지 않아도 attack=10 이 가능하다.
        {   //그냥 이런게 있다정도만 알아둬라. 속성이 겁나 많아지면 사용하는 방법임.
            this.hp = hp;
        }

    }
    struct Mage // class가 아니라 struct로 만들면 어떻게될까?
    {   // class = Ref(참조)를 사용, struct = 복사를 사용하는 것
        public int hp; 
        public int attack;
    }

    class Program
    {
        static void KillMage(Mage mage) //참조와 복사의 차이를 알기위해 Kill 메소드를 만들어보자.
        {
            mage.hp = 0;
        }
        static void KillKnight(Knight knight)
        {
            knight.hp = 0;
        }

        static void Main(string[] args)
        {
            Knight knight = new Knight(); //new는 새롭게 Kinght틀을 사용하겠다는 뜻
            knight.hp = 100;
            knight.attack = 10;
            knight.Move();
            knight.Attack();

            Mage mage = new Mage(); //struct의 경우 new가 필요없긴해..
            mage.hp = 100;
            mage.attack = 50;

            KillMage(mage);
            KillKnight(knight);

            Console.WriteLine($"mage hp is {mage.hp}, knight hp is {knight.hp}");
            //위 출력결과 mage의 hp는 그대로 100인것을 볼수있다.
            //복사(struct)를 사용하였기에 진퉁 mage의 데이터는 변하지 않은것.

            Mage mage2 = mage;  //새로운 mage2와 knight2를 만들어보고
            Knight knight2 = knight;    // 기존 mage와 knight의 체력을 변화시켜보자
            mage.hp = 2;
            knight.hp = 2;

            Console.WriteLine($"mage2 hp is {mage2.hp}, knight2 hp is {knight2.hp}");
            // 출력결과 기존 캐릭터를 바꿧음에도 knight2의 체력이 바뀐다.
            // 복사를 사용하였기에 mage2는 별개이지만
            // 참조를 사용한 knight2는 실제 기존 knight와 같은 인물이 된것.

            Knight knight3 = new Knight(); // 새로운 knight를 만들고싶으면 new를 사용해라
                                           //근데 new쓰기가 싫다면? = Deep Copy를 사용해라
            Knight knight4 = knight.Clone();
        }

        // [스택과 힙]
        // 참조와 복사를 이해하기 위해서는 포인터 구조에 대해 알아둬야한다. 이론을 살펴보자
        //stack 메모리 = 임시적 저장소, ex)함수 내부에서 연산할때 잠깐쓰는 임시값
        //복사의 경우 본체의 데이터'값'을 호출함. 그러나 참조의 경우 주소를 호출함
        //heap 메모리 = 영구적 저장소, ex)위와 같은 주소값(주소에 할당된 데이터)

        //c++의 경우 heap메모리의 해제를 수동으로 해줘야됨. 근데 C#은 알아서 해제해줌
        //그래서 c#이 메모리 관리에 좀더 수월하다.


        // [static의 정체]
        //public은 변할 수 있음. ex) knight.hp
        //그러나 static 을 붙이면 각 필드에 종속이 아닌 knight자체에 종속적인 개념임.
        //그래서 모든 객체들이 static을 공유함. item id라고 생각하면 됨.

        //함수에 static을 붙인다는것은? 붕어빵틀(클래스)에 종속되는 틀,
        //static이 붙지않은 함수는? 객체에 종속되는 메소드(객체가 존재해야함)

        class Staticknight
        {
            static public int id = 111;
            public int hp;
            public int attack;

            static public void Test() //
            {
                id++;
            }
            static public Staticknight createSknight()
            {
                Staticknight sknight = new Staticknight();
                sknight.hp = 100;
                sknight.attack = 10;
                return sknight;
            }
            public void Move()
            {
                Console.Write("Move!");
            }
        }

        static void staticTestPlace(string[] args)
        {
            Staticknight sknight = Staticknight.createSknight(); //static메소드라 이런식으로 New없이 사용가능
            //Staticknight.메소드 로 사용가능하다는말
            sknight.Move(); //static이 없기에 위와같이 객체를 먼저 선언해야됨

            Console.WriteLine(); //그렇기에 샘플없이 사용할수 있는 클래스.writeline인 얘도 static타입인 걸 알수있다.

            Random rand = new Random(); //애는 new가 필요한 static타입이 아니다는 걸 유추가능 
        }

        // [객체지향의 3대 속성] 상속,은닉,다형
        // 1. 상속성
        // 앞서 knight뿐만 아니라 다른 Archer Mage등의 클래스를 만든다고 하자.
        // id hp attack 등이 얘네도 똑같이 필요함. 이를 필요할때마다 클래스내에 코드로 쓰는건 비효율적

        // 그렇기에 상속을 사용한다. (부모 <-> 자식)
        // 플레이어라는 상위 클래스를 만들고, 각각 직업들은 플레이어를 상속받아 만들면된다.
        // 상속을 받았기에 상위 클래스가 들고있던것을 물려받게됨
        class Player
        {
            static public int id;
            public int hp;
            public int attack;

            public Player()
            {
                Console.WriteLine("Player 생성자 호출");
            }
            public Player(int hp)
            {
                this.hp = hp;
                Console.WriteLine("Player hp 생성자 호출");
            }
        }
        class Archer : Player // 자식 : 부모
        {
            //기본적인 스탯들은 부모클래스에서 상속받음 // 즉, 필드가 딸려옴
            public Archer() : base(100)
            {
                base.attack = 20; // base로 부모의 변수를 조정할 수 있다.
                Console.WriteLine("Archer 생성자 호출");
            }
            //주로 오브젝트 필드를 하나 만들고, 이를 세분화 시켜 플레이어, 몬스터로 파생시켜 사용
        }//상속은 상위 필드를 좀더 세분화 시킬때 하위(자식) 필드를 만드는 것이라 이해하자.


        // 2.은닉성
        // public private 등을 사용해 접근을 한정시키는것 

        // public : 모두에게 공개, 어디서든 접근가능

        //private : 클래스 내부에서만 접근 가능 (클래스 로직단계에서만 변경가능)
        /* ex) private int hp 일경우
          Knight.hp = 100; 로는 수정불가
          knight.Sethp(100); 과 같이 내부 메소드를 만들어줘야한다.
         */

        // protected : 같은 패키지 내 상속받은 클래스까지 접근가능
        // 자식에게는 열어준다.

        //접근한정자를 쓰지 않는다면 기본적으로 private로 취급함.


        //[클래스 형식변환]
        //상속성에 관한 추가내용임.
        //상속의 장점은 코드의 재사용을 줄인다는 것이다.

        //만약 내부속성에 mp가 추가된 mage가 있을때
        /* mage타입을 player타입으로 만들수있다.
         * 하지만 모든 player타입은 mage타입이 될 수 없다. (mp가 없으니)
         * 만약 강제로 변환시키고 싶다면 Mage mage2 = (Mage) player 라고 괄호를 만들어야한다.
         * 사실 이렇게해도 에러가 뜨는데 빌드 단계에서는 신호가 안떠 잘못된걸 알수없다.*/

        static void EnterGame(Player player)
        {
            bool isMage = (player is Mage); //player가 mage타입인지 확인하는 코드
            if (isMage)
                Console.Write("its mage");

            Mage mage = (player as Mage); //as를 사용해 player타입을 확인. player가 Mage가 아니면 null이 들어감
            if (mage != null)
                Console.Write("player is not Mage type");
            //as를 많이 사용한다.
        }

            //[3. 다형성]
            // 대부분 메소드에서는 상위 클래스를 매개변수로 입력한다.
            //그럴경우 입력인자에 따른 내부 메소드가 아닌 상위 클래스의 메소드를 사용하게된다.(메소드 이름이 같다면)
            // 이때 사용하는 것이 다형성 
            //virtual 이라는 가상 함수를 붙여주고 하위에는 override를 사용한다. 중요
            //오버로딩 아님(오버로딩은 함수이름 재사용, 오버라이딩은 다형성을 이용)

            /* class Player내부에 public virtual void Move() 를 만들고
             *자식 클래스 mage에는 public override void Move() 를 사용한다.
             *이러면 함수가 인자의 실제형태를 파악하고 해당하는 메소드를 사용한다.

             *즉, 상위에서 virtual을 만들고 하위에는 override 메소드를 만든다.*/

            //추가로 c# 에만 있는 문법. public sealed override void Move() 실드가 추가됬다.
            //더욱 하위에서는 오버라이드 할수없다는 뜻. 잘안씀
        static void AboutString()
          {  // [String 문자열 부록]
            //스트링도 하나의 클래스임. 무슨 메소드가 있는지 알아보자. (나중에 구글써서 활용해라)
            string name = "Harry Potter";

        // 1.찾기
        bool found = name.Contains("Harry");
        int index = name.IndexOf('P');

        // 2. 변형
        name = name + " Junior"; // harry potter junior 가 된다.
            string lovwerCaseNmae = name.ToLower();
        name.ToUpper();
            string newName = name.Replace('r', 'l');

        // 3.분할
        string[] Newnames = name.Split(new char[] { ' ' }); // 띄어쓰기를 기준으로 분리해줘라
        string substringNmae = name.Substring(5);

        //온갖메소드가 있따. 그러니 필요할때마다 찾아써라
    }
        }
    }

}
