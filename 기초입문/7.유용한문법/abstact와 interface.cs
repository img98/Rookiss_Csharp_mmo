using System;

namespace _7.유용한기타문법
{
    class Program
    {
        // 추상클래스 와 interface
        // 상속받아 만드는 클래스에 무조건 넣고싶은(overide) 메소드가 있다면
        // abstract라는 추상클래스를 사용해라.
        abstract class Monster
        {
            public abstract void Shout(); //추상적인 메소드이기에 내용{}이 있어선 안됨 //그러나 상속받는 애들은 애를 무조건 만들어야함
        }

        abstract class Flyable //나는 여러 기능이있는 클래스를 만들고싶어. 하지만 다중상속은 C#에서 불가능함
            // interface를 사용해라
        {
            public abstract void Fly();
        }

        class Orc : Monster
        {
            public override void Shout()
            {
                Console.WriteLine("Waagh!");
            }
        }

        interface IFlyable //interface는 자신을 물려받는 애들은 무조건 어떤 메소드가 있어야한다는 의미.
        {
            void Fly();
        }

        class FlyableOrc : Orc, IFlyable
        {
            public void Fly()
            {

            }
        }

        // 즉, 클래스에 내가 원하는 기능을 넣고싶을때 쓰는방법
        // 1. 추상화 (Abstract) 를 사용한다
        // 2. 그러나 abstract의 경우 여러 부모를 가질수없기에 interface를 사용하는게 더 유용함.
        // 2.1 interface는 구현 내용에 대해 정확히 명시할 필요없기에 여러개 존재할수있다.



        static void Main(string[] args)
        {

        }
    }
}
