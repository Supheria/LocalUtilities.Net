namespace LocalUtilities.Net.Sockets;

/// <summary>
/// Socket流处理器接口。
/// </summary>
public interface ISocketStreamHandler
{
    /// <summary>
    /// 接收
    /// </summary>
    /// <param name="context">处理上下文</param>
    /// <returns>收到的数据</returns>
    byte[] Receive(SocketStreamHandlerContext context);
    /// <summary>
    /// 异步接收
    /// </summary>
    /// <param name="context">处理上下文</param>
    /// <returns>任务结果</returns>
    Task<byte[]> ReceiveAsync(SocketStreamHandlerContext context);
    /// <summary>
    /// 开始发送
    /// </summary>
    /// <param name="data">要发送的数据</param>
    /// <param name="context">处理上下文</param>
    /// <returns>是否成功</returns>
    bool Send(byte[] data, SocketStreamHandlerContext context);
    /// <summary>
    /// 开始发送
    /// </summary>
    /// <param name="data">要发送的数据</param>
    /// <param name="context">处理上下文</param>
    /// <returns>任务结果</returns>
    Task<bool> SendAsync(byte[] data, SocketStreamHandlerContext context);
}
