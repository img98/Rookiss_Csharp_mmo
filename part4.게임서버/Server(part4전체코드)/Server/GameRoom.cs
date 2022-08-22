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

            Console.WriteLine($"Flushed {_pendingList.Count} items"); //몇개보냈는지 궁금하니까 count를 한번 해보자
            _pendingList.Clear();
            //어쩌피 잡큐에서 수행될거니, 하나만 돌아간다는게 보장되므로, lock을 걸 필요없다.
        }


        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = $"{chat} I am {packet.playerId}"; // 전달받은 패킷채트에다가 뒤에 자기 id를 밝히도록, 서버쪽에서 수정했다! 
            ArraySegment<byte> segment = packet.Write(); //segment에 하고싶은말의 패킷정보를 담는다.

            _pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
        }
    }

}
