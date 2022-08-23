using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;

namespace Server
{
	class ClientSession : PacketSession
    {
        public int SessionId { get; set; }
        public GameRoom Room { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; } //사실 Player라는 새로운 클래스를만들고, ClientSession에서 걔를 호출하는게 좋은데, 지금은 귀찮으니 좌표xyz 각각 만들어줌

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
                        
            Program.Room.Push(() => Program.Room.Enter(this));
        }

       
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instacne.OnRecvPacket(this, buffer); //매니저의 Recv와 연결
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if(Room!=null)
            {
                GameRoom room = Room;//잡큐로 인해, Room=null이후에 leave가 일어남. 그러면 Room.leave는 null.leave가 되어버려 에러가 발생. 이걸막기위해 Room을 복사한 room을 통해 leave를 호출해야한다. 
                room.Push(() => room.Leave(this)); //클라이언트의Room은 null이 됬지만, room은 계속 이전의Room을 참조할수있기에, 이러한 코드가 가능해진다.
                Room = null;
            }
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }


        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
