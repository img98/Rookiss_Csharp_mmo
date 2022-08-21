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

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            //TODO 채팅방에 강제로 입장시키기
            Program.Room.Enter(this);
        }

       
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instacne.OnRecvPacket(this, buffer); //매니저의 Recv와 연결
            //이젠 로그 안찍음
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if(Room!=null) // 해당session에 연결된 Room값이 있다면
            {
                Room.Leave(this); //Leave를 통해 Room의 _sessions에서 this를 제거
                Room = null; //그리고 Room을 밀어버려서 this와 _sessions의 연결고리를 끊는다.
            }
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }


        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
