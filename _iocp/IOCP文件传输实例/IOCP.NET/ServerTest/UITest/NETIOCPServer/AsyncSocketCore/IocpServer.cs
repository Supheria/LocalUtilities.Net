using Net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Net;

public class IocpServer
{
    Socket? Core { get; set; } = null;

    public bool IsStart { get; private set; } = false;

    /// <summary>
    /// 每个连接接收缓存大小
    /// </summary>
    //int ReceiveBufferSize { get; } = ConstTabel.ReceiveBufferSize;

    /// <summary>
    /// 最大支持连接个数
    /// </summary>
    int ConnectCountMax { get; }

    /// <summary>
    /// 限制访问接收连接的线程数，用来控制最大并发数
    /// </summary>
    Semaphore ClientCountMax { get; }

    /// <summary>
    /// Socket最大超时时间，单位为毫秒
    /// </summary>
    public int TimeoutMilliseconds { get; }

    AsyncUserTokenPool UserTokenPool { get; }

    public AsyncUserTokenList UserTokenList { get; } = [];

    /// <summary>
    /// 所有新加入的服务端协议，必须在此处实例化
    /// </summary>
    //ProtocolManager<ServerFullHandlerProtocol> FullHandlerProtocolManager { get; } = [];

    DaemonThread DaemonThread { get; }

    public enum ClientState
    {
        Connect,
        Disconnect,
    }

    public delegate void HandleMessage(string message, ServerFullHandlerProtocol protocol);

    public delegate void ClientNumberChange(ClientState state, AsyncUserToken userToken);

    public delegate void ReceiveClientData(AsyncUserToken userToken, byte[] data);

    public event HandleMessage? OnReceiveMessage;

    public event ClientNumberChange? OnClientNumberChange;

    public event ReceiveClientData? OnReceiveClientData;

    public IocpServer(int numConnections, int timeoutMilliseconds)
    {
        ConnectCountMax = numConnections;
        UserTokenPool = new AsyncUserTokenPool(numConnections);
        ClientCountMax = new(numConnections, numConnections);
        DaemonThread = new(ProcessDaemon);
        for (int i = 0; i < numConnections; i++) //按照连接数建立读写对象
        {
            var userToken = new AsyncUserToken(this);
            userToken.OnClosed += () =>
            {
                ClientCountMax.Release();
                UserTokenPool.Push(userToken);
                UserTokenList.Remove(userToken);
                OnClientNumberChange?.Invoke(ClientState.Disconnect, userToken);
            };
            UserTokenPool.Push(userToken);
        }
        TimeoutMilliseconds = timeoutMilliseconds;
    }

    /// <summary>
    /// 守护线程
    /// </summary>
    private void ProcessDaemon()
    {
        UserTokenList.CopyTo(out var userTokenss);
        foreach (var userToken in userTokenss)
        {
            try
            {
                if ((DateTime.Now - userToken.SocketInfo.ActiveTime).Milliseconds > TimeoutMilliseconds) //超时Socket断开
                {
                    lock (userToken)
                        userToken.Close();
                }
            }
            catch (Exception ex)
            {
                //ServerInstance.Logger.ErrorFormat("Daemon thread check timeout socket error, message: {0}", ex.Message);
                //ServerInstance.Logger.Error(ex.StackTrace);
            }
        }
    }

    /// <summary>
    /// 设置服务端SOCKET是否延迟，如果保证实时性，请设为true,默认为false
    /// </summary>
    /// <param name="NoDelay"></param>
    //public void SetNoDelay(bool NoDelay)
    //{
    //    Core.NoDelay = NoDelay;
    //}

    public void Start(int port)
    {
        if (IsStart)
        {
            //Program.Logger.InfoFormat("server {0} has started ", localEndPoint.ToString());
            return;
        }
        // 使用0.0.0.0作为绑定IP，则本机所有的IPv4地址都将绑定
        var endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
        Core = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);            
        Core.Bind(endPoint);
        Core.Listen(ConnectCountMax);
        //ServerInstance.Logger.InfoFormat("Start listen socket {0} success", localEndPoint.ToString());
        //for (int i = 0; i < 64; i++) //不能循环投递多次AcceptAsync，会造成只接收8000连接后不接收连接了
        StartAccept(null);
        DaemonThread.Start();
        IsStart = true;
    }

    public void Stop()
    {
        if (!IsStart)
        {
            //ServerInstance.Logger.Info("server {0} has not started yet", localEndPoint.ToString());
            return;
        }
        UserTokenList.CopyTo(out var userTokens);
        foreach (var userToken in userTokens)//双向关闭已存在的连接
            userToken.Close();
        UserTokenList.Clear();
        Core?.Close();
        DaemonThread.Stop();
        IsStart = false;
        //ServerInstance.Logger.Info("Server is Stoped");
    }

    private void StartAccept(SocketAsyncEventArgs? acceptEventArgs)
    {
        acceptEventArgs?.Dispose();
        acceptEventArgs = new();
        //acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
        acceptEventArgs.Completed += (sender, e) => ProcessAccept(e);
        ClientCountMax.WaitOne(); //获取信号量
        if (Core is not null && !Core.AcceptAsync(acceptEventArgs))
            ProcessAccept(acceptEventArgs);
    }

    private void ProcessAccept(SocketAsyncEventArgs acceptArgs)
    {
        var userToken = UserTokenPool.Pop();
        if (!userToken.ProcessAccept(acceptArgs.AcceptSocket))
        {
            UserTokenPool.Push(userToken);
            return;
        }
        UserTokenList.Add(userToken);
        try
        {
            userToken.ReceiveAsync();
            OnClientNumberChange?.Invoke(ClientState.Connect, userToken);
        }
        catch (Exception E)
        {
            //ServerInstance.Logger.ErrorFormat("Accept client {0} error, message: {1}", userToken.AcceptSocket, E.Message);
            //ServerInstance.Logger.Error(E.StackTrace);
        }
        if (acceptArgs.SocketError is not SocketError.OperationAborted)
            StartAccept(acceptArgs); //把当前异步事件释放，等待下次连接
    }

    public void HandleReceiveMessage(string message, ServerFullHandlerProtocol protocol)
    {
        OnReceiveMessage?.Invoke(message, protocol);
    }

    /// <summary>
    /// 检测文件是否正在使用中，如果正在使用中则检测是否被上传协议占用，如果占用则关闭, 真表示正在使用中，并没有关闭
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public bool CheckFileInUse(string filePath)
    {
        if (isFileInUse())
        {
            var result = true;
            UserTokenList.CopyTo(out var userTokens);
            foreach (var userToken in userTokens)
            {
                if (userToken.Protocol is not ServerFullHandlerProtocol fullHandler)
                    continue;
                if (!filePath.Equals(fullHandler.FilePath, StringComparison.CurrentCultureIgnoreCase))
                    continue;
                lock (userToken) //AsyncSocketUserToken有多个线程访问
                    userToken.Close();
                result = false;
            }
            return result;
        }
        return false;
        bool isFileInUse()
        {
            try
            {
                // 使用共享只读方式打开，可以支持多个客户端同时访问一个文件。
                using var _ = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
