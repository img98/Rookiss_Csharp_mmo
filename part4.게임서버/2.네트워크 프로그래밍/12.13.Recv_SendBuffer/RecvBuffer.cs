using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public class RecvBuffer
    {
        // [rw][ ][ ][ ][ ][ ][ ][ ][ ][ ]
        ArraySegment<byte> _buffer;
        int _readPos; //전달받은 패킷이 읽을수있는 단위인지 확인, 읽기시작할 위치
        int _writePos; //패킷이 전달될 위치

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize { get { return _writePos - _readPos; } } //버퍼에 들어가야할, 아직 처리되지 않은 데이터 사이즈
        public int FreeSize { get { return _buffer.Count - _writePos; } } //버퍼에 남은공간

        public ArraySegment<byte> ReadSegment //유효범위의 세그먼트 => 어디부터 읽으면 되는지 (현재까지 받은 데이터중 어디서 어디까지가 유효한지)
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegment //다음에 recv할때 어디서부터가 보내야할지.
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        public void Clean() //버퍼가 꽉차서 못쓰지않게, 중간중간 비워줌
        {
            int dataSize = DataSize;
            if (dataSize == 0) //readPos와 writePos가 정확히 겹치는 경우 = 남은 데이터가 없다
            {
                // 복사없이 커서들의 위치만 리셋
                _readPos = _writePos = 0;
            }
            else
            {
                //남은 데이터가 잇으면 시작위치로 복사시켜줘야됨(있던걸 날려버릴순 없으니)
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize); //복사할 배열, 복사시작할위치, 붙여넣을 배열, 붙여넣기 시작할 위치, 얼마나 복사할지
                _readPos = 0;
                _writePos = dataSize;
            }
        }


        public bool OnRead(int numOfBytes) //데이터 전송이 성공하면 커서 OnRead호출해서 커서위치 옮길거임
        {
            if (numOfBytes > DataSize) //보낸데이터랑 보내야되는 데이터랑 크기보다 작다? 먼가 에러가 있는거임
                return false;

            _readPos += numOfBytes; //별일없다면 readPos 커서 위치 옮기기
            return true;
        }
        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize) //보낸데이터가 남은크기 보다 크다면 먼가 문제있다.
                return false;

            _writePos += numOfBytes; 
            return true;

        }
    }
}
