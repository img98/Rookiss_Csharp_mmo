using System;

namespace _7.유용한기타문법
{
    class Program
    {
        // [Property]
        // 객체지향의 은닉성을 사용 (필요없는 내용은 외부에서 접근 불가) public, private

        class Knight
        {
            // 외부에서 함부로 hp를 수정할수없게 protected로 접근을 막고, 꼭 필요할 경우 수정할수 있게 아래처럼 함수를 만들었었다.
            protected int hp;

            // Getter Get함수
            public int GetHp()
            {
                return hp;
            }

            // Setter Set함수
            public void SetHp(int hp)
            {
                this.hp = hp;
            }

            //그러나 위처럼 만드는것도 나중가면 번거로움 -> Property를 사용하자
            // 함수랑 사용법이 비슷해서 문법도 비슷함.

            public int HP // 이게 프로퍼티문법
            {
                get { return hp; }
                set { this.hp = value; } //value라는 디폴트 변수를 사용
            }

        }

         
        static void Main(string[] args)
        {
            Knight knight = new Knight();

            knight.HP = 100;
            int hp = knight.HP; //이런식으로 변수처럼 사용할수있다. 단, 프로퍼티 문법 이름대로(HP)
            // Set은 값을 넣을때, Get은 값을 리턴할때 사용함
            //만약 함부로 set 못쓰게 외부에서 막고싶다면? private를 달아준다. private set {hp = value;}

            // 즉, 굳이 GetHp(), SetHp() 두개의 함수를 만들지 않아도 HP하나만으로 두 기능을 만들수있다는 장점 잇음


        }

        //여기서 더나아가 '자동구현 프로퍼티'가 있다.
        // HP라는 새로운 필드를 만드는것조차 귀찮다면
        class Mage // 이렇게 하나만 넣어도된다.
        {
            public int HP
            {
                get; set;
            }
        } //이걸 극한으로 압축하면 class Mage { public int HP {get; set;} } 이렇게 한줄로 끝남
            //그러나 위 내용은 private int hp; int GetHp(); int SetHp(); 3개의 내용이 담겨있음

    }
}
