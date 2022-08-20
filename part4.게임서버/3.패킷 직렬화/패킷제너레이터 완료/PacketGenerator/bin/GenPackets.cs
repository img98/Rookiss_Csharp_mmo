using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public enum PacketID
{
    PlayerInfoReq = 1,
	Test = 2,
	
}

class PlayerInfoReq 
{
    public byte testByte;
	public long playerId;
	public string name;
	public class Skill
	{
	    public int id;
		public short level;
		public float duration;
		public class Attribute
		{
		    public int att;
		
		    public void Read(ReadOnlySpan<byte> s, ref ushort count)
		    {
		        this.att = BitConverter.ToInt32(s.Slice(count, s.Length - count));
				count += sizeof(int);
		    }
		
		    public bool Write(Span<byte> s, ref ushort count)
		    {
		        bool success = true;
		
		        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.att); 
				count += sizeof(int);
		
		        return success;
		    }
		}
		public List<Attribute> attributes = new List<Attribute>();
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
			count += sizeof(short);
			this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			this.attributes.Clear();
			ushort attributeLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
			count += sizeof(ushort);
			for (int i = 0; i < attributeLen; i++) 
			{
			    Attribute attribute = new Attribute();
			    attribute.Read(s, ref count);
			    attributes.Add(attribute);
			}
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	
	        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id); 
			count += sizeof(int);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level); 
			count += sizeof(short);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration); 
			count += sizeof(float);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.attributes.Count);
			count += sizeof(ushort);
			foreach (Attribute attribute in this.attributes) 
			    success &= attribute.Write(s, ref count);
	
	        return success;
	    }
	}
	public List<Skill> skills = new List<Skill>();

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);     

        this.testByte = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
		this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
		count += nameLen;
		this.skills.Clear();
		ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		for (int i = 0; i < skillLen; i++) 
		{
		    Skill skill = new Skill();
		    skill.Read(s, ref count);
		    skills.Add(skill);
		}

    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0; 
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count); 

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.PlayerInfoReq); 
        count += sizeof(ushort);
        segment.Array[segment.Offset + count] = (byte)this.testByte;
		count += sizeof(byte);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId); 
		count += sizeof(long);
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
		count += sizeof(ushort);
		count += nameLen;
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.skills.Count);
		count += sizeof(ushort);
		foreach (Skill skill in this.skills) 
		    success &= skill.Write(s, ref count);        
        success &= BitConverter.TryWriteBytes(s, count); // 지난번에 말했듯, 얘는 s.size의 바이트를 넣기위한 줄인데 맨위에 있어야할게 일부러 count를 이용하려고 맨 아래로 내려온것. 그러므로 인자는 span s를 그대로 넣어도 이상이 없다.

        if (success == false)
            return null; //실패하면 Array에 null을 담을거니, bool값을 리턴하지 않아도 결과값을 보고 실패했다는걸 알 수 있다.

        return SendBufferHelper.Close(count);
    }
}
class Test 
{
    public int testInt;

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);     

        this.testInt = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);

    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0; 
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count); 

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.Test); 
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.testInt); 
		count += sizeof(int);        
        success &= BitConverter.TryWriteBytes(s, count); // 지난번에 말했듯, 얘는 s.size의 바이트를 넣기위한 줄인데 맨위에 있어야할게 일부러 count를 이용하려고 맨 아래로 내려온것. 그러므로 인자는 span s를 그대로 넣어도 이상이 없다.

        if (success == false)
            return null; //실패하면 Array에 null을 담을거니, bool값을 리턴하지 않아도 결과값을 보고 실패했다는걸 알 수 있다.

        return SendBufferHelper.Close(count);
    }
}
