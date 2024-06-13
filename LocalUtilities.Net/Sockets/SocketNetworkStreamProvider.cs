using System.Net.Sockets;

namespace LocalUtilities.Net.Sockets
{
    public class SocketNetworkStreamProvider : ISocketStreamProvider
    {
        public Stream GetStream(Socket socket)
        {
            if (!socket.Connected)
                throw new ArgumentException("Socket未连接。");
            return new NetworkStream(socket);
        }

        public Task<Stream> GetStreamAsync(Socket socket)
        {
            if (!socket.Connected)
                throw new ArgumentException("Socket未连接。");
            return Task.Run<Stream>(() =>
            {
                return new NetworkStream(socket);
            });
        }
    }
}
