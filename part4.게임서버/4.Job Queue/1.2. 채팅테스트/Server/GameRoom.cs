using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameRoom
    {
        List<ClientSession> _sessions = new List<ClientSession>(); //유저들(세션들)을 리스트에 담았다.(사실 딕셔너리로 해도될듯, 아이디를 키로해서)
        object _lock = new object();

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat =  $"{chat} I am {packet.playerId}"; // 전달받은 패킷채트에다가 뒤에 자기 id를 밝히도록, 서버쪽에서 수정했다! 
            ArraySegment<byte> segment = packet.Write(); //segment에 하고싶은말의 패킷정보를 담는다.

            lock(_lock) //위까지는 각 session의 변수들을 다루는거라 상관없지만, 이제부터는 _sessions라는 공유하는 변수를 사용할 것이기에 lock필요
            {
                foreach (ClientSession s in _sessions) //_sessions에 있는 모든 세션(s)들에게
                    s.Send(segment); // 패킷을 모두에게 보낸다.
            }
            //그런데 이렇게 한세션의 말을, 모든 세션들에게 보내는, 한동작마다 락을 잡아주면, 수백의 세션이 말을한다고 할때 쓰레드가 여기 몰리게 될거다. 그렇기에 이렇게 락잡으면 악순환생김
        }

        public void Enter(ClientSession session)
        {
            lock(_lock)
            {
                _sessions.Add(session);
                session.Room = this;
            }
        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
            }
        }
    }
}
