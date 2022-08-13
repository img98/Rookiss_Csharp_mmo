using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Session
    {
        Socket _socket;
        int _disconnected = 0;

        public void Start(Socket socket) // init으로했다가, 시작하는 느낌내려고 start로 바꿈
        {
            _socket = socket;
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);                    
            recvArgs.SetBuffer(new byte[1024], 0, 1024); //지난 서버에서 버퍼를 받을 배열을 만들고 clientSocket.Receive(recvBuff);를 했던 부분과 같다. (담을 버퍼그릇을 만들고 담기까지 한번에한것)

            RegisterRecv(recvArgs);
        }

        public void Send(byte[] sendBuff) //센드는 더어려움. 나중에할거라 일단 구색만
        {
            _socket.Send(sendBuff);
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1) //원래값이 1이라면 이미 다른데서 disconnected신청이 난거니, 두번 disconnect으로 크래시나지않게 그냥 return;
                return;

            _socket.Shutdown(SocketShutdown.Both); // 서로 통신종료라고 미리 알려주기
            _socket.Close();

        }

        #region 네트워크 통신
        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);
            if (pending == false)
                OnRecvCompleted(null, args);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred >0 && args.SocketError==SocketError.Success) //BytesTransferred는 전송받은 데이터 크기를 의미하는데, 상대가 접속을 종료하면 0이 올수도있다. 그렇기에 이런 종료상황을 체크를 해줘야됨
            {
                try //혹시 실패할지도 모르니 확인차 트라이캐치
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From client] {recvData}");

                    RegisterRecv(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecevCompleted fail {e}");
                }  
            }
            else
            {
                Disconnect();
            }
        }
        #endregion
    }
}
