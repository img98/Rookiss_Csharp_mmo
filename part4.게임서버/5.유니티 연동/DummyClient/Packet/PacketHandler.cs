using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler //패킷이 조립되면 그걸보고 무엇을 호출할지 관리
{
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEnterGame chatPacket = packet as S_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;

        //빌드만 통과할 수 있게 함수만 만들거고, 작업은 유니티 클라이언트 코드에서 할거다.
    }
    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastLeaveGame chatPacket = packet as S_BroadcastLeaveGame;
        ServerSession serverSession = session as ServerSession;

        //빌드만 통과할 수 있게 함수만 만들거고, 작업은 유니티 클라이언트 코드에서 할거다.
    }
    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList chatPacket = packet as S_PlayerList;
        ServerSession serverSession = session as ServerSession;

        //빌드만 통과할 수 있게 함수만 만들거고, 작업은 유니티 클라이언트 코드에서 할거다.
    }
    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastMove chatPacket = packet as S_BroadcastMove;
        ServerSession serverSession = session as ServerSession;

        //빌드만 통과할 수 있게 함수만 만들거고, 작업은 유니티 클라이언트 코드에서 할거다.
    }
}
