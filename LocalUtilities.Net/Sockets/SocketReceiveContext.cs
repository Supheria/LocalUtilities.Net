namespace LocalUtilities.Net.Sockets;

public class SocketReceiveContext(Stream source) : SocketProcessContext(source)
{
    public SocketBufferedStream Buffer { get; private set; } = new();

    public byte[] ByteBuffer { get; set; } = [];

    public byte[] Result { get; set; } = [];

    public override void Reset()
    {
        Result = [];
        Buffer.ResetPosition();
        ByteBuffer = [];
        base.Reset();
    }
}
