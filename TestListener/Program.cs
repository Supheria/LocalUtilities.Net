using LocalUtilities.Net.Sockets;

var listener = new SocketTcpListener<byte[], byte[]>(new SocketByteHandler());
listener.Port = 5000;
listener.AcceptCompleted += listener_ReceiveCompleted;
listener.Start();

Console.ReadLine();

void listener_ReceiveCompleted(object sender, SocketEventArgs<ISocket<byte[], byte[]>> e)
{
    e.Data.ReceiveAsync().ContinueWith(async (requestTask) =>
    {

        var request = requestTask.Result;
        await e.Data.SendAsync(request);
    });
}