using System;

namespace _7.유용한기타문법
{
    class Program
    {
        // 여러 타입을 사용하고 싶다면 어떻게 해야할까?
        // 1차원적으론 여러개의 타입을 호출하면 된다. -> 단 귀찮고 무식한 방법임
        // 그렇기에 object, generic같은 방법이 있다.

        class MyList<T> //꺽쇠안에 T (type) 을 적어서 만든다. 아무런 타입이 와도 괜찮다는 뜻
        {
            T[] arr = new T[10];

            public T GetItem(int i) //반환하는 타입도 T 로 놔도된다.
            {
                return arr[i];
            }
        }

        class Monster { };

        static void Main(string[] args)
        {
            // object는 object라는 애매한 타입으로 뭉뜽그려서 보관하는것
            object obj = 3;
            object obj2 = "hello world";

            int num = (int)obj;
            string str = (string)obj2; // 사용시에는 적합한 형태로 casting해줘야됨

            // var은 컴퓨터가 적합한 타입을 찾아주는 것이라 다른 로직임.

            // object의 경우 처리속도가 느리기에 모든 타입을 object로 쓰기에는 무리가 있음 ->일반화를 사용하자.

            //일반화를 사용할때에는 위에서 클래스를 만들어준후에 실사용때 필요한 타입을 넣으면된다.
            MyList<int> myIntList = new MyList<int>(); 
            MyList<short> myShortList = new MyList<short>(); 
            MyList<Monster> myMonsterList = new MyList<Monster>(); // 타입으로 클래스를 넣을수도있다.

        }

        //이뿐 아니라 어느타입의 인자가 들어와도되는  함수도 만들수있다.
        static void Test<T, K> (T input, K input2)
        {
            //인자끼리의 타입이 다를수있으니 그때는 T, K 이런식으로 다르게 하나봄
        }
    }
}
