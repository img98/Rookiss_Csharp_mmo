using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCore;

namespace DummyClient
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;
    } //플레이어 정보를 알고싶다는 요청할때 쓸 패킷 클래스
    class PlayerInfoOk : Packet
    {
        public int hp;
        public int attack;
    } //위 요청에 응답할 대답 패킷

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk =2,
    }

    class ServerSession : Session //세션이라는게 결국 대리자(식당 대리인)의 개념이기에, 클라쪽에 서버세션이 있는게 맞다.
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { packetId = (ushort)PacketID.PlayerInfoReq, playerId = 1001 }; //보낼 패킷을 생성, 그런데 정보를 알고싶다는 클래스의 패킷이네? => 이게 서버에 플레이어 정보를 알고싶다는 요청을 만드는것임! //참고로 사이즈는 나중에 버퍼를 다채우고나서 넣어줘야지 자동화시키기 편함.

            //보낸다
            //for (int i = 0; i < 5; i++) 연습으로 한번만 보낼거라 임시 주석
            {
                ArraySegment<byte> s = SendBufferHelper.Open(4096);

                // 패킷 파싱단계에서 전에는 BitConverter.GetBytes를 통해 각각의 바이트를 알아와 새로운 임시버퍼를 만들고, 이것을 전달할 버퍼(s)에 담는것으로 안정적이게 전달버퍼를 만들수 있었다. 하지만 안전을 추구하다보니 리소스손실이 있음(새로운 임시버퍼를 만들어야하는것)
                // 한번에 전달버퍼에 패킷내용을 담을수 없을까? => 여러방법이 있는데, 그중 하나로 아래 TryWriteBytes를 사용하자!

                ushort count = 0; //지금까지 버퍼에 몇바이트나 밀어넣었는지 추적. ex)사이즈(2) + 패킷ID(2) + ...
                bool success = true;
               
                count += 2;// 사이즈가 버퍼 맨앞에 오긴하는데 그걸 자동화로 채우려면 count를 전부 계산하고 넣는게 편하다. 그래서 size칸은 자리만 맡아놓고(count+=2) 마지막에 그자리를 채울것임.
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.packetId); // new span에서 span은 설명없음. 그냥 TryWriteBytes에 필요인자로 써있길래 사용함
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.playerId); //시작위치는offset에서 count만큼 뒤이니 + count, 넣은용량(s.Count - count)은 앞에서 하나 이미 들어갔으니 count만큼 줄어야되니 -count !
                count += 8;
                success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count); //지금까지 더해온 count = packet.size
                //근데 이게 유니티에서 될지는 모름. 그래서 여러방법을 다 알면 좋다.

                ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);

                if(success) // TryWriteBytes는 bool값을 리턴함. 즉, 괄호로 넣은 임무가 성공적으로 완료해야지 true를 리턴한다는것, 그렇기에 success(true) && TryWriteBytes를 통해 한 임무라도 실패하면 false를 얻어낼수있음
                    Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");

            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
