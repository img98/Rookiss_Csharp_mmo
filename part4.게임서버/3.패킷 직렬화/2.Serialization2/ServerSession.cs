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

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> s)
        {
            ushort count = 0;

            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset); //어쩌피 playerInfoReq클래스에 id, size등이 담겨있을텐데, 여기서 까는 의미가 있나?
            count += 2;
            //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;
            this.playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count)); //악의적인 테러를 막기위한 방법: span을 통해 바이트 크기를 입력하면, 이 범위를 초과하는 값을 파싱할 때 자동으로 에러가 나올것임 = 이걸 exception으로 받아 수정할수있음                       
            count += 8;
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> s = SendBufferHelper.Open(4096);

            ushort count = 0; 
            bool success = true;

            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
            count += 8;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

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

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001 }; //size랑 packetId는 PlayerInfoReq내에 담겨있기에, 굳이 인자로 새로 입력하지않음

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
