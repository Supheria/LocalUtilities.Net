using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net;

/// <summary>
/// 由于是异步发送，有可能接收到两个命令，写入了两次返回，发送需要等待上一次回调才发下一次的响应
/// </summary>
/// <param name="bufferSize"></param>
public class AsyncSendBufferManager(int bufferSize)
{
    public DynamicBufferManager DynamicBuffer { get; } = new DynamicBufferManager(bufferSize);

    record SendBufferPack
    {
        public int Offset { get; set; } = 0;

        public int Count { get; set; } = 0;
    }

    List<SendBufferPack> SendPackList { get; } = [];

    SendBufferPack SendPack { get; } = new();

    public void StartPack()
    {
        SendPack.Offset = DynamicBuffer.DataCount;
        SendPack.Count = 0;
    }

    public void EndPack()
    {
        SendPack.Count = DynamicBuffer.DataCount - SendPack.Offset;
        SendPackList.Add(SendPack);
    }

    public bool GetFirstPack(out int offset, out int count)
    {
        offset = 0; // m_sendBufferList[0].Offset;清除了第一个包后，后续的包往前移，因此Offset都为0
        count = 0;
        if (SendPackList.Count <= 0)
            return false;
        count = SendPackList[0].Count;
        return true;
    }

    public bool ClearFirstPack()
    {
        if (SendPackList.Count <= 0)
            return false;
        int count = SendPackList[0].Count;
        DynamicBuffer.Clear(count);
        SendPackList.RemoveAt(0);
        return true;
    }

    public void ClearPacks()
    {
        SendPackList.Clear();
        DynamicBuffer.Clear(DynamicBuffer.DataCount);
    }
}
