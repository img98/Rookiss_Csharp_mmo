using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } } //Singleton 형식으로 _session하나만 사용하게

        int _sessionId = 0;
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        object _lock = new object();

        public ClientSession Generate()
        {
            lock(_lock)
            {
                int sessionId = ++_sessionId;

                ClientSession session = new ClientSession();
                session.SessionId = sessionId;
                _sessions.Add(sessionId, session); //새롭게 만든 세션과, 그 Id를 딕셔너리에 담는다(나중에 그런 세션들의 관리를 편하게하려고 담는것).

                Console.WriteLine($"Connected : {sessionId}"); //필요없는데 확인차 cw
                
                return session; //만들어진 세션 리턴
            }
        }

        public ClientSession Find(int id)
        {
            lock(_lock)
            {
                ClientSession session = null; // 비어있는(null) 새로운 세션 일단 생성, 여기다가 찾아낸 세션을 복붙할거임.
                _sessions.TryGetValue(id, out session);

                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.SessionId); //_sessions를 딕셔너리로 만든이유가 이것, session의 sessionId를 키로 줘서 쉽고빠르게 삭제가능
            }
        }

    }
}
