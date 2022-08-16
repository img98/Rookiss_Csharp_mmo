using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;

namespace Server
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
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId); //span = 범위를 찝어주는 역할
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
        PlayerInfoOk = 2,
    }

    class ClientSession : PacketSession //세션이라는게 결국 대리자(식당 대리인)의 개념이기에, 서버쪽에 클라세션이 있는게 맞다.
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            //Packet packet = new Packet() { size = 100, packetId = 10 };

            /*
            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(packet.size);
            byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset+buffer.Length, buffer2.Length);
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length); //다썻으니 이만큼 썻다고 알려주면, 보내야될 범위 리턴함 = sendBuff
            */

            //Send(sendBuff);
            Thread.Sleep(5000);
            Disconnect();
        }

        //기존의 OnRecv는 패킷세션만들면서 seal해줬으니 아예 지워버림
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            
            switch((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);
                        Console.WriteLine($"PlayerInfoReq: {p.playerId}");
                    }
                    break;
            }


            Console.WriteLine($"RecvPacketId: {id}, Size: {size}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }


        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
