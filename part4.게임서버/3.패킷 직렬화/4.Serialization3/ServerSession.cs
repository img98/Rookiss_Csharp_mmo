using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCore;

namespace DummyClient
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;
        public string name;

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;
            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            count += sizeof(ushort);
            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length-count));
            count += sizeof(long);

            //string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096); //ArraySegment란 배열을 새로 만들지않고, 특정데이터를 참조할수있다.(쉽게말해 배열을 새로 안만들고 배열처럼 사용)

            ushort count = 0; 
            bool success = true;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count); //TryWriteBytes에서 매번 new Span...하드코딩이 보기싫기도하고, 더 효율적이기도한 slice를 사용해보자.

            count += sizeof(ushort);//일단 size크기만큼 자리비워두기
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.packetId); // slice는 span의 기능중 하나로, 말그대로 span을 토막내서 일정부분을 추출하는것임. 단 실질적으로 s에 변화가 있는건 아니고, slice해서 새로운 복제품을 추출하는것 // Slice(시작위치, 길이)
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId); 
            count += sizeof(long);

            //string len => [2]
            //byte []
            /*
            ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name); //GetByteCount는 string을 바이트배열로 변환하면 몇바이트일지 계산해줌 //GetByte는 string을 받으면 그걸 바이트배열로 변환해줌.
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(this.name), 0, segment.Array, count, nameLen);
            count += nameLen;
            */ //그런데 이렇게 nameLen따로구하고 그만큼의 배열을 segment에 복사해주는, 2번의 행동을, 한번으로 줄일순없을까? =>GetBytes의 다른 버전을 사용하면 가능하다.

            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort)); //이버전의 GetBytes는 segment에 복사를 해줌과 동시에, name이라는 string의 byte크기를 반환한다! //offset에 ushort의 사이즈를 더해서 nameLen이 들어갈자리를 미리 마련해줌
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);//s의size를 count로 넣기위해 맨아래로 보낸것과 비슷하게, 얘도 nameLen을 구한다음에 응용하려고, name복사단계에서 미리 nameLen들어갈 자리를 남겨주고, 지금 그자리에 들어가는 거다.
            count += sizeof(ushort);
            count += nameLen;
            success &= BitConverter.TryWriteBytes(s, count); // 지난번에 말했듯, 얘는 s.size의 바이트를 넣기위한 줄인데 맨위에 있어야할게 일부러 count를 이용하려고 맨 아래로 내려온것. 그러므로 인자는 span s를 그대로 넣어도 이상이 없다.

            if (success == false)
                return null; //실패하면 Array에 null을 담을거니, bool값을 리턴하지 않아도 결과값을 보고 실패했다는걸 알 수 있다.

            return SendBufferHelper.Close(count);
        }
    } //플레이어 정보를 알고싶다는 요청할때 쓸 패킷 클래스

    //PlayerInfoOk 는 일단 연습에서 안쓸거라 일단 삭제

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

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001, name = "ABCD" };

            //보낸다
            //for (int i = 0; i < 5; i++) 연습으로 한번만 보낼거라 임시 주석
            {
                ArraySegment<byte> s = packet.Write();
                if (s != null)
                    Send(s);
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
