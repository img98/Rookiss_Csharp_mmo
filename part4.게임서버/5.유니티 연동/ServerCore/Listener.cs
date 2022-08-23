using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket; 
        Func<Session> _sessionFactory; //지금이야 사용하는 session이 GameSession뿐이지만, 후에 여러 session이 생긴다면 어떤 session을 받아 사용할지 알기위한 코드

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            //문지기의 핸드폰
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _sessionFactory += sessionFactory; //OnAcceptCompleted가 실행되면, 콜백방식으로 외부 함수에 신호를 보내서 데이터 받기 보내기등의 기능을 실행시키라고 명령할것임.

            //문지기 교육 = 문지기의 핸드폰에 주소를 알려줌
            _listenSocket.Bind(endPoint);

            //영업시작
            //backLog : 최대 대기수
            _listenSocket.Listen(backlog);

            for (int i = 0; i < register; i++) //문지기를 register만큼 생성 => connect요청이 밀려도, 멀티쓰레드를 이용해 잘처리할수 있을것으로 기대됨
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs(); //AcceptAsync가 펜딩 후에 이벤트를 OnAccpetCompleted에 보낼 그릇을 만든다.
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted); //인자로 콜백함수를 넣어준다.
                RegisterAccept(args);
                //즉 위코드를 설명하면, Listener가 생성됨과 동시에, 커넥트 요청을 받는 함수(RegisterAccpet)를 실행시키고, 요청을 받는다면(args.Completed) 콜백방식(+=)으로 OnAccpetCompleted가 호출된다는 것
            }
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;//args가 계속 정보를 물고 움직이는데, 새로운 요청이 들어오면 그전에 있던것을 싹 비우는, 초기화를 해줘야된다.
                                     //이벤트를 재사용할때는 초기화해줘라.

            bool pending = _listenSocket.AcceptAsync(args); //값을 넣어달라고 요청, 그리고 결과를 bool로 뱉어넴 //return으로 보류여부(pending)을 뱉는다.
            if (pending == false) //pending이 false라는건, 운좋게 Accept를 돌리자마자 클라이언트가 와서 보류한적이 없다는 것을 의미함
                OnAcceptCompleted(null, args);
            //펜딩이 true라면 나중에 콜백방식으로 해결될것임.
        } 

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)  //에러가 성공이라면 (걍 에러없이 잘됬다는 뜻, 우리가 확인할때 쓴 debug log라고 생각해보자)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);                
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccept(args); // 여기까지 온다는건, 위에서 커넥트요청 성공이든 실패든 모든 처리가 끝났음을 의미하므로, 다음 요청을 받겠다는것! (마치 재귀함수같네)
        }

    }
}
