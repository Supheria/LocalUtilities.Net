using System.Net;
using System.Net.Sockets;

namespace LocalUtilities.Net.Sockets;

public interface ISocketDgramContext
{
    bool Send(byte[] data, SocketFlags flags);

    Task<bool> SendAsync(byte[] data, SocketFlags flags);

    event SocketDgramReceiveDelegate Receive;
    public IPEndPoint LocalEndPoint { get; }

    public IPEndPoint RemoteEndPoint { get; }

    public void OnReceive(byte[] data, int length);

    public void OnDisconnect();
}

public delegate void SocketDgramReceiveDelegate(byte[] data, int length);
