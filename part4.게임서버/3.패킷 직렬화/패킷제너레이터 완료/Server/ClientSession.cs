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
	class ClientSession : PacketSession //세션이라는게 결국 대리자(식당 대리인)의 개념이기에, 서버쪽에 클라세션이 있는게 맞다.
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            //Packet packet = new Packet() { size = 100, packetId = 10 };

            /*
            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(packet.size);
            byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset+buffer.Length, buffer2.Length);
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length); //다썻으니 이만큼 썻다고 알려주면, 보내야될 범위 리턴함 = sendBuff
            */

            //Send(sendBuff);
            Thread.Sleep(5000);
            Disconnect();
        }

        //기존의 OnRecv는 패킷세션만들면서 seal해줬으니 아예 지워버림
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instacne.OnRecvPacket(this, buffer); //매니저의 Recv와 연결
            //이젠 로그 안찍음
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }


        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
