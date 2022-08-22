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

        //잡큐에 Broadcast하라는 명령을 넣었기에, 사실상 위 주문과 실행사이에는 텀이 있다. 그 텀동안에 세션이 disconnect되면 Room이 null이 되버리기에, 참조 과정에서 Room을 찾지못해 에러가 발생함.
        //이걸막기위해 Room을 복사한 room을 사용해 push와 broadcast를 호출해야함. Room은 null이 되었어도, room은 참조가 가능하기 때문이다.
        GameRoom room = clientSession.Room;
        room.Push(() => room.Broadcast(clientSession, chatPacket.chat));
        
    }
}
