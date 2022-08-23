using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
    #region Singleton 
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instacne { get { return _instance; } }
    #endregion

    PacketManager() //패킷매니저가 생성됨과 동시에 Register
    {
        Register();
    }

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>,IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>,IPacket>>(); //이젠 Recv를한뒤의 과정이아닌, MakePacket이라는, 만들어주는함수이니, 이름도 MakeFunc로 바꿈.
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>(); //여기의 밸류는 Handler의 인자에 형태를 맞춰줬다. 

    public void Register() //어떤 작업을 하는지 등록하는 자동화코드
    {
        _makeFunc.Add((ushort)PacketID.C_LeaveGame, MakePacket<C_LeaveGame>);
        _handler.Add((ushort)PacketID.C_LeaveGame, PacketHandler.C_LeaveGameHandler);
        _makeFunc.Add((ushort)PacketID.C_Move, MakePacket<C_Move>);
        _handler.Add((ushort)PacketID.C_Move, PacketHandler.C_MoveHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (_makeFunc.TryGetValue(id, out func))
        {
            IPacket packet = func.Invoke(session, buffer); //이젠 MakePacket이 return을 하므로 packet에 해당 패킷을 담을수있다.
            if (onRecvCallback != null) //어떤기능으로 처리할지 정해놨다면
                onRecvCallback.Invoke(session, packet); //걔한테 패킷 알려줘서 처리 (핸들러가 아닌 기능으로도 보낼수있다는 뜻인듯?) //뒤에보니, ServerSession.cs에서 무조건 패킷큐에 넣으라는 명령을 넣어줬기에, 그명령용으로 만든 임시코드인듯
            else
                HandlePacket(session, packet); //아니면 원래하듯이 바로 핸들러로 보냄.
        }
        // <_makeFunc로 바뀐 _OnRecv에 대한 설명. 그런데 이거라도 읽어야, 그냥 func를 호출했는데 왜 MakePacket이 나오는지 이해가 될거라서 남겨둠>
        // _OnRecv 딕셔너리로 검색해서 action이 있다(=핸들러를 등록해놨다)면 그녀석을 호출할거고, 호출하는 순간 아래 MakePacket이 호출됨.
        // 왜냐면 우리는 _OnRecv에 등록하는 과정에서 인자로 MakePacket을 사용했기 때문이다. 그러면 MakePacket함수에서 _handler를 호출해서 action을 찾으면 Invoke하게된다. 그러면 최종적으로 PacketHandler.cs에 있는 코드가 호출된다.

    }

    //MakePackeet을 두파트로 분리함. 1.MakePacket: 패킷을 만드는파트 / 2.HandlePacket: 핸들러에서 기능을 호출하는 파트
    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new() //Generic을써서 PlayerInfoReq말고도 다른 패킷종류를 사용할수 있다 //대신 T는 IPacket을 상속받고, new가 가능해야한다는 조건달았다.
    {
        T pkt = new T(); //패킷을 만들고
        pkt.Read(buffer);
        return pkt;
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.Protocol, out action)) //핸들러를 호출해서 action(기능)이 있으면 //참고로 action은 delegate라서 out이 필요하다.
            action.Invoke(session, packet);
    }

}