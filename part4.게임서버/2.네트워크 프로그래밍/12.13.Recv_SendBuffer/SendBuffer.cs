using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class SendBufferHelper
    {//헬퍼를 만든이유는, 쓰레드 로컬을 사용하여, 컨텐츠끼리의 경합을 없애기 위함이다.
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

        public static int ChunkSize { get; set; } = 4096 * 100;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuffer.Value == null)//지금 처음 쓰는거라면
                CurrentBuffer.Value = new SendBuffer(ChunkSize); // SendBuffer만들어주며 시작

            if (CurrentBuffer.Value.FreeSize < reserveSize) //꽉차서 못쓰겠다면
                CurrentBuffer.Value = new SendBuffer(ChunkSize); //새로운 SendBuffer를 만들어서 쓰겠다. (sendBuffer는 내용이 크니까, 단순히 내가 다썻다고 해도 다른곳에서 참조하고있을수 있어서, recv버퍼마냥 쉽게 clean해줄수없다.)

            return CurrentBuffer.Value.Open(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }

    public class SendBuffer
    {
        // [ ][ ][ ][ ][ ][ ][ ][ ][ ][ ]
        byte[] _buffer;
        int _usedSize = 0; //recv버퍼의 writePos와 유사한 용도

        public int FreeSize { get { return _buffer.Length - _usedSize; } }

        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }

        public ArraySegment<byte> Open(int reserveSize) //내가 버퍼를 얼마만큼 쓸건지 알려주면, 어디 쓰라고return
        {
            if (reserveSize > FreeSize)
                return null; //남은거 없으니 돌아가

            return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
        }

        public ArraySegment<byte> Close(int usedSize) //닫을때 내가 얼마나 썻다고 알려주면
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize); //사용한만큼 채워주고
            _usedSize += usedSize; //_usedSize 커서 위치 옮기기
            return segment; //보낸 범위 return
            //Open과 달리 segment를 return에서 선언하지 않은 이유는, 채움과 return사이에 _usedSize의 커서위치를 옮겨줘야했기때문
        }
    }
}
