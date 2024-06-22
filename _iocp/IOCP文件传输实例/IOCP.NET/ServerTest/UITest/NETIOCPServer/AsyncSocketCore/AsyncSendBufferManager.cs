using System.Collections.Generic;
using Net;

namespace Net
{
    /// <summary>
    /// 由于是异步发送，有可能接收到两个命令，写入了两次返回，发送需要等待上一次回调才发下一次的响应
    /// </summary>
    /// <param name="bufferSize"></param>
    public class AsyncSendBufferManager(int bufferSize)
    {
        public DynamicBufferManager DynamicBuffer { get; } = new(bufferSize);

        record SendBufferPacket
        {
            public int Offset { get; set; } = 0;

            public int Count { get; set; } = 0;
        }

        List<SendBufferPacket> SendPacketList { get; } = [];

        SendBufferPacket SendPacket { get; } = new();

        public void StartPacket()
        {
            SendPacket.Offset = DynamicBuffer.DataCount;
            SendPacket.Count = 0;
        }

        public void EndPacket()
        {
            SendPacket.Count = DynamicBuffer.DataCount - SendPacket.Offset;
            SendPacketList.Add(SendPacket);
        }

        public bool GetFirstPacket(out int offset, out int count)
        {
            // m_sendBufferList[0].Offset;清除了第一个包后，后续的包往前移，因此Offset都为0
            offset = 0;
            count = 0;
            if (SendPacketList.Count <= 0)
                return false;
            count = SendPacketList[0].Count;
            return true;
        }

        public bool ClearFirstPacket()
        {
            if (SendPacketList.Count <= 0)
                return false;
            DynamicBuffer.Clear(SendPacketList[0].Count);
            SendPacketList.RemoveAt(0);
            return true;
        }

        public void ClearPacket()
        {
            SendPacketList.Clear();
            DynamicBuffer.Clear(DynamicBuffer.DataCount);
        }
    }
}
