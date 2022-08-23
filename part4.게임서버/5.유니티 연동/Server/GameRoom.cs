using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>(); //유저들(세션들)을 리스트에 담았다.(사실 딕셔너리로 해도될듯, 아이디를 키로해서)        
        JobQueue _jobQueue = new JobQueue(); //잡큐내부 Flush의 lock구조로 인해, GameRoom에서 기능을 구현할때는 lock이 필요없어진다.
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>(); //패킷을 모을 리스트


        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (ClientSession s in _sessions) //_sessions에 있는 모든 세션(s)들에게
                s.Send(_pendingList); // 보낸다.

            //Console.WriteLine($"Flushed {_pendingList.Count} items"); //몇개보냈는지 궁금하니까 count를 한번 해보자
            _pendingList.Clear();
            //어쩌피 잡큐에서 수행될거니, 하나만 돌아간다는게 보장되므로, lock을 걸 필요없다.
        }


        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            //플레이어 추가하고
            _sessions.Add(session);
            session.Room = this;

            //신입에게 모든 플레이어 목록 전송
            S_PlayerList players = new S_PlayerList();
            foreach(ClientSession s in _sessions)
            {
                players.players.Add(new S_PlayerList.Player() //방금위에서 만든 players라는 S_PlayerList패킷 클래스의 players라는 리스트가 내부에 또 있다. 거기다가 Add해준것.(변수이름이 거지같아서 보기힘든것)
                {
                    //S_PlayerList의 속성으로는 isSelf(bool),playerId, posXYZ가 있다.
                    isSelf = (s == session), //지금 세션이 나랑 같은질 판별해서, 플레이어 리스트의 플레이어속성 isSelf에 담아, 자신인지 판별가능
                    playerId = s.SessionId,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ,
                }); //Add(new, {} ); 이런문법은 또 처음보네. 대충 new로만든 리스트에 {}를 넣게다는건 이해된다.
            }
            session.Send(players.Write()); //Broadcast가 아니라, 신입에게만 보내면됨

            //모두에게 신입의 입장을 알림
            S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
            enter.playerId = session.SessionId;
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0; // 실제 게임에서는 좌표가 서버에 저장되어 있겠지만, 지금은 대충 000에서 시작하게하자.
            Broadcast(enter.Write());
        }

        public void Leave(ClientSession session)
        {
            //플레이어 제거하고
            _sessions.Remove(session);

            //모두에게 알린다.
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.SessionId;
            Broadcast(leave.Write());
        }

        public void Move(ClientSession session, C_Move packet)
        {
            //좌표 바꿔주고
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;

            //모두에게 알린다.
            S_BroadcastMove move = new S_BroadcastMove();
            move.playerId = session.SessionId;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;
            Broadcast(move.Write());
        }
    }

}
