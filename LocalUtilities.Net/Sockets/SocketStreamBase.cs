﻿using System.Net;
using System.Net.Sockets;

namespace LocalUtilities.Net.Sockets;

/// <summary>
/// 流式Socket基类。
/// </summary>
public abstract class SocketStreamBase : ISocket, IDisposable
{
    private SocketDataBag _DataBag;

    /// <summary>
    /// 实例化Socket基类。
    /// </summary>
    /// <param name="socket">Socket套接字。</param>
    /// <param name="socketHandler">Socket处理者。</param>
    /// <param name="streamProvider">Socket网络流提供者。</param>
    protected SocketStreamBase(Socket socket, ISocketStreamHandler socketHandler, ISocketStreamProvider streamProvider)
    {
        ArgumentNullException.ThrowIfNull(socket);
        ArgumentNullException.ThrowIfNull(socketHandler);
        ArgumentNullException.ThrowIfNull(streamProvider);
        Socket = socket;
        Handler = socketHandler;
        StreamProvider = streamProvider;
    }

    /// <summary>
    /// 获取Socket套接字。
    /// </summary>
    protected Socket Socket { get; private set; }

    /// <summary>
    /// 获取Socket处理者。
    /// </summary>
    public ISocketStreamHandler Handler { get; private set; }

    /// <summary>
    /// 获取Socket处理上下文。
    /// </summary>
    public SocketStreamHandlerContext? HandlerContext { get; private set; }

    /// <summary>
    /// 获取Socket网络流提供者。
    /// </summary>
    public ISocketStreamProvider StreamProvider { get; private set; }

    /// <summary>
    /// 获取或设置Socket是否已连接。
    /// </summary>
    public virtual bool IsConnected { get; protected set; }

    #region 初始化

    /// <summary>
    /// 初始化Socket连接。调用于Socket连接建立后。
    /// </summary>
    public virtual void Initialize()
    {
        HandlerContext = new SocketStreamHandlerContext(StreamProvider.GetStream(Socket), DataBag);
    }

    public virtual async Task InitializeAsync()
    {
        HandlerContext = new SocketStreamHandlerContext(await StreamProvider.GetStreamAsync(Socket), DataBag);
    }

    #endregion

    #region 断开连接

    /// <summary>
    /// 断开与服务器的连接。
    /// </summary>
    public abstract void Disconnect();

    /// <summary>
    /// 异步断开与服务器的连接。
    /// </summary>
    public abstract Task DisconnectAsync();

    /// <summary>
    /// 断开连接后调用的方法。
    /// </summary>
    /// <param name="raiseEvent">是否触发事件。</param>
    protected void Disconnected(bool raiseEvent)
    {
        HandlerContext = null;
        if (raiseEvent && DisconnectCompleted != null)
            DisconnectCompleted(this, new SocketEventArgs(SocketAsyncOperation.Disconnect));
    }

    #endregion

    #region 发送数据

    /// <summary>
    /// 发送数据。
    /// </summary>
    /// <param name="data">要发送的数据。</param>
    public bool Send(byte[] data)
    {
        //是否已连接
        if (!IsConnected)
            throw new SocketException(10057);
        //发送的数据不能为null
        if (data == null)
            throw new ArgumentNullException("data");

        //开始发送数据
        if (!Handler.Send(data, HandlerContext))
        {
            Disconnected(true);
            return false;
        }
        else
        {
            if (SendCompleted != null)
                SendCompleted(this, new SocketEventArgs<byte[]>(data, SocketAsyncOperation.Send));
            return true;
        }
    }

    /// <summary>
    /// 异步发送数据。
    /// </summary>
    /// <param name="data">要发送的数据。</param>
    public async Task<bool> SendAsync(byte[] data)
    {
        //是否已连接
        if (!IsConnected)
            throw new SocketException(10057);
        //发送的数据不能为null
        if (data == null)
            throw new ArgumentNullException("data");

        if (!await Handler.SendAsync(data, HandlerContext))
        {
            Disconnected(true);
            return false;
        }
        else
        {
            if (SendCompleted != null)
                SendCompleted(this, new SocketEventArgs<byte[]>(data, SocketAsyncOperation.Send));
            return true;
        }
    }

    #endregion

    #region 接收数据

    public byte[] Receive()
    {
        //是否已连接
        if (!IsConnected)
            throw new SocketException(10057);
        byte[] value = Handler.Receive(HandlerContext);
        if (value == null)
        {
            Disconnect();
        }
        else
            if (ReceiveCompleted != null)
            ReceiveCompleted(this, new SocketEventArgs<byte[]>(value, SocketAsyncOperation.Receive));
        return value;
    }

    public async Task<byte[]> ReceiveAsync()
    {
        //是否已连接
        if (!IsConnected)
            throw new SocketException(10057);
        byte[] value = await Handler.ReceiveAsync(HandlerContext);
        if (value == null)
        {
            await DisconnectAsync();
        }
        else
            if (ReceiveCompleted != null)
            ReceiveCompleted(this, new SocketEventArgs<byte[]>(value, SocketAsyncOperation.Receive));
        return value;
    }

    public async void ReceiveCycle()
    {
        //是否已连接
        if (!IsConnected)
            throw new SocketException(10057);
        while (IsConnected)
        {
            byte[] data = await Handler.ReceiveAsync(HandlerContext);
            if (data == null)
            {
                await DisconnectAsync();
                return;
            }
            if (ReceiveCompleted != null)
                ReceiveCompleted(this, new SocketEventArgs<byte[]>(data, SocketAsyncOperation.Receive));
        }
    }

    #endregion

    #region 事件

    ///// <summary>
    ///// 断开完成时引发事件。
    ///// </summary>
    public event EventHandler<SocketEventArgs>? DisconnectCompleted;
    ///// <summary>
    ///// 接收完成时引发事件。
    ///// </summary>
    public event EventHandler<SocketEventArgs<byte[]>>? ReceiveCompleted;
    ///// <summary>
    ///// 发送完成时引发事件。
    ///// </summary>
    public event EventHandler<SocketEventArgs<byte[]>>? SendCompleted;

    #endregion

    #region 字典

    public dynamic DataBag
    {
        get
        {
            _DataBag ??= new SocketDataBag();
            return _DataBag;
        }
    }

    #endregion

    #region 其它

    /// <summary>
    /// 释放资源。
    /// </summary>
    public void Dispose()
    {
        Disposed();
        if (_DataBag != null)
        {
            _DataBag.Clear();
            _DataBag = null;
        }
    }

    /// <summary>
    /// 释放资源。
    /// </summary>
    protected virtual void Disposed()
    {

    }

    /// <summary>
    /// 获取远程终结点地址。
    /// </summary>
    public abstract IPEndPoint RemoteEndPoint { get; }

    /// <summary>
    /// 获取本地终结点地址。
    /// </summary>
    public abstract IPEndPoint LocalEndPoint { get; }

    #endregion
}
