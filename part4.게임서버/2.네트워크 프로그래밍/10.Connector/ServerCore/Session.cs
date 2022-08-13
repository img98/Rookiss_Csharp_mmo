using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        object _lock = new object();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs(); //매번 sendArgs를 만들게아니라, 있는걸 재사용하고싶다.
        Queue<byte[]> _sendQueue = new Queue<byte[]>(); //보낼데이터를 담을 큐       
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>(); //sendBufferList에 대입시기기 위해, 일시적으로 sendQueue를 담아놓을 리스트. 매번만들 필요 없으니 밖으로 빼줬다.

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Start(Socket socket) // init으로했다가, 시작하는 느낌내려고 start로 바꿈
        {
            _socket = socket;
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _recvArgs.SetBuffer(new byte[1024], 0, 1024); //지난 서버에서 버퍼를 받을 배열을 만들고 clientSocket.Receive(recvBuff);를 했던 부분과 같다. (담을 버퍼그릇을 만들고 담기까지 한번에한것)

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted); //sendArgs를 재사용할거니 여기저기서 연결할필요없이, 처음에 한번만 해준다.

            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
            // 전에있던 _pending은 _pendingList가 나옴과 동시에, 리스트가 차있는지 여부로 pending을 하고있는지를 알수있어서 지워버렸다.

            //매 데이터마다 send하는게 아니라, OnSendCompleted가 끝날때까지 데이터를 모았다(패킷, 지금은 패킷을 모르니 byte배열 큐로 구현) 한번에 보내는 식으로 고쳐볼것이다.
            lock(_lock) //멀티쓰레드 개념을 생각해야하니, 보내는동안 다른 데이터로 훼손되지않게 lock을 사용하자.
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0) //만약 대기중인 애들이 없다면 바로 보내기
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1) //원래값이 1이라면 이미 다른데서 disconnected신청이 난거니, 두번 disconnect으로 크래시나지않게 그냥 return;
                return;

            OnDisconnected(_socket.RemoteEndPoint); // Disconnect가 성공하면 OnDistconnected에게 콜백
            _socket.Shutdown(SocketShutdown.Both); // 서로 통신종료라고 미리 알려주기
            _socket.Close();

        }

        #region 네트워크 통신
        void RegisterSend()
        {
            while (_sendQueue.Count>0) //매번 SetBuffer로 임시 buff에 넣어줄필요없이 BufferList를 통해 한번에 모아놨다가 보낼수있음.
            {
                byte[] buff = _sendQueue.Dequeue();
                _pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length)); //ArraySegment = 배열의 일부 , 이걸쓰는 이유? = C++과 달리 포인터를 사용할수 없기에 넣고싶은 위치를 표현하기 위해서(0, buff.Length)

            }
            _sendArgs.BufferList = _pendingList;//BufferList는 그저 add로 추가하면 안되고, 다른 리스트에 넣어놨다가 _sendArgs.BufferList = list; 로 대입해줘야함.(이건 document에도 잘안나와서 헷갈릴수있대)

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
                        _sendArgs.BufferList = null; 
                        _pendingList.Clear(); // complete까지 왔다는건 send가 완료됬다는 것이니, sendArgs를 다시쓰기위해 깨끗이 비워준다(초기화).

                        OnSend(_sendArgs.BytesTransferred);

                        if (_sendQueue.Count>0) //보내는동안에 누군가 입력해서 sendQueue가 차있다면, 다시 RegisterSend가 돌아가게하자.                        
                            RegisterSend();//이러면 내가 보내고있는동안에 누군가 예약을해도 문제없이 다시 RegisterSend가 돌아간다.
                       
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

        void RegisterRecv() //send와 형태 맞추기위해 session3에서 수정
        {
            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)
                OnRecvCompleted(null, _recvArgs);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred >0 && args.SocketError==SocketError.Success) //BytesTransferred는 전송받은 데이터 크기를 의미하는데, 상대가 접속을 종료하면 0이 올수도있다. 그렇기에 이런 종료상황을 체크를 해줘야됨
            {
                try //혹시 실패할지도 모르니 확인차 트라이캐치
                {
                    OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
                    RegisterRecv();
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
