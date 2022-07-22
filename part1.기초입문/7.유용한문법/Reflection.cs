using System;
using System.Reflection;

namespace _7.유용한기타문법
{
    // [Reflection] 
    // X-Ray를 찍는것 = 런타임 도중에 클래스의 모든 정보들을 뜯어볼수있다.
    class Program
    {
        class Monster
        {
            [Important("Very important")] // Attribute 문법 // 어이없는게 얘가 Attack아래로, 클래스 맨 아래로 가면 에러남.

            public int hp;
            public int attack;
            private float speed;

            void Attack()
            {

            }
        }

        static void Main(string[] args)
        {
            Monster monster = new Monster();

            Type type = monster.GetType(); //GetTpye은 모든 클래스의 상위인 Object클래스에 있는 메소드임

            var fields = type.GetFields(System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Instance);

            foreach(FieldInfo field in fields)
            {
                string access = "protected";
                if (field.IsPublic)
                    access = "public";
                else if (field.IsPrivate)
                    access = "Private";

                Console.WriteLine($"{access} {field.FieldType.Name} {field.Name}");
            }

            // 문법적으로 설명도 없고 그냥 개념 설명이 대부분이기에
            // Reflection은 클래스의 내용을 런타임 도중에 알수있는 문법이다. 라는 것만 알아두자.

            // Attribute ,리플렉션과의 짝꿍
            // 우린 중요정보를 남기기위해 주석(//)을 남긴다.
            // 그러나 주석은 사람에게 남기는것임. 컴퓨터에게 중요정보를 주고싶다면?
            // Attribute를 사용한다.
        }
        class Important : System.Attribute // 컴퓨터에게 런타임도중에 체크할수있게해주는 주석.
        {
            string message;
            public Important(string message)
            { this.message = message; }
        }
    }

}
