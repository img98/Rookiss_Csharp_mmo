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
        public static GameRoom Room = new GameRoom();

        static void Main(string[] args)
        {
            //DNS (Domain Name System)
            //서버주소를 172.12.x.x. 이런식으로 하드코딩으로 쓰는게아닌, www.google.com이렇게 주소로 쓰는거임. 경우에 따라 ip주소가 바뀌는 경우도 있기에, 하드코딩보다는 DNS를 쓰는게 낫다.
            string host = Dns.GetHostName(); //내 컴퓨터의 Host이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); }); //전에는 new ClientSession()을 리턴해서, 새로운 세션을 여기서만들었지만, 지금은 그러한 생성을 SessionManager를 통해 처리. 매니저를 사용했기에, 후에 이 세션의 관리가 더욱편해짐

            Console.WriteLine("Listening...");

            while (true)
            {
                ;//사실 while문이 없어도, 우리가 만든 listener의 구조가 무한히 돌아가는 형태이기에 필요는 없음 .그저 프로그램이 끝나지않게 while문을 넣어만놓자.
            }

        }
    }
}
