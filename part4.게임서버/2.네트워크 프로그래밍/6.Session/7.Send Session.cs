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
        
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs(); //매번 sendArgs를 만들게아니라, 있는걸 재사용하고싶다.
        Queue<byte[]> _sendQueue = new Queue<byte[]>(); //보낼데이터를 담을 큐
        bool _pending = false; //send는 하나만 쓸거고, 멀티쓰레드로 인해 다른곳에서 OnSendCompleted를 사용하고 있는중인지 확인하기 위해 pending을 외부로 꺼냈다.

        object _lock = new object();

        public void Start(Socket socket) // init으로했다가, 시작하는 느낌내려고 start로 바꿈
        {
            _socket = socket;
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);                    
            recvArgs.SetBuffer(new byte[1024], 0, 1024); //지난 서버에서 버퍼를 받을 배열을 만들고 clientSocket.Receive(recvBuff);를 했던 부분과 같다. (담을 버퍼그릇을 만들고 담기까지 한번에한것)

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted); //sendArgs를 재사용할거니 여기저기서 연결할필요없이, 처음에 한번만 해준다.

            RegisterRecv(recvArgs);
        }

        public void Send(byte[] sendBuff)
        {
            //매 데이터마다 send하는게 아니라, OnSendCompleted가 끝날때까지 데이터를 모았다(패킷, 지금은 패킷을 모르니 byte배열 큐로 구현) 한번에 보내는 식으로 고쳐볼것이다.
            lock(_lock) //멀티쓰레드 개념을 생각해야하니, 보내는동안 다른 데이터로 훼손되지않게 lock을 사용하자.
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pending == false) //만약 대기할 필요 없다면 바로 보내기
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1) //원래값이 1이라면 이미 다른데서 disconnected신청이 난거니, 두번 disconnect으로 크래시나지않게 그냥 return;
                return;

            _socket.Shutdown(SocketShutdown.Both); // 서로 통신종료라고 미리 알려주기
            _socket.Close();

        }

        #region 네트워크 통신
        void RegisterSend()
        {
            _pending = true; //지금 보내는중이니까 send에선 일단 큐에 담으라고 말하기
            byte[] buff = _sendQueue.Dequeue();
            _sendArgs.SetBuffer(buff, 0, buff.Length);

            bool pending = _socket.SendAsync(_sendArgs);
            if (pending == false)
                OnSendCompleted(null, _sendArgs);
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock(_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        if(_sendQueue.Count>0) //보내는동안에 누군가 입력해서 sendQueue가 차있다면, RegisterSend가 돌아가게하자.                        
                            RegisterSend();//이러면 내가 보내고있는동안에 누군가 예약을해도 문제없이 다시 RegisterSend가 돌아간다.
                       else
                        _pending = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted fail {e}");
                    }

                }
                else
                {
                    Disconnect(); //보낼때 실패했다는것은 상대방에게 문제가 있다는 것이니 disconnect
                }
            }
        }

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
