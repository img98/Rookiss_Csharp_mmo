using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();

        static void OnAccpetHandler(Socket clientSocket)
        {
            try
            {
                //손님의 명령 받기 (데이터 받기)
                byte[] recvBuff = new byte[1024]; //임의의 버퍼 
                int recvBytes = clientSocket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes); //문자열과 byte배열을 변환하는 것이라 생각하자.
                Console.WriteLine($"[From client] {recvData}");

                // 보내기 (서버에서 클라이언트로 데이터 보내기)
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome TO MMORPG Server !"); //위에선 클라이언트에서 무슨말을 할지모르니 1024바이트 배열을 만들었지만, 지금은 무슨말을 보낼지 선택할수 있느니 바로 인코딩
                clientSocket.Send(sendBuff);

                //쫓아낸다
                clientSocket.Shutdown(SocketShutdown.Both); // 서로 통신종료라고 미리 알려주기
                clientSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void Main(string[] args)
        {
            //DNS (Domain Name System)
            //서버주소를 172.12.x.x. 이런식으로 하드코딩으로 쓰는게아닌, www.google.com이렇게 주소로 쓰는거임. 경우에 따라 ip주소가 바뀌는 경우도 있기에, 하드코딩보다는 DNS를 쓰는게 낫다.
            string host = Dns.GetHostName(); //내 컴퓨터의 Host이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, OnAccpetHandler); //목표경로는 endPoint이고, 무슨신호가 온다면 OnAccpetHandler로 알려달라는뜻

            while (true)
            {
                Console.WriteLine("Listening...");

                ;//사실 while문이 없어도, 우리가 만든 listener의 구조가 무한히 돌아가는 형태이기에 필요는 없음 .그저 프로그램이 끝나지않게 while문을 넣어만놓자.
            }

        }
    }
}
    