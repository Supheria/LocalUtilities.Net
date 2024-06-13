using System.Net;
using System.Net.Sockets;

namespace LocalUtilities.Net.Sockets;

public class SocketUdpHost
{
    private Dictionary<IPEndPoint, SocketUdpClient> _Clients;
    private IPEndPoint _ReceiveFrom;

    public SocketUdpHost(ISocketDgramHandler socketHandler)
    {
        ArgumentNullException.ThrowIfNull(socketHandler);

        Handler = socketHandler;
        Timeout = TimeSpan.FromSeconds(30);
        BufferSize = 1024;
        _Clients = [];
    }

    public ISocketDgramHandler Handler { get; private set; }

    protected internal Socket Socket { get; private set; }

    public int Port { get; private set; }

    public TimeSpan Timeout { get; set; }

    public bool IsStarted { get; private set; }

    public int BufferSize { get; set; }

    public void Start()
    {
        Start(new IPEndPoint(IPAddress.Any, 0));
    }

    public void Start(IPEndPoint endPoint)
    {
        if (IsStarted)
            throw new InvalidOperationException("已开始服务。");
        IsStarted = true;
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        Socket.Bind(endPoint);
        Port = ((IPEndPoint)Socket.LocalEndPoint).Port;
        _ReceiveFrom = new IPEndPoint(IPAddress.Any, 0);
        BeginReceive(new IPEndPoint(endPoint.Address, endPoint.Port));
    }

    private void BeginReceive(IPEndPoint endPoint)
    {
        byte[] buffer = new byte[BufferSize];
        EndPoint cEndPoint = endPoint;
        Socket.BeginReceiveFrom(buffer, 0, BufferSize, SocketFlags.None, ref cEndPoint, EndReceive, buffer);
    }

    private void EndReceive(IAsyncResult ar)
    {
        if (!IsStarted)
            return;
        EndPoint endPoint = new IPEndPoint(_ReceiveFrom.Address, _ReceiveFrom.Port);
        var length = Socket.EndReceiveFrom(ar, ref endPoint);
        var buffer = (byte[])ar.AsyncState;
        IPEndPoint cEndPoint = (IPEndPoint)endPoint;
        if (!_Clients.TryGetValue(cEndPoint, out var client))
        {
            client = new SocketUdpClient(new SocketUdpContext(Socket, cEndPoint), Handler);
            client.Context.Disconnect += Context_Disconnect;
            _Clients.Add(cEndPoint, client);
            if (AcceptCompleted != null)
                AcceptCompleted(this, new SocketEventArgs<ISocket>(client, SocketAsyncOperation.Accept));
        }
        BeginReceive(new IPEndPoint(_ReceiveFrom.Address, _ReceiveFrom.Port));
        client.Context.OnReceive(buffer, length);
    }

    void Context_Disconnect(object sender, EventArgs e)
    {
        SocketUdpContext context = (SocketUdpContext)sender;
        _Clients.Remove(context.RemoteEndPoint);
    }

    public void Stop()
    {
        if (!IsStarted)
            throw new InvalidOperationException("还未开始服务。");
        IsStarted = false;
        lock (_Clients)
        {
            foreach (var items in _Clients.ToArray())
                items.Value.Disconnect();
        }
        Socket.Dispose();
        Socket = null;
    }

    public ISocket Open(IPEndPoint endPoint)
    {
        if (!IsStarted)
            throw new InvalidOperationException("还未开始服务。");
        if (!_Clients.TryGetValue(endPoint, out var client))
        {
            client = new SocketUdpClient(new SocketUdpContext(Socket, endPoint), Handler);
            client.Context.Disconnect += Context_Disconnect;
            _Clients.Add(endPoint, client);
        }
        return client;
    }

    public event EventHandler<SocketEventArgs<ISocket>> AcceptCompleted;

}
