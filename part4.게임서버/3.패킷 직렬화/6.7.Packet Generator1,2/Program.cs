using System;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static string genPackets; //실시간으로 만들어지는 패킷 을 여기다 넣어서 관리할것임

        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            using (XmlReader r = XmlReader.Create("PDL.xml", settings))
            {
                r.MoveToContent();

                while(r.Read())
                {
                    if (r.Depth == 1 && r.NodeType == XmlNodeType.Element) 
                        ParsePacket(r);
                    //Console.WriteLine(r.Name + " " + r["name"]); //r.Name은 줄의 타입, r["name"]은 name="~~"에서의 ~~를 의미 => 자동화툴 만들때 필요한건 이게 전부

                    File.WriteAllText("GenPackets.cs", genPackets);
                }

            }

            
        }

        public static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement)
                return;
            if (r.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invalid packet node");
                return;
            }

            string packetName = r["name"];
            if(string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without name");
                return;
            }

            Tuple<string, string, string> t = ParseMembers(r);
            genPackets += string.Format(PacketFormat.packetFormat, packetName, t.Item1, t.Item2, t.Item3);
        }

        //{1} 멤버 변수들
        //{2} 멤버 변수 Read
        //{3} 멤버 변수 Write
        public static Tuple<string, string, string> ParseMembers(XmlReader r)
        {
            string packetName = r["name"];
            int depth = r.Depth + 1; //우리가 파싱하고 싶은애들은 packet의 안쪽이니까 depth가 2여야됨. (0=PDL, 1=packet(=pdl의 내용물들) 2=id,name,skill...(packet의내용물들))

            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            while (r.Read())
            {
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return null;
                }

                if (string.IsNullOrEmpty(memberCode) == false)
                    memberCode += Environment.NewLine; //줄바꿈, 이게없으면 생성되는 GenPackets.cs의 모든 코드가 한줄이 되서나옴.
                if (string.IsNullOrEmpty(readCode) == false)
                    readCode += Environment.NewLine;
                if (string.IsNullOrEmpty(writeCode) == false)
                    writeCode += Environment.NewLine;

                string memberType = r.Name.ToLower();
                switch(memberType)
                {
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double": //string전까지의 이런 고정사이즈 타입들은 memberFormat, readFormat, writeFormat으로 만들수있으니, 여기서 break한번해주자.
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType); //{1} To~ 변수형식 같은경우에는, 대입할 방법이 마땅치않아 직접함수를 만들었다.
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        Tuple<string, string, string> t = ParseList(r);
                        memberCode += t.Item1; 
                        readCode += t.Item2;
                        writeCode += t.Item3;
                        break;
                    default:
                        break;
                }
            }

            memberCode = memberCode.Replace("\n", "\n\t"); //memberCode가 여러줄이라서 줄바꿈이 있다면, 줄바꿈 뒤에 탭을 눌러서 줄맞춤을 해줘라 라는 의미.
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memberCode, readCode, writeCode);

        }

        public static Tuple<string, string, string> ParseList(XmlReader r)
        {
            string listName = r["name"];
            if (string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("List without name");
                return null;
            }

            Tuple<string, string, string> t = ParseMembers(r);
            string memberCode = string.Format(PacketFormat.memberListFormat, FirstCharToUpper(listName), FirstCharToLower(listName), t.Item1, t.Item2, t.Item3);
            string readCode = string.Format(PacketFormat.readListFormat, FirstCharToUpper(listName), FirstCharToLower(listName));
            string writeCode = string.Format(PacketFormat.writeListFormat, FirstCharToUpper(listName), FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static string ToMemberType(string memberType)
        {
            switch(memberType)
            {
                case "bool":
                    return "ToBoolean";
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                default:
                    return "";

            }
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToLower() + input.Substring(1);
        }
    }
}
