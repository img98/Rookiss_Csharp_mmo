using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler //패킷이 조립되면 그걸보고 무엇을 호출할지 관리
{
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        S_Chat chatPacket = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;

        //if(chatPacket.playerId==1) //더미클라 수십개에 똑같은 짓을 할건데, 로그가 계속 나오면 보기싫으니, 대표로 1번만 봐서 전달이 잘되는지 확인
            Console.WriteLine(chatPacket.chat);
    }
}
