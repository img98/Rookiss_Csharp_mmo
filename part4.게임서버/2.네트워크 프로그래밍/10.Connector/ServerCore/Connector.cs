using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Connector
    {
        Func<Session> _sessionFactory; //참고로 session을 abstract로 만들었기에, 그저 new Session();으로는 생성할 수 없다.=>그래서 sessionFactory를 쓰는것.

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            // 휴대폰 설정
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory = sessionFactory;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = endPoint;
            args.UserToken = socket; // UserToken에 원하는 정보(지금은 socket)를 담아서 넘겨줄수있다.
            //소켓을 밖으로 빼서 사용하지 않는 이유 = 여러개의 connect입력이 들어올수있으니 이벤트를 통해 인자를 넘겨준것
            RegisterConnect(args);
        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket; //토큰에 담긴 정보 활용
            if (socket == null)
                return;
            
            bool pending = socket.ConnectAsync(args);
            if (pending == false)
                OnConnectCompleted(null, args);
        }
        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.ConnectSocket); //args.ConnectSocket = args에 소켓 새로하나 연결 (제공되는 라이브러리다.)
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine($"OnConnectCompleted Fail : {args.SocketError}");
            }
        }

    }
}
