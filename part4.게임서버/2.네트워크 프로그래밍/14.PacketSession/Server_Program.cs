using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class Packet
    {
        public ushort size; //int가 아니라 ushort로도 충분 (4바이트 절약가능)
        public ushort packetId;
    }

    class GameSession : PacketSession
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
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset+2); //+2는 size 2바이트를 뒤에 id가 나옴을 표현한건데, 나중엔 이렇게 하드코딩안할거임. 지금은 귀찮아서+2
            Console.WriteLine($"RecvPacketId: {id}, Size: {size}");
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

    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            //DNS (Domain Name System)
            //서버주소를 172.12.x.x. 이런식으로 하드코딩으로 쓰는게아닌, www.google.com이렇게 주소로 쓰는거임. 경우에 따라 ip주소가 바뀌는 경우도 있기에, 하드코딩보다는 DNS를 쓰는게 낫다.
            string host = Dns.GetHostName(); //내 컴퓨터의 Host이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return new GameSession(); }); //어떤 세션을 사용할지만 전해주기

            Console.WriteLine("Listening...");

            while (true)
            {
                ;//사실 while문이 없어도, 우리가 만든 listener의 구조가 무한히 돌아가는 형태이기에 필요는 없음 .그저 프로그램이 끝나지않게 while문을 넣어만놓자.
            }

        }
    }
}
