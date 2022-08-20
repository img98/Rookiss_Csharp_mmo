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
    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            PacketManager.Instacne.Register(); //아직 멀티쓰레드가 돌기전에 빠르게 패킷매니저를 등록해버리면, 멀티 쓰레드로인한 변질을 걱정할필요없다.

            //DNS (Domain Name System)
            //서버주소를 172.12.x.x. 이런식으로 하드코딩으로 쓰는게아닌, www.google.com이렇게 주소로 쓰는거임. 경우에 따라 ip주소가 바뀌는 경우도 있기에, 하드코딩보다는 DNS를 쓰는게 낫다.
            string host = Dns.GetHostName(); //내 컴퓨터의 Host이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return new ClientSession(); }); //어떤 세션을 사용할지만 전해주기

            Console.WriteLine("Listening...");

            while (true)
            {
                ;//사실 while문이 없어도, 우리가 만든 listener의 구조가 무한히 돌아가는 형태이기에 필요는 없음 .그저 프로그램이 끝나지않게 while문을 넣어만놓자.
            }

        }
    }
}
