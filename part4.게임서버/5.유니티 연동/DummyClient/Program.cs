using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // DNS
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();

            connector.Connect(endPoint,
                () => { return SessionManager.Instance.Generate(); },
                300);

            while(true)
            {
                try
                {
                    SessionManager.Instance.SendForEach(); //더미클라의 여러 세션들이 서버에 패킷을 보내기위한 함수
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(250); //쉼없이 패킷을 보내면 우리가 보기도 힘들테니, 0.1초마다 보내도록 하자.
            }
        }
    }
}
