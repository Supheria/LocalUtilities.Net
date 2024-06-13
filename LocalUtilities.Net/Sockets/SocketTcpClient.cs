using System.Net;
using System.Net.Sockets;

namespace LocalUtilities.Net.Sockets
{
    /// <summary>
    /// Socket Tcp客户端。
    /// </summary>
    public class SocketTcpClient : SocketTcpBase
    {
        /// <summary>
        /// 实例化SocketTcpClient。
        /// </summary>
        /// <param name="handler">Socket处理器。</param>
        public SocketTcpClient(ISocketStreamHandler handler)
            : base(new Socket(SocketType.Stream, ProtocolType.Tcp), handler)
        { }

        /// <summary>
        /// 实例化SocketTcpClient。
        /// </summary>
        /// <param name="handler">Socket处理器。</param>
        /// <param name="streamProvider">Socket流提供器。</param>
        public SocketTcpClient(ISocketStreamHandler handler, ISocketStreamProvider streamProvider)
            : base(new Socket(SocketType.Stream, ProtocolType.Tcp), handler, streamProvider)
        { }


        /// <summary>
        /// 使用已处理过的Socket实例化SocketTcpClient。
        /// </summary>
        /// <param name="socket">处理过的Socket。</param>
        /// <param name="handler">Socket处理器。</param>
        /// <param name="streamProvider">Socket流提供器。</param>
        public SocketTcpClient(Socket socket, ISocketStreamHandler handler, ISocketStreamProvider streamProvider)
            : base(socket, handler, streamProvider)
        { }

        #region 连接

        /// <summary>
        /// 连接至服务器。
        /// </summary>
        /// <param name="endPoint">服务器终结点。</param>
        public bool Connect(IPEndPoint endPoint)
        {
            //判断是否已连接
            if (IsConnected)
                throw new InvalidOperationException("已连接至服务器。");
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");

            try
            {
                Socket.Connect(endPoint);
                ConnectCompleted?.Invoke(this, new SocketEventArgs(SocketAsyncOperation.Connect));
                Initialize();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 异步连接至服务器。
        /// </summary>
        /// <param name="endPoint"></param>
        public async Task<bool> ConnectAsync(IPEndPoint endPoint)
        {
            //判断是否已连接
            if (IsConnected)
                throw new InvalidOperationException("已连接至服务器。");
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");

            try
            {
                await Task.Factory.FromAsync(Socket.BeginConnect(endPoint, null, null), Socket.EndConnect);
                if (ConnectCompleted != null)
                    ConnectCompleted(this, new SocketEventArgs(SocketAsyncOperation.Connect));
                await InitializeAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 连接完成时引发事件。
        /// </summary>
        public event EventHandler<SocketEventArgs>? ConnectCompleted;
    }
}
