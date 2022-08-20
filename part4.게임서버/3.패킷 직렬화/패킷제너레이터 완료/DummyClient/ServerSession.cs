using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCore;

namespace DummyClient
{
	class ServerSession : Session //세션이라는게 결국 대리자(식당 대리인)의 개념이기에, 클라쪽에 서버세션이 있는게 맞다.
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            C_PlayerInfoReq packet = new C_PlayerInfoReq() { playerId = 1001, name = "ABCD" }; //보낼패킷을 임의로 만들었다.

			var skill = new C_PlayerInfoReq.Skill() { id = 101, duration = 3.0f, level = 1 };
			skill.attributes.Add(new C_PlayerInfoReq.Skill.Attribute() { att = 77 });
			packet.skills.Add(skill);

            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 201, duration = 3.0f, level = 2 });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 301, duration = 3.0f, level = 3 });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 401, duration = 3.0f, level = 4 });


			//보낸다
			//for (int i = 0; i < 5; i++) 연습으로 한번만 보낼거라 임시 주석
			{
                ArraySegment<byte> s = packet.Write();
                if (s != null)
                    Send(s);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");

            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
