using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace LocalUtilities.Net.Sockets
{
    public class SocketTcpListener : IEnumerable<ISocket>
    {
        protected Socket? Socket { get; private set; }

        HashSet<ISocket> Clients { get; } = [];

        public ISocketStreamHandler Handler { get; private set; }

        public ISocketStreamProvider StreamProvider { get; set; } = new SocketNetworkStreamProvider();

        public int Count => Clients.Count;

        public int Port
        {
            get => _port;
            set
            {
                if (value < 0 || value > 65535)
                    throw new ArgumentOutOfRangeException(_port + "不是有效端口。");
                _port = value;
            }
        }
        int _port;

        /// <summary>
        /// 接受客户完成时引发事件。
        /// </summary>
        public event EventHandler<SocketEventArgs<ISocket>>? AcceptCompleted;

        public bool IsStarted { get; private set; }

        public SocketTcpListener(ISocketStreamHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);
            Clients = [];
            Handler = handler;
            IsStarted = false;
        }

        public void Start()
        {
            if (IsStarted)
                throw new InvalidOperationException("已经开始服务。");
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定端口
            //可以引发端口被占用异常
            Socket.Bind(new IPEndPoint(IPAddress.Any, _port));
            //监听队列
            Socket.Listen(ushort.MaxValue);
            //如果端口是0，则是随机端口，把这个端口赋值给port
            _port = ((IPEndPoint)Socket.LocalEndPoint).Port;
            //服务启动中设置为true
            IsStarted = true;
            //开始异步监听
            Socket.BeginAccept(EndAccept, null);
        }

        /// <summary>
        /// 停止服务。
        /// </summary>
        public void Stop()
        {
            if (!IsStarted)
                throw new InvalidOperationException("没有开始服务。");
            foreach (var client in Clients.ToArray())
            {
                client.Disconnect();
                client.DisconnectCompleted -= Client_DisconnectCompleted;
            }
            Socket?.Close();
            Socket?.Dispose();
            Socket = null;
            IsStarted = false;
        }

        private async void EndAccept(IAsyncResult result)
        {
            Socket? clientSocket = null;

            //获得客户端Socket
            try
            {
                clientSocket = Socket?.EndAccept(result);
                Socket?.BeginAccept(EndAccept, null);
            }
            catch { }

            if (clientSocket is null)
                return;
            //实例化客户端类
            var client = await GetClientAsync(clientSocket);
            if (client != null)
            {
                //增加事件钩子
                client.DisconnectCompleted += Client_DisconnectCompleted;

                //增加客户端
                lock (Clients)
                    Clients.Add(client);

                //客户端连接事件
                AcceptCompleted?.Invoke(this, new SocketEventArgs<ISocket>(client, SocketAsyncOperation.Accept));
            }
        }

        protected virtual async Task<ISocket?> GetClientAsync(Socket socket)
        {
            var client = new SocketTcpClient(socket, Handler, StreamProvider);
            try
            {
                await client.InitializeAsync();
            }
            catch
            {
                return null;
            }
            return client;
        }

        //客户端断开连接
        private void Client_DisconnectCompleted(object? sender, SocketEventArgs e)
        {
            if (sender is not SocketTcpBase client)
                return;
            lock (Clients)
                Clients.Remove(client);
            client.DisconnectCompleted -= Client_DisconnectCompleted;
        }

        /// <summary>
        /// 获取客户端泛型。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ISocket> GetEnumerator()
        {
            return Clients.GetEnumerator();
        }

        /// <summary>
        /// 获取客户端泛型。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Clients.GetEnumerator();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Socket == null)
                return;
            Stop();
        }
    }
}
