using System.Net;
using System.Net.Sockets;

namespace LocalUtilities.Net.Sockets;

public abstract class SocketDgramBase : ISocket
{
    private SocketDataBag _DataBag;

    protected SocketDgramBase(SocketUdpContext context, ISocketDgramHandler socketHandler)
    {
        if (context == null)
            throw new ArgumentNullException("context");
        if (socketHandler == null)
            throw new ArgumentNullException("socketHandler");
        Context = context;
        Handler = socketHandler;
        context.Receive += context_Receive;
    }

    private void context_Receive(byte[] data, int length)
    {
        var item = Handler.ConvertReceiveData(data, length);
        if (item != null && ReceiveCompleted != null)
            ReceiveCompleted(this, new SocketEventArgs<byte[]>(item, SocketAsyncOperation.ReceiveFrom));
    }

    public SocketUdpContext Context { get; protected set; }

    public ISocketDgramHandler Handler { get; set; }

    public virtual bool IsConnected
    {
        get { return true; }
    }

    #region 发送

    public bool Send(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException("data");
        var buffer = Handler.ConvertSendData(data);
        if (Context.Send(buffer, SocketFlags.None))
        {
            if (SendCompleted != null)
                SendCompleted(this, new SocketEventArgs<byte[]>(data, SocketAsyncOperation.SendTo));
            return true;
        }
        return false;
    }

    public async Task<bool> SendAsync(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException("data");
        var buffer = Handler.ConvertSendData(data);
        if (await Context.SendAsync(buffer, SocketFlags.None))
        {
            if (SendCompleted != null)
                SendCompleted(this, new SocketEventArgs<byte[]>(data, SocketAsyncOperation.SendTo));
            return true;
        }
        return false;
    }

    #endregion

    #region 接收（无效）

    public byte[] Receive()
    {
        throw new NotSupportedException();
    }

    public Task<byte[]> ReceiveAsync()
    {
        throw new NotSupportedException();
    }

    public void ReceiveCycle()
    {
        throw new NotSupportedException();
    }

    #endregion

    #region 事件

    public event EventHandler<SocketEventArgs<byte[]>> ReceiveCompleted;

    public event EventHandler<SocketEventArgs<byte[]>> SendCompleted;

    public event EventHandler<SocketEventArgs> DisconnectCompleted;

    #endregion

    #region 断开

    public abstract void Disconnect();

    public abstract Task DisconnectAsync();

    protected void OnDisconnected()
    {
        if (DisconnectCompleted != null)
            DisconnectCompleted(this, new SocketEventArgs(SocketAsyncOperation.Disconnect));
    }
    #endregion

    #region 字典

    public dynamic DataBag
    {
        get
        {
            if (_DataBag == null)
                _DataBag = new SocketDataBag();
            return _DataBag;
        }
    }

    #endregion

    #region 其它

    public abstract IPEndPoint RemoteEndPoint { get; }

    public abstract IPEndPoint LocalEndPoint { get; }

    #endregion
}
