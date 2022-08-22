using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
    class PacketFormat
    {
        //{0} 패킷 등록
        public static string managerFormat =
@"using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{{
    #region Singleton 
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instacne {{ get {{ return _instance; }} }}
    #endregion

    PacketManager() //패킷매니저가 생성됨과 동시에 Register
    {{
        Register();
    }}

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>(); //(ushort)protocol이라는 키, Action이라는 밸류
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>(); //여기의 밸류는 Handler의 인자에 형태를 맞춰줬다. 

    public void Register() //어떤 작업을 하는지 등록하는 자동화코드
    {{
{0}
    }}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {{
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
    }}

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new() //Generic을써서 PlayerInfoReq말고도 다른 패킷종류를 사용할수 있다 //대신 T는 IPacket을 상속받고, new가 가능해야한다는 조건달았다.
    {{
        T pkt = new T(); //패킷을 만들고
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Protocol, out action)) //핸들러를 호출해서 action(기능)이 있으면 //참고로 action은 delegate라서 out이 필요하다.
            action.Invoke(session, pkt);
    }}
}}";

        //{0} 패킷 이름
        public static string managerRegisterFormat =
@"        _onRecv.Add((ushort)PacketID.{0}, MakePacket<{0}>);
        _handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);";

        //{0} 패킷 이름/번호 목록
        //{1} 패킷 목록
        public static string fileFormat = //파일 자체에 대한 포맷
@"using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public enum PacketID
{{
    {0}
}}

interface IPacket
{{
	ushort Protocol {{ get; }}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}}

{1}";


        //{0} 패킷 이름
        //{1} 패킷 번호
        public static string packetEnumFormat =
@"{0} = {1},";



        //{0} 패킷 이름
        //{1} 멤버 변수들
        //{2} 멤버 변수 Read
        //{3} 멤버 변수 Write

        public static string packetFormat =
@"class {0} : IPacket
{{
    {1}

	public ushort Protocol {{ get {{ return (ushort)PacketID.{0}; }} }}

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);     

        {2}

    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0; 
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count); 

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.{0}); 
        count += sizeof(ushort);
        {3}        
        success &= BitConverter.TryWriteBytes(s, count); // 지난번에 말했듯, 얘는 s.size의 바이트를 넣기위한 줄인데 맨위에 있어야할게 일부러 count를 이용하려고 맨 아래로 내려온것. 그러므로 인자는 span s를 그대로 넣어도 이상이 없다.

        if (success == false)
            return null; //실패하면 Array에 null을 담을거니, bool값을 리턴하지 않아도 결과값을 보고 실패했다는걸 알 수 있다.

        return SendBufferHelper.Close(count);
    }}
}}
"; //여러줄에 걸쳐 문자열을 정희하고 싶으면 @" "를 쓰면됨(@) //여기 }}"; 이런식으로 끝내면 마지막 괄호 안나오던데 왜지? 꼭 줄바꿈을 해줘야 제대로 찍히네..

        //{0} 리스트 이름 [대문자]
        //{1} 리스트 이름 [소문자]
        //{2} 멤버 변수들
        //{3} 멤버 변수 Read
        //{4} 멤버 변수 Write
        public static string memberListFormat =
@"public class {0}
{{
    {2}

    public void Read(ReadOnlySpan<byte> s, ref ushort count)
    {{
        {3}
    }}

    public bool Write(Span<byte> s, ref ushort count)
    {{
        bool success = true;

        {4}

        return success;
    }}
}}
public List<{0}> {1}s = new List<{0}>();";

        //{0} 변수 형식
        //{1} 변수 이름
        public static string memberFormat =
@"public {0} {1};";

        //{0} 변수 이름
        //{1} To~ 변수 형식
        //{2} 변수 형식
        public static string readFormat =
@"this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
count += sizeof({2});";

        //{0} 변수 이름
        //{1} 변수 형식
        public static string readByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});";

        //{0} 변수 이름
        public static string readStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Len));
count += {0}Len;";

        //{0} 리스트 이름 [대문자]
        //{1} 리스트 이름 [소문자]
        public static string readListFormat =
@"this.{1}s.Clear();
ushort {1}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
for (int i = 0; i < {1}Len; i++) 
{{
    {0} {1} = new {0}();
    {1}.Read(s, ref count);
    {1}s.Add({1});
}}";

        //{0} 변수 이름
        //{1} 변수 형식
        public static string writeFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.{0}); 
count += sizeof({1});";

        //{0} 변수 이름
        //{1} 변수 형식
        public static string writeByteFormat =
@"segment.Array[segment.Offset + count] = (byte)this.{0};
count += sizeof({1});";

        //{0} 변수 이름
        public static string writeStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);
count += sizeof(ushort);
count += {0}Len;";

        //{0} 리스트 이름 [대문자]
        //{1} 리스트 이름 [소문자]
        public static string writeListFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.{1}s.Count);
count += sizeof(ushort);
foreach ({0} {1} in this.{1}s) 
    success &= {1}.Write(s, ref count);";


    }
}
