using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure_Algorithm
{
    class Board //말그대로 보드판. 현재 상황(데이터)을 담는 판
    {
        //데이터를 담는 방법에는 여러 방법이있다.
        public int[] _data = new int[25];   // 배열로 담기
        public List<int> _data2 = new List<int>();  // 동적 배열에 담기 //C#에서는 list는 동적배열임
        public LinkedList<int> _data3 = new LinkedList<int>(); // 연결 리스트
        
        // 배열, 동적배열, 연결리스트 의 차이?
        // 호텔에 방을 잡는다고 생각해보자
        // 배열: 연속된 방을 잡을수있음, but 방을 추가하거나 축소불가능
        // 동적배열 : 근본이 배열이니 연속된 방을 잡을수있음, 방 추가/축소가 자유로움 but 배열 중간에 삽입/삭제는 어려움
        // (동적배열 할당정책) : 실제사용량보다 메모리를 많이 확보해둠(낭비) 한 2배좀 안되게? 그리고 동적이동을 최소화해야됨
        // 연결리스트 : 연속되지 않는방 사용 = 중간에 추가/삭제 쉬움 but 연속되지 않기에 N번째 방을 바로 찾을수없음.

        // 지금 상황의 경우 보드판 즉, 맵을 구현하는 것이기에 맵이 함부로 커지거나 작아지지 않는것을 생각하면 배열로 하는게 쉽고좋다.

        // [동적배열 구현]
        // List. 으로 잘구현되어있긴한데,,, 비슷한 함수를 직접만들어서 연습이나 해보자.

        public void InitializeList()
        {
            _data2.Add(101);
            _data2.Add(102);
            _data2.Add(103);
            _data2.Add(104);
            _data2.Add(105);

            int temp = _data2[2];

            _data2.RemoveAt(2);
        } // 이게 구비된 메소드. 이제 Add, index찾기, RemoveAt을 구현해보자

        class MyListFunc<T>
        {
            const int DEFAULT_SIZE = 1;
            T[] _data = new T[DEFAULT_SIZE];

            public int Count = 0; // 실제 사용중인 데이터 갯수
            public int Capacity { get { return _data.Length; } } // 예약된(확보해논) 데이터 갯수

            public void Add(T item)
            {
                // 1. 공간이 남아있는지 확인한다.
                if (Count >= Capacity) //여유가 없을때
                {
                    //공간을 더 확보한다.
                    T[] newArray = new T[Count * 2]; // 새로 이사갈곳먼저 만들고 옮기기
                    for (int i = 0; i < Count; i++)
                        newArray[i] = _data[i];
                    _data = newArray;
                }
                // 2. 공간에 데이터 넣기
                _data[Count] = item;

                Count++;
            } //O(1)

            public T this[int index]
            {
                get { return _data[index]; }
                set { _data[index] = value; }
            } //O(1)

            public void RemoveAt(int index) // 중간 인덱스를 삭제하기
            {
                for(int i=index;i<Count-1;i++)
                {
                    _data[i] = _data[i + 1];
                }
                _data[Count - 1] = default(T); // T의 초기형식으로 밀어주세요 라는 뜻.

                Count--;
            } //O(N)
        }

        public void InitializeLinkedList() // 위와 마찬가지로 연결리스트의 기능과 구현을 해보자.
        {
            _data3.AddLast(101);
            _data3.AddLast(102);
            LinkedListNode<int> node = _data3.AddLast(103);
            _data3.AddLast(104);
            _data3.AddLast(105); //보면 얘는 데이터 추가뿐 아니라, 그 데이터로 접근하는 LinkedListNode를 반환함

            _data3.Remove(node); // 노드값을 통해 value를 지운다.
        } // 연결리스트는 value의 앞뒤로 Node를 추가해, 앞value와 뒷value의 주소를 담는다. (학교에서 배운 그것)

        class Room<T> // 노드는 잊어버리고 호텔예제로 생각해보자.
        {
            public T Data;
            public Room<T> Next; //참조를 넘긴다. 참조->주소값 이용
            public Room<T> Prev;
        }
        class RoomList<T>
        {
            public Room<T> Head = null;
            public Room<T> Tail = null;
            public int Count = 0;

            public Room<T> AddLast(T data)
            {
                Room<T> newRoom = new Room<T>();
                newRoom.Data = data;

                if (Head == null) //지금 넣는 데이터가 처음이라면, 지금 방이 head가 되야겠지
                    Head = newRoom;

                // 101 102 103 
                if(Tail != null) // 기존에 tail이 있었다면. (만약 지금 넣는data가 처음이라면 tail=null일것이다.)
                {
                    Tail.Next = newRoom; // 기존 tail이랑 지금 들어오는 애랑 연결시켜주는것
                    newRoom.Prev = Tail;
                }
                Tail = newRoom; //지금 추가된 방을 tail로 인정한다.

                Count++;
                return newRoom; //위 기존기능에도 말했지만 LinkedListNode를 반환한다고 했던것을 표현함.
            } //O(1)

            public void Remove(Room<T> room)
            {   // 일단 제거하는 애가 무조건 list안에 있다고 가정하자. //안에 없으면 몇가지 if처리해줘야됨

                if (Head == room)
                    Head = Head.Next; //지금 삭제하는게 head라면 head 다음 노드를 새로운 head로 인정한다.
                                      //애초에 리스트에 하나만 있어서 head.next가 없다면 문제가 될까? -> 문제없다. head.Next가 애초에 null이니까 괜찮다.

                if (Tail == room)
                    Tail = Tail.Prev; //기존 tail의 이전방을 tail로 인정한다.

                //위 두개는 극단적인 상황. 이제 일반적인 상황을 구현하자.
                // 중간방의 앞과 뒤를 이어줘야한다.
                if (room.Prev != null) //room.Prev.Next를 이용하기에 null체크를 해줘야한다. //prev방이 존재한다면 연결한다는뜻
                    room.Prev.Next = room.Next;
                if (room.Next != null)
                    room.Next.Prev = room.Prev;

                Count--;
            } // O(1)

        }

    }
}
