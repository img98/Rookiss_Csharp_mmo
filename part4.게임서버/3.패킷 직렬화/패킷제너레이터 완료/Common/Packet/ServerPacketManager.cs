using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
    #region Singleton 
    static PacketManager _instance;
    public static PacketManager Instacne
    {
        get
        {
            if (_instance == null)
                _instance = new PacketManager();
            return _instance;
        }
    }
    #endregion

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>(); //(ushort)protocol이라는 키, Action이라는 밸류
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>(); //여기의 밸류는 Handler의 인자에 형태를 맞춰줬다. 

    public void Register() //어떤 작업을 하는지 등록하는 자동화코드
    {
        _onRecv.Add((ushort)PacketID.C_PlayerInfoReq, MakePacket<C_PlayerInfoReq>);
        _handler.Add((ushort)PacketID.C_PlayerInfoReq, PacketHandler.C_PlayerInfoReqHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer);
        // _OnRecv 딕셔너리로 검색해서 action이 있다(=핸들러를 등록해놨다)면 그녀석을 호출할거고, 호출하는 순간 아래 MakePacket이 호출됨.
        // 왜냐면 우리는 _OnRecv에 등록하는 과정에서 인자로 MakePacket을 사용했기 때문이다. 그러면 MakePacket함수에서 _handler를 호출해서 action을 찾으면 Invoke하게된다. 그러면 최종적으로 PacketHandler.cs에 있는 코드가 호출된다.
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new() //Generic을써서 PlayerInfoReq말고도 다른 패킷종류를 사용할수 있다 //대신 T는 IPacket을 상속받고, new가 가능해야한다는 조건달았다.
    {
        T pkt = new T(); //패킷을 만들고
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Protocol, out action)) //핸들러를 호출해서 action(기능)이 있으면 //참고로 action은 delegate라서 out이 필요하다.
            action.Invoke(session, pkt);
    }
}