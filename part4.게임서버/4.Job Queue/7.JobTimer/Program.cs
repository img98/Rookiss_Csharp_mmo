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

        static void FlushRoom()
        {
            Room.Push(() => Room.Flush()); //룸에게 Flush하라는 명령을 전달
            JobTimer.Instance.Push(FlushRoom, 250); //0.25초 뒤에, FlushRoom이라는 함수를 다시 호출해주라고 push
        }

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

            //FlushRoom(); //처음 FLush를 의뢰함과 동시에 다음 Flush를 예약하는 FlushRoom을 잡큐에 넣어줘야한다.
            JobTimer.Instance.Push(FlushRoom); //위와 같은의미로 이렇게 FlushRoom을 큐에 넣으라고 해도 된다.
            while (true)
            {
                JobTimer.Instance.Flush(); //FlushRoom내에 재귀적으로 FlushRoom이 있기에, flush를 하게되면, 계속 FlushRoom이 예약됨.
            } 

        }
    }
}
