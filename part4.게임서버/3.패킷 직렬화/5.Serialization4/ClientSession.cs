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
        public string name;

        public struct SkillInfo
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> s, ref ushort count)
            {
                bool success = true;
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
                count += sizeof(float);
                return success;
            }

            public void Read(ReadOnlySpan<byte> s, ref ushort count)
            {
                id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);
                level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
                count += sizeof(short);
                duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
                count += sizeof(float);
            }
        }

        public List<SkillInfo> skills = new List<SkillInfo>();

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
            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += sizeof(long);

            //string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;//지난번에 count늘리는거 깜빡했다.

            //skill List
            skills.Clear(); //새로 전달받은 skill을 넣을 배열(새로 만든 배열은 아님. 위에 list<SkillInfo>) skills를 새로 만든 흔적있다.)인데, 그전에 있던게 남아있으면 안되니 clear
            ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            for (int i = 0; i < skillLen; i++)
            {
                SkillInfo skill = new SkillInfo();
                skill.Read(s, ref count);
                skills.Add(skill);
            }

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

            //string
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort)); //이버전의 GetBytes는 segment에 복사를 해줌과 동시에, name이라는 string의 byte크기를 반환한다! //offset에 ushort의 사이즈를 더해서 nameLen이 들어갈자리를 미리 마련해줌
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);//s의size를 count로 넣기위해 맨아래로 보낸것과 비슷하게, 얘도 nameLen을 구한다음에 응용하려고, name복사단계에서 미리 nameLen들어갈 자리를 남겨주고, 지금 그자리에 들어가는 거다.
            count += sizeof(ushort);
            count += nameLen;

            //skill List
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
            count += sizeof(ushort);
            foreach (SkillInfo skill in skills)
                success &= skill.Write(s, ref count); //skill.write안에 이미 ref count를 늘리는 코드가 있어서, foreach에서 따로 count코드를 적지 않아도 된다.


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
                        Console.WriteLine($"PlayerInfoReq: {p.playerId}, name: {p.name}");

                        foreach(PlayerInfoReq.SkillInfo skill in p.skills)
                        {
                            Console.WriteLine($"Skills ({skill.id}) ({skill.level}) ({skill.duration})");
                        }
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
