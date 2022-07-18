using System;
using System.Collections.Generic; //잠재적 수정사항으로 추가함. List쓸때 이거없으니 에러나더라

namespace _7.유용한기타문법
{
    // [Lambda] : 일회용 함수를 만드는데 사용하는 문법이다.
    enum ItemType
    {
        Weapon,
        Armor,
        Amulet,
        Ring
    }
    enum Rarity
    {
        Normal,
        Uncommon,
        Rare
    }
    class Item
    {
        public ItemType ItemType;
        public Rarity Rarity;
    }

    class Program
    {
        static List<Item> _items = new List<Item>();

        static Item FindWeapon() // 아이템을 찾는 함수를 만들고싶다.
        {
            foreach(Item item in _items)
            {
                if (item.ItemType == ItemType.Weapon)
                    return item;
            }
            return null;
        }

        static Item FindRareItem() // 이번엔 레어 아이템을 찾는 함수를 만들고싶다.
        {
            foreach (Item item in _items)
            {
                if (item.Rarity == Rarity.Rare)
                    return item;
            }
            return null;
        }
        // 이렇게 함수를 용도에 따라 하나하나 늘리는건 굉장히 비효울적이다.

        // 위같은 반복을 없에기 위한 방법으로 우린 deligate를 배웠었다.
        delegate bool ItemSelector(Item item); // 다시한번 말하지만 delegate는 함수를 인자로 넣을수 있게해주는 '타입의 한종류'
        
        static Item FindItemDelegate (ItemSelector selector)
        {
            foreach(Item item in _items)
            {
                if (selector(item))
                    return item;
            }
            return null;
        }
        
        static bool IsWeapon(Item item) 
        {
            return item.ItemType == ItemType.Weapon;
        }
        // 그런데 여러 함수를 만들기싫어서 delegate함수를 만들었으면서 그것을 위해 이런식으로 여러 함수인자를 만드는건 주객전도임.
        // 즉, FIndItemDelegate(isWeapon)에 들어가는 인자처럼, 딱 한번만 쓰는 문법이 없을까? ->Main에 나오는 Lambda문법을 보자.

        static void Main(string[] args)
        {
            _items.Add(new Item() { ItemType = ItemType.Weapon, Rarity = Rarity.Normal });
            _items.Add(new Item() { ItemType = ItemType.Armor, Rarity = Rarity.Uncommon });
            _items.Add(new Item() { ItemType = ItemType.Ring, Rarity = Rarity.Rare });

            Item item = FindItemDelegate(IsWeapon); //사용시 이런식으로 쓰면된다.

            //무형 함수, 익명 함수 = 이름이 없는 함수사용 //초창기에 나온문법. 근데 이거도 귀찮아서 쓰는게 lambda식
            Item item2 = FindItemDelegate(delegate (Item item) { return item.ItemType == ItemType.Weapon; }); // 이런식으로 isWeapon 대신 delegate를 써준뒤
            // 필요한 내용까지 한번에 넣어줄수있다. 
            //만약 나중에 똑같이 weapon인지 찾는 경우가 생기면 똑같은 코드를 다시 쓰긴해야한다. 그러나 딱한번 쓰는 희귀 케이스에는 쓰기 좋은 문법이다.

            //Lambda 문법 // 얘도 무명함수임.
            Item item3 = FindItemDelegate((Item item) => { return item.ItemType == ItemType.Weapon; });
            // delegate조차 쓰지않았다.
            // ( 입력값 => 반환값 ) 꼴이다

            MyFunc<Item, bool> selector = (Item item) => { return item.ItemType == ItemType.Weapon; };
        }

        // delegate로 인자를 넘긴다고 했는데, 이것을 좀 더 일반화시키는 방법이 있다.
        // delegate 타입에도 Generic을 사용할수있다.
        delegate Return MyFunc<T, Return>(T item); // 입력형식 하나와 반환형식 하나 있는 delegate인자는 이걸로 전부 해결이 된다.

        // 근데 위 MyFunc는 사실 Func라고 이미 만들어져있음!!
        // 앞으로 delegate를 직접 선언하지 않아도, 이미 만들어진 애들이 존재함.(Func Action)
        // return 을 void로 하는 action 이라고 잇음

        // 반환타입이 있으면 Func
        // 반환타입이 없으면 Action

    }
}
