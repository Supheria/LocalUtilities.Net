using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalUtilities.Net.Sockets
{
    /// <summary>
    /// Socket��������
    /// </summary>
    public class SocketBufferedStream : Stream
    {
        private int _BufferSize;
        private long _Length;
        private long _Position;
        private List<byte[]> _Buffer;
        private int _CurrentCursor;
        private int _CurrentPosition;
        private int _DirtyLength;

        /// <summary>
        /// ��4096�ֽڴ�Сʵ����һ��Socket��������
        /// </summary>
        public SocketBufferedStream() : this(4096) { }

        /// <summary>
        /// ��ʼ��Socket��������
        /// </summary>
        /// <param name="bufferSize">������С��</param>
        public SocketBufferedStream(int bufferSize)
        {
            if (bufferSize < 1)
                throw new ArgumentOutOfRangeException("bufferSize", "Buffer size can not less than 1.");
            _BufferSize = bufferSize;
            _Buffer = new List<byte[]>();
            _Buffer.Add(new byte[bufferSize]);
        }

        /// <summary>
        /// ��ȡ�Ƿ�֧�ֶ�ȡ��
        /// ��Զ����true��
        /// </summary>
        public override bool CanRead { get { return true; } }

        /// <summary>
        /// ��ȡ�Ƿ�֧�ֲ��ҡ�
        /// ��Զ����true��
        /// </summary>
        public override bool CanSeek { get { return true; } }

        /// <summary>
        /// ��ȡ�Ƿ�֧��д�롣
        /// ��Զ����true��
        /// </summary>
        public override bool CanWrite { get { return true; } }

        /// <summary>
        /// ˢ�»��档��Ч�ķ�����
        /// </summary>
        public override void Flush() { }

        /// <summary>
        /// ��ȡ������Ч���ȡ�
        /// </summary>
        public override long Length { get { return _Length; } }

        /// <summary>
        /// ��ȡ���浱ǰλ�á�
        /// </summary>
        public override long Position
        {
            get
            { return _Position; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > _Length)
                    value = _Length;
                _Position = value;
                _CurrentCursor = (int)Math.Floor((value + _DirtyLength) / (double)_BufferSize);
                _CurrentPosition = (int)((value + _DirtyLength) % _BufferSize);
            }
        }

        /// <summary>
        /// �ӻ����ȡ���ݡ�
        /// </summary>
        /// <param name="buffer">������</param>
        /// <param name="offset">ƫ������</param>
        /// <param name="count">Ҫ��ȡ���ֽ�������</param>
        /// <returns>�ɹ���ȡ���ֽ�����</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > _Length - Position)
                count = (int)(_Length - Position);
            int result = count;
            while (count > 0)
            {
                int size = count;
                byte[] currentBuffer = _Buffer[_CurrentCursor];
                if (size > _BufferSize - _CurrentPosition)
                    size = _BufferSize - _CurrentPosition;
                Array.Copy(currentBuffer, _CurrentPosition, buffer, offset, size);
                count -= size;
                offset += size;
                _CurrentPosition += size;
                Position += size;
                if (_CurrentPosition == _BufferSize)
                {
                    _Buffer.Add(new byte[_BufferSize]);
                    _CurrentCursor++;
                    _CurrentPosition = 0;
                }
            }
            return result;
        }

        /// <summary>
        /// �ӻ����ȡ�����ֽڡ�
        /// </summary>
        /// <returns>û�������򷵻�-1��</returns>
        public override int ReadByte()
        {
            if (_Position == _Length)
                return -1;
            byte[] buffer = _Buffer[_CurrentCursor];
            byte value = buffer[_CurrentPosition];
            _CurrentPosition++;
            _Position++;
            if (_CurrentPosition == _BufferSize)
            {
                _CurrentCursor++;
                _CurrentPosition = 0;
            }
            return value;
        }

        /// <summary>
        /// Ѱ�һ���λ�á�
        /// </summary>
        /// <param name="offset">ƫ��λ�á�</param>
        /// <param name="origin">���λ�á�</param>
        /// <returns>�µ���λ�á�</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _Position = offset;
                    break;
                case SeekOrigin.Current:
                    _Position += offset;
                    break;
                case SeekOrigin.End:
                    _Position = Length + offset;
                    break;
            }
            if (_Position > _Length)
                _Position = _Length;
            if (_Position < 0)
                _Position = 0;
            _CurrentCursor = (int)Math.Floor((_Position + _DirtyLength) / (double)_BufferSize);
            _CurrentPosition = (int)((_Position + _DirtyLength) % _BufferSize);
            return _Position;
        }

        /// <summary>
        /// ���û����ܴ�С��
        /// </summary>
        /// <param name="value">�����С��</param>
        public override void SetLength(long value)
        {
            int bufferSize = (int)Math.Ceiling((value + _DirtyLength) / (double)_BufferSize);
            for (int i = _Buffer.Count; i < bufferSize; i++)
                _Buffer.Add(new byte[_BufferSize]);
            _Length = value;
        }

        /// <summary>
        /// �򻺴�д�����ݡ�
        /// </summary>
        /// <param name="buffer">��������</param>
        /// <param name="offset">ƫ������</param>
        /// <param name="count">�ֽ�����</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                int size = count;
                byte[] currentBuffer = _Buffer[_CurrentCursor];
                if (size > _BufferSize - _CurrentPosition)
                    size = _BufferSize - _CurrentPosition;
                Array.Copy(buffer, offset, currentBuffer, _CurrentPosition, size);
                count -= size;
                offset += size;
                _CurrentPosition += size;
                _Position += size;
                if (_Position > _Length)
                    _Length = _Position;
                if (_CurrentPosition == _BufferSize)
                {
                    if (_CurrentCursor + 1 == _Buffer.Count)
                        _Buffer.Add(new byte[_BufferSize]);
                    _CurrentCursor++;
                    _CurrentPosition = 0;
                }
            }
        }

        /// <summary>
        /// �򻺴�д�뵥���ֽڡ�
        /// </summary>
        /// <param name="value">���ݡ�</param>
        public override void WriteByte(byte value)
        {
            byte[] buffer = _Buffer[_CurrentCursor];
            buffer[_CurrentPosition] = value;
            _CurrentPosition++;
            _Position++;
            if (_Position > _Length)
                _Length = _Position;
            if (_CurrentPosition == _BufferSize)
            {
                if (_CurrentCursor + 1 == _Buffer.Count)
                    _Buffer.Add(new byte[_BufferSize]);
                _CurrentCursor++;
                _CurrentPosition = 0;
            }
        }

        /// <summary>
        /// ���������������ת��Ϊ�ֽ����ݡ�
        /// </summary>
        /// <returns></returns>
        public virtual byte[] ToArray()
        {
            byte[] data = new byte[_Length];
            long offset = 0;
            for (int i = 0; i < _Buffer.Count; i++)
            {
                byte[] buffer = _Buffer[i];
                if (i == _Buffer.Count - 1)
                {
                    Array.Copy(buffer, 0, data, offset, _Length % _BufferSize);
                }
                else
                {
                    if (i == 0)
                    {
                        Array.Copy(buffer, _DirtyLength, data, offset, _BufferSize - _DirtyLength);
                        offset += _BufferSize - _DirtyLength;
                    }
                    else
                    {
                        buffer.CopyTo(data, offset);
                        offset += _BufferSize;
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// �����ֽ�λ�á�
        /// </summary>
        /// <param name="value">Ҫ���ҵ��ֽڡ�</param>
        /// <returns>�ֽ�����λ�ã��Ҳ�������-1��</returns>
        public virtual int IndexOf(byte value)
        {
            return IndexOf(value, 0);
        }

        /// <summary>
        /// �����ֽ�λ�á�
        /// </summary>
        /// <param name="value">Ҫ���ҵ��ֽڡ�</param>
        /// <param name="startIndex">��ʼ���ҵ�λ�á�</param>
        /// <returns>�ֽ�����λ�ã��Ҳ�������-1��</returns>
        public virtual int IndexOf(byte value, int startIndex)
        {
            if (startIndex < 0 || startIndex >= _Length)
                throw new ArgumentOutOfRangeException("startIndex");
            int bufferIndex = (int)Math.Floor((startIndex + _DirtyLength) / (double)_BufferSize);
            int bufferOffset = (startIndex + _DirtyLength) % _BufferSize;
            int position = startIndex;
            for (int i = bufferIndex; i < _Buffer.Count; i++)
            {
                byte[] buffer = _Buffer[i];
                while (bufferOffset < _BufferSize)
                {
                    if (buffer[bufferOffset] == value)
                        return position;
                    position++;
                    if (position == Length)
                        goto End;
                    bufferOffset++;
                }
                bufferOffset = 0;
            }
        End:
            return -1;
        }

        /// <summary>
        /// �����ֽ�λ�á�
        /// </summary>
        /// <param name="values">Ҫ���ҵ����ݡ�</param>
        /// <returns>��������λ�ã��Ҳ�������-1��</returns>
        public virtual int IndexOfValues(params byte[] values)
        {
            return IndexOfValues(values, 0);
        }

        /// <summary>
        /// �����ֽ�λ�á�
        /// </summary>
        /// <param name="values">Ҫ���ҵ����ݡ�</param>
        /// <param name="startIndex">��ʼ���ҵ�λ�á�</param>
        /// <returns>��������λ�ã��Ҳ�������-1��</returns>
        public virtual int IndexOfValues(byte[] values, int startIndex)
        {
            if (startIndex < 0 || startIndex >= _Length)
                throw new ArgumentOutOfRangeException("startIndex");
            int bufferIndex = (int)Math.Floor((startIndex + _DirtyLength) / (double)_BufferSize);
            int bufferOffset = (startIndex + _DirtyLength) % _BufferSize;
            int position = startIndex;
            int correct = 0;
            for (int i = bufferIndex; i < _Buffer.Count; i++)
            {
                byte[] buffer = _Buffer[i];
                while (bufferOffset < _BufferSize)
                {
                    if (buffer[bufferOffset] == values[correct])
                    {
                        correct++;
                        if (correct == values.Length)
                            return position - correct + 1;
                    }
                    else
                        correct = 0;
                    position++;
                    if (position == Length)
                        goto End;
                    bufferOffset++;
                }
                bufferOffset = 0;
            }
        End:
            return -1;
        }

        /// <summary>
        /// �رջ���������ͬ��Clear��
        /// </summary>
        public override void Close()
        {
            Clear();
        }

        /// <summary>
        /// ��ջ�������
        /// </summary>
        public virtual void Clear()
        {
            _Buffer.RemoveRange(1, _Buffer.Count - 1);
            _CurrentCursor = 0;
            _CurrentPosition = 0;
            _Length = 0;
            _Position = 0;
            _DirtyLength = 0;
        }
        
        /// <summary>
        /// ���û���λ�á�
        /// �÷����Ὣ����ĵ�ǰλ��֮ǰ������ɾ������ǰλ�ñ�Ϊ0��
        /// </summary>
        public virtual void ResetPosition()
        {
            _Buffer.RemoveRange(0, _CurrentCursor);
            _CurrentCursor = 0;
            _DirtyLength = _CurrentPosition;
            _Length -= _Position;
            _Position = 0;
        }

        /// <summary>
        /// �ͷŻ�������
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            _Buffer.Clear();
            _Buffer = null;
            base.Dispose(disposing);
        }
    }
}
