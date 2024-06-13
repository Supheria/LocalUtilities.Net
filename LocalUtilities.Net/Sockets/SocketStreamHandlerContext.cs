namespace LocalUtilities.Net.Sockets;

/// <summary>
/// Socket流处理器上下文。
/// </summary>
public class SocketStreamHandlerContext
{
    /// <summary>
    /// 初始化Socket流处理器上下文。
    /// </summary>
    /// <param name="stream">网络流。</param>
    /// <param name="dataBag">Socket字典。</param>
    public SocketStreamHandlerContext(Stream stream, SocketDataBag dataBag)
    {
        ArgumentNullException.ThrowIfNull(stream);
        Stream = stream;
        DataBag = dataBag;
        ReceiveContext = new(stream);
        SendContext = new(stream);
    }

    /// <summary>
    /// 获取Socket网络流。
    /// </summary>
    public Stream Stream { get; private set; }

    /// <summary>
    /// 获取Socket字典。
    /// </summary>
    public dynamic DataBag { get; private set; }

    /// <summary>
    /// 获取接收上下文。
    /// </summary>
    public SocketReceiveContext ReceiveContext { get; private set; }

    /// <summary>
    /// 获取发送上下文。
    /// </summary>
    public SocketSendContext SendContext { get; private set; }
}
