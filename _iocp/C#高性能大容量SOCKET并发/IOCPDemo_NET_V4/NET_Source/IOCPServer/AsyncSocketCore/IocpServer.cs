using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Net;

public class IocpServer
{
    Socket? Core { get; set; } = null;

    public bool IsStart { get; private set; } = false;

    /// <summary>
    /// 每个连接接收缓存大小
    /// </summary>
    int ReceiveBufferSize { get; } = ConstTabel.ReceiveBufferSize;

    /// <summary>
    /// 最大支持连接个数
    /// </summary>
    int ConnectCountMax { get; } 

    /// <summary>
    /// 限制访问接收连接的线程数，用来控制最大并发数
    /// </summary>
    Semaphore ClientCountMax { get; }

    /// <summary>
    /// Socket最大超时时间，单位为MS
    /// </summary>
    public int TimeoutMilliseconds { get; }

    AsyncUserTokenPool UserTokenPool { get; } 

    public AsyncUserTokenList UserTokenList { get; } = [];

    public FileProcessProtocolManager<UploadProtocol> UploadProtocolManager { get; } = [];

    public FileProcessProtocolManager<DownloadProtocol> DownloadProtocolManager { get; } = [];

    DaemonThread? DaemonThread { get; set; } = null;

    public enum ClientState
    {
        Connect,
        Disconnect,
    }

    public delegate void ClientNumberChange(ClientState state, AsyncUserToken userToken);

    public delegate void ReceiveClientData(AsyncUserToken userToken, byte[] data);

    public event ClientNumberChange? OnClientNumberChange;

    public event ReceiveClientData? OnReceiveClientData;

    public IocpServer(int connectCountMax, int timeoutMilliseconds)
    {
        ConnectCountMax = connectCountMax;
        ClientCountMax = new(connectCountMax, connectCountMax);
        UserTokenPool = new(connectCountMax);
        for (int i = 0; i < ConnectCountMax; i++) //按照连接数建立读写对象
        {
            var userToken = new AsyncUserToken(ReceiveBufferSize, ProcessReceive, ProcessSend);
            UserTokenPool.Push(userToken);
        }
        TimeoutMilliseconds = timeoutMilliseconds;
    }

    public void Start(int port)
    {
        if (IsStart)
        {
            //Program.Logger.InfoFormat("server {0} has started ", localEndPoint.ToString());
            return;
        }
        var endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
        Core = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Core.Bind(endPoint);
        Core.Listen(ConnectCountMax);
        //Program.Logger.InfoFormat("Start listen socket {0} success", localEndPoint.ToString());
        //for (int i = 0; i < 64; i++) //不能循环投递多次AcceptAsync，会造成只接收8000连接后不接收连接了
        StartAccept(null);
        DaemonThread = new DaemonThread(this);
        IsStart = true;
    }

    public void Stop()
    {
        if (!IsStart)
        {
            //Program.Logger.InfoFormat("server {0} has not started yet", localEndPoint.ToString());
            return;
        }
        lock (UserTokenList)
        {
            foreach (var userToken in UserTokenList)
            {
                userToken.Close(null);
                OnClientNumberChange?.Invoke(ClientState.Disconnect, userToken);
            }
        }
        UserTokenList.Clear();
        Core?.Close();
        IsStart = false;
    }

    public void StartAccept(SocketAsyncEventArgs? acceptEventArgs)
    {
        acceptEventArgs?.Dispose();
        acceptEventArgs = new();
        acceptEventArgs.Completed += (sender, e) => ProcessAccept(e);
        ClientCountMax.WaitOne(); //获取信号量
        if (Core is not null && !Core.AcceptAsync(acceptEventArgs))
            ProcessAccept(acceptEventArgs);
    }

    private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
    {
        //Program.Logger.InfoFormat("Client connection accepted. Local Address: {0}, Remote Address: {1}",
        //    acceptEventArgs.AcceptSocket.LocalEndPoint, acceptEventArgs.AcceptSocket.RemoteEndPoint);
        if (acceptEventArgs.AcceptSocket is null)
            return;
        var userToken = UserTokenPool.Pop();
        // 添加到正在连接列表
        UserTokenList.Add(userToken);
        userToken.ProcessAccept(acceptEventArgs.AcceptSocket);
        try
        {
            if (userToken.AcceptSocket is not null && !userToken.AcceptSocket.ReceiveAsync(userToken.ReceiveAsyncArgs))
                ProcessReceive(userToken.ReceiveAsyncArgs);
            OnClientNumberChange?.Invoke(ClientState.Connect, userToken);
        }
        catch (Exception ex)
        {
            //Program.Logger.ErrorFormat("Accept client {0} error, message: {1}", userToken.ConnectSocket, ex.Message);
            //Program.Logger.Error(ex.StackTrace);
        }
        // 把当前异步事件释放，等待下次连接
        if (acceptEventArgs.SocketError is not SocketError.OperationAborted)
            StartAccept(acceptEventArgs);
    }

    private void ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
    {
        if (receiveEventArgs.UserToken is not AsyncUserToken userToken)
            return;
        if (!userToken.ProcessReceive(this, out var receiveBuffer))
        {
            CloseClient(userToken);
            return;
        }
        var offset = userToken.ReceiveAsyncArgs.Offset + 1;
        var size = userToken.ReceiveAsyncArgs.BytesTransferred + 1;
        // 处理接收数据
        if (size > 0)
        {
            if (!userToken.Protocol.ProcessReceive(receiveBuffer, offset, size))
            {
                CloseClient(userToken);
                return;
            }
        }
        if (!userToken.AcceptSocket.ReceiveAsync(userToken.ReceiveAsyncArgs))
            ProcessReceive(userToken.ReceiveAsyncArgs);
    }

    private void ProcessSend(SocketAsyncEventArgs sendEventArgs)
    {
        var userToken = sendEventArgs.UserToken as AsyncUserToken;
        if (sendEventArgs.SocketError is SocketError.Success)
            userToken?.ProcessSend();
        else
            CloseClient(userToken);
    }

    public void SendAsync(AsyncUserToken userToken, byte[] buffer, int offset, int count)
    {
        if (userToken.AcceptSocket is null)
            return;
        userToken.SendAsyncArgs.SetBuffer(buffer, offset, count);
        if (!userToken.AcceptSocket.SendAsync(userToken.SendAsyncArgs))
            ProcessSend(userToken.SendAsyncArgs);
    }

    public void CloseClient(AsyncUserToken? userToken)
    {
        if (userToken is null)
            return;
        userToken.Close(protocol =>
        {
            if (protocol is UploadProtocol upload)
                lock (UploadProtocolManager)
                {
                    UploadProtocolManager.Remove(upload);
                }
            else if (protocol is DownloadProtocol download)
                lock (DownloadProtocolManager)
                {
                    DownloadProtocolManager.Remove(download);
                }

        });
        //Program.Logger.InfoFormat("Client connection disconnected. {0}", socketInfo);
        ClientCountMax.Release();
        UserTokenPool.Push(userToken);
        UserTokenList.Remove(userToken);
        OnClientNumberChange?.Invoke(ClientState.Disconnect, userToken);
    }
}
