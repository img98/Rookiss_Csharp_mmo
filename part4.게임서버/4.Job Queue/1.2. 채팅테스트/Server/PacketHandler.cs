using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler //패킷이 조립되면 그걸보고 무엇을 호출할지 관리
{
    public static void C_ChatHandler(PacketSession session, IPacket packet) //인자로 1. 어떤세션에서 조립되었느냐, 2. 어떤 패킷이냐
    {
        C_Chat chatPacket = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;//전달받은 PacketSession은 더 자세히 살펴보면 ClientSession일 것이다. 그러니 as로 ClientSession이라고 재명명

        if (clientSession.Room == null) //해당 세션에게 Room값이 없으면, 그냥끝낸다.
            return;

        clientSession.Room.Broadcast(clientSession, chatPacket.chat); //인자 = 내가 누군지, 말의 내용
    }
}
