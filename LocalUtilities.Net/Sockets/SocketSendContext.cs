namespace LocalUtilities.Net.Sockets;

public class SocketSendContext(Stream source) : SocketProcessContext(source)
{
    public byte[] Data { get; set; } = [];

    public override void Reset()
    {
        Data = [];
        base.Reset();
    }
}
