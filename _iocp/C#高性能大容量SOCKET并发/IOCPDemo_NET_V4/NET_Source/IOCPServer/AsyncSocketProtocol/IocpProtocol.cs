using System.Text;

namespace Net;

/// <summary>
/// 异步Socket调用对象，所有的协议处理都从本类继承
/// </summary>
public abstract partial class IocpProtocol(IocpProtocolTypes type, IocpServer server, AsyncUserToken userToken) : IDisposable
{
    public IocpProtocolTypes Type { get; } = type;

    protected IocpServer Server { get; } = server;

    protected AsyncUserToken UserToken { get; } = userToken;

    /// <summary>
    /// 长度是否使用网络字节顺序
    /// </summary>
    public bool UseNetByteOrder { get; set; } = false;

    /// <summary>
    /// 协议解析器，用来解析客户端接收到的命令
    /// </summary>
    protected CommandParser CommandParser { get; } = new();

    /// <summary>
    /// 协议组装器，用来组织服务端返回的命令
    /// </summary>
    protected CommandComposer CommandComposer { get; } = new();

    /// <summary>
    /// 标识是否有发送异步事件
    /// </summary>
    protected bool IsSendingAsync { get; set; } = false;

    public DateTime ConnectTime { get; } = DateTime.UtcNow;

    public DateTime ActiveTime { get; protected set; } = DateTime.UtcNow;

    public bool ProcessReceive(byte[] buffer, int offset, int count) //接收异步事件返回的数据，用于对数据进行缓存和分包
    {
        ActiveTime = DateTime.UtcNow;
        UserToken.ReceiveBuffer.WriteBuffer(buffer, offset, count);
        while (UserToken.ReceiveBuffer.DataCount > sizeof(int))
        {
            //按照长度分包
            int packetLength = BitConverter.ToInt32(UserToken.ReceiveBuffer.Buffer, 0); //获取包长度
            if (UseNetByteOrder)
                packetLength = System.Net.IPAddress.NetworkToHostOrder(packetLength); //把网络字节顺序转为本地字节顺序

            // 最大Buffer异常保护
            if ((packetLength > 10 * 1024 * 1024) | (UserToken.ReceiveBuffer.DataCount > 10 * 1024 * 1024)) 
                return false;
            // 收到的数据没有达到包长度，继续接收
            if ((UserToken.ReceiveBuffer.DataCount - sizeof(int)) < packetLength) 
                return true;
            if (AnalyzePack(UserToken.ReceiveBuffer.Buffer, sizeof(int), packetLength))
                UserToken.ReceiveBuffer.Clear(packetLength + sizeof(int)); //从缓存中清理
            else
                return false;
        }
        return true;
    }

    public virtual bool AnalyzePack(byte[] buffer, int offset, int count) //处理分完包后的数据，把命令和数据分开，并对命令进行解析
    {
        if (count < sizeof(int))
            return false;
        int commandLen = BitConverter.ToInt32(buffer, offset); //取出命令长度
        string tmpStr = Encoding.UTF8.GetString(buffer, offset + sizeof(int), commandLen);
        if (!CommandParser.DecodeProtocolText(tmpStr)) //解析命令
          return false;
        return ProcessCommand(buffer, offset + sizeof(int) + commandLen, count - sizeof(int) - commandLen); //处理命令
    }

    /// <summary>
    /// 处理具体命令，子类从这个方法继承
    /// </summary>
    /// <param name="command">收到的数据</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public abstract bool ProcessCommand(byte[] command, int offset, int count);

    protected bool CommandFail(int errorCode, string message, bool sendBackResult = true)
    {
        CommandComposer.AddFailure(errorCode, message);
        if (sendBackResult)
            SendBackResult();
        return false;
    }

    protected bool CommandSuccess(Dictionary<string, List<object>> addValues, bool sendBackResult = true)
    {
        return CommandSuccess([], 0, 0, addValues, sendBackResult);
    }

    protected bool CommandSuccess(byte[] buffer, int offset, int count, Dictionary<string, List<object>> addValues, bool sendBackResult = true)
    {
        CommandComposer.AddSuccess();
        foreach (var (key, values) in addValues)
        {
            foreach (var value in values)
                CommandComposer.AddValue(key, value.ToString() ?? "");
        }
        if (sendBackResult)
            SendBackResult(buffer, offset, count);
        return true;
    }

    public virtual void ProcessSend()
    {
        ActiveTime = DateTime.UtcNow;
        IsSendingAsync = false;
        var asyncSendBufferManager = UserToken.SendBuffer;
        asyncSendBufferManager.ClearFirstPack(); //清除已发送的包
        if (!asyncSendBufferManager.GetFirstPack(out var offset, out var count))
            return;
        IsSendingAsync = true;
        Server.SendAsync(UserToken, asyncSendBufferManager.DynamicBuffer.Buffer, offset, count);
    }

    public void SendBackResult()
    {
        SendBackResult([], 0, 0);
    }

    public void SendBackResult(byte[] buffer, int offset, int count)
    {
        string commandText = CommandComposer.GetProtocolText();
        byte[] bufferUTF8 = Encoding.UTF8.GetBytes(commandText);
        int totalLength = sizeof(int) + bufferUTF8.Length + count; //获取总大小
        AsyncSendBufferManager asyncSendBufferManager = UserToken.SendBuffer;
        asyncSendBufferManager.StartPack();
        asyncSendBufferManager.DynamicBuffer.WriteInt(totalLength, false); //写入总大小
        asyncSendBufferManager.DynamicBuffer.WriteInt(bufferUTF8.Length, false); //写入命令大小
        asyncSendBufferManager.DynamicBuffer.WriteBuffer(bufferUTF8); //写入命令内容
        asyncSendBufferManager.DynamicBuffer.WriteBuffer(buffer, offset, count); //写入二进制数据
        asyncSendBufferManager.EndPack();

        if (IsSendingAsync)
            return;
        if (asyncSendBufferManager.GetFirstPack(out var packOffset, out var packCount))
        {
            IsSendingAsync = true;
            Server.SendAsync(UserToken, asyncSendBufferManager.DynamicBuffer.Buffer, packOffset, packCount);
        }
    }

    /// <summary>
    /// 不是按包格式下发一个内存块，用于日志这类下发协议
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    public void SendBackBuffer(byte[] buffer, int offset, int count)
    {
        AsyncSendBufferManager asyncSendBufferManager = UserToken.SendBuffer;
        asyncSendBufferManager.StartPack();
        asyncSendBufferManager.DynamicBuffer.WriteBuffer(buffer, offset, count);
        asyncSendBufferManager.EndPack();

        if (IsSendingAsync)
            return;
        if (asyncSendBufferManager.GetFirstPack(out var packOffset, out var packCount))
        {
            IsSendingAsync = true;
            Server.SendAsync(UserToken, asyncSendBufferManager.DynamicBuffer.Buffer, packOffset, packCount);
        }
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
