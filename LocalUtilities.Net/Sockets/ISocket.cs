using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalUtilities.Net.Sockets;

/// <summary>
/// Socket支持接口。
/// </summary>
public interface ISocket
{
    bool IsConnected { get; }

    event EventHandler<SocketEventArgs> DisconnectCompleted;

    event EventHandler<SocketEventArgs<byte[]>> ReceiveCompleted;

    event EventHandler<SocketEventArgs<byte[]>> SendCompleted;

    IPEndPoint RemoteEndPoint { get; }

    IPEndPoint LocalEndPoint { get; }

    /// <summary>
    /// 获取字典。
    /// </summary>
    dynamic DataBag { get; }

    void Disconnect();

    Task DisconnectAsync();

    bool Send(byte[] data);

    Task<bool> SendAsync(byte[] data);

    byte[] Receive();

    Task<byte[]> ReceiveAsync();
    /// <summary>
    /// 循环接收数据。
    /// </summary>
    /// <returns></returns>
    void ReceiveCycle();
}
