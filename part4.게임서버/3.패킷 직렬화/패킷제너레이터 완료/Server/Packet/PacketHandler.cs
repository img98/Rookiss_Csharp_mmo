using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler //패킷이 조립되면 그걸보고 무엇을 호출할지 관리
{
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet) //인자로 1. 어떤세션에서 조립되었느냐, 2. 어떤 패킷이냐
    {
        C_PlayerInfoReq p = packet as C_PlayerInfoReq;

        Console.WriteLine($"PlayerInfoReq: {p.playerId}, name: {p.name}");

        foreach (C_PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"Skills ({skill.id}) ({skill.level}) ({skill.duration})");
        }
    }
}
