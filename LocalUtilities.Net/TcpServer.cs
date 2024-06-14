using LocalUtilities.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LocalUtilities.Net;

public sealed class TcpServer
{
    const int OpsToAlloc = 2;

    Socket? Core { get; set; } = null;

    //BufferManager BufferManager { get; }

    SocketEventPool SocketEventPool { get; }

    public int ClientCount => Clients.Count;

    Dictionary<string, AsyncClientProfile> Clients { get; } = [];

    public bool IsStart { get; private set; } = false;

    /// <summary>  
    /// 客户端连接数量变化时触发  
    /// </summary>  
    /// <param name="num">当前增加客户的个数(用户退出时为负数,增加时为正数,一般为1)</param>  
    /// <param name="client">增加用户的信息</param>  
    public delegate void OnClientNumberChange(bool connect, AsyncClientProfile client);

    /// <summary>  
    /// 接收到客户端的数据  
    /// </summary>  
    /// <param name="client">客户端</param>  
    /// <param name="buff">客户端数据</param>  
    public delegate void OnReceiveData(AsyncClientProfile client, byte[] buff);

    public event OnClientNumberChange? ClientNumberChange;

    public event OnReceiveData? ReceiveClientData;

    //Dictionary<string, Socket> Clients { get; } = [];

    public event EventHandler<SocketEventArgs<ISocket>>? AcceptCompleted;

    public TcpServer(int clientCountMax, int receiveBufferSize)
    {
        //ConnectCountMax = numConnections;
        // allocate buffers such that the maximum number of sockets can have one outstanding read and   
        // write posted to the socket simultaneously    
        //BufferManager = new BufferManager(receiveBufferSize * clientCountMax * OpsToAlloc, receiveBufferSize);
        SocketEventPool = new SocketEventPool(clientCountMax);
        for (int i = 0; i < clientCountMax; i++)
        {
            var readWriteEventArg = new SocketAsyncEventArgs();
            readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            readWriteEventArg.UserToken = new AsyncClientProfile();
            // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object  
            //BufferManager.SetBuffer(readWriteEventArg);
            // add SocketAsyncEventArg to the pool  
            SocketEventPool.Push(readWriteEventArg);
        }
        void IO_Completed(object? sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation is SocketAsyncOperation.Receive)
                ProcessReceive(e);
            else if (e.LastOperation is SocketAsyncOperation.Send)
                ProcessSend(e);
            else
                throw new ArgumentException("The last operation completed on the socket was not a receive or send");
        }
    }

    public void Start(int port, out string message)
    {
        message = "";
        if (IsStart)
            message = "server has started";
        else
        {
            Clients.Clear();
            try
            {
                var localEndPoint = new IPEndPoint(IPAddress.Any, port);
                Core = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Core.Bind(localEndPoint);
                Core.Listen(10);
                StartAccept(null);
                IsStart = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }
    }

    public void Stop()
    {
        if (Core is null)
            return;
        foreach (var client in Clients.Values)
        {
            try
            {
                client.Socket?.Shutdown(SocketShutdown.Send);
            }
            catch { }
        }
        Core.Close();
        IsStart = false;
        lock (Clients)
        {
            Clients.Clear();
        }
    }

    public void CloseClient(string remoteEndPoint)
    {
        try
        {
            if (Clients.TryGetValue(remoteEndPoint, out var client))
                client.Socket?.Shutdown(SocketShutdown.Both);
        }
        catch { }
    }

    /// <summary>
    /// Begins an operation to accept a connection request from the client
    /// </summary>
    /// <param name="acceptEventArg">The context object to use when issuing
    /// the accept operation on the server's listening socket</param>
    public void StartAccept(SocketAsyncEventArgs? acceptEventArg)
    {
        acceptEventArg?.Dispose();
        acceptEventArg = new SocketAsyncEventArgs();
        acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>((sender, e) => ProcessAccept(e));
        Core?.AcceptAsync(acceptEventArg);
    }

    private void ProcessAccept(SocketAsyncEventArgs e)
    {
        try
        {
            var client = new AsyncClientProfile()
            {
                Socket = e.AcceptSocket,
                ConnectTime = DateTime.Now,
                RemoteEndPoint = e.AcceptSocket?.RemoteEndPoint,
                IPAddress = (e.AcceptSocket?.RemoteEndPoint as IPEndPoint)?.Address
            };
            var signature = client.RemoteEndPoint?.ToString();
            ArgumentNullException.ThrowIfNull(signature);
            lock (Clients)
            {
                Clients[signature] = client;
            }
            // Get the socket for the accepted client connection and put it into the   
            // ReadEventArg object user token  
            var readEventArgs = SocketEventPool.Pop();
            readEventArgs.UserToken = client;

            ClientNumberChange?.Invoke(true, client);
            if (e.AcceptSocket is not null && !e.AcceptSocket.ReceiveAsync(readEventArgs))
                ProcessReceive(readEventArgs);
        }
        catch (Exception ex)
        {
            //TODO: use log for ex
        }
        // Accept the next connection request  
        if (e.SocketError is not SocketError.OperationAborted)
            StartAccept(e);
    }

    /// <summary>
    ///  This method is invoked when an asynchronous receive operation completes.
    ///  If the remote host closed the connection, then the socket is closed.
    ///  If data was received then the data is echoed back to the client.
    /// </summary>
    /// <param name="e"></param>
    private void ProcessReceive(SocketAsyncEventArgs e)
    {
        // check if the remote host closed the connection  
        if (e.Buffer is null || e.BytesTransferred <= 0 || e.SocketError is not SocketError.Success)
        {
            CloseClient(e);
            e.UserToken = new AsyncClientProfile();
            SocketEventPool.Push(e);
            return;
        }
        try
        {
            if (e.UserToken is not AsyncClientProfile client)
                return;
            var data = new byte[e.BytesTransferred];
            Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
            lock (client.Buffer)
            {
                client.Buffer.AddRange(data);
            }
            // 这里为什么要用do-while循环?   
            // 如果当客户发送大数据流的时候,e.BytesTransferred的大小就会比客户端发送过来的要小,  
            // 需要分多次接收.所以收到包的时候,先判断包头的大小.够一个完整的包再处理.  
            // 如果客户短时间内发送多个小数据包时, 服务器可能会一次性把他们全收了.  
            // 这样如果没有一个循环来控制,那么只会处理第一个包,  
            // 剩下的包全部留在token.Buffer中了,只有等下一个数据包过来后,才会放出一个来.  
            do
            {
                //判断包的长度  
                var package = client.Buffer.GetRange(0, 4).ToArray();
                int length = BitConverter.ToInt32(package, 0);
                if (length > client.Buffer.Count - 4)
                    //长度不够时,退出循环,让程序继续接收
                    break;
                //包够长时,则提取出来,交给后面的程序去处理  
                package = client.Buffer.GetRange(4, length).ToArray();
                //从数据池中移除这组数据  
                lock (client.Buffer)
                {
                    client.Buffer.RemoveRange(0, length + 4);
                }
                //将数据包交给后台处理,这里你也可以新开个线程来处理.加快速度.  
                ReceiveClientData?.Invoke(client, package);
                //这里API处理完后,并没有返回结果,当然结果是要返回的,却不是在这里, 这里的代码只管接收.  
                //若要返回结果,可在API处理中调用此类对象的SendMessage方法,统一打包发送.不要被微软的示例给迷惑了.  
            } while (client.Buffer.Count > 4);
            // 继续接收. 为什么要这么写,请看Socket.ReceiveAsync方法的说明  
            if (client.Socket is not null && !client.Socket.ReceiveAsync(e))
                ProcessReceive(e);
        }
        catch (Exception ex)
        {
            //TODO: use log for ex
        }
    }

    /// <summary>
    /// This method is invoked when an asynchronous send operation completes.  
    /// The method issues another receive on the socket to read any additional   
    /// data sent from the client  
    /// </summary>
    /// <param name="e"></param>
    private void ProcessSend(SocketAsyncEventArgs e)
    {
        if (e.SocketError is SocketError.Success)
        {
            if (e.UserToken is not AsyncClientProfile client || client.Socket is null)
                return;
            // read the next block of data send from the client  
            var willRaiseEvent = client.Socket.ReceiveAsync(e);
            if (!willRaiseEvent)
                ProcessReceive(e);
        }
        else
        {
            CloseClient(e);
        }
    }

    //关闭客户端  
    private void CloseClient(SocketAsyncEventArgs e)
    {
        var client = e.UserToken as AsyncClientProfile;
        var remote = client?.RemoteEndPoint?.ToString();
        if (client is null || remote is null)
            return;
        lock (Clients)
        {
            Clients.Remove(remote);
        }
        ClientNumberChange?.Invoke(false, client);
        // close the socket associated with the client  
        try
        {
            client.Socket?.Shutdown(SocketShutdown.Send);
            client.Socket?.Close();
        }
        catch { }
        // decrement the counter keeping track of the total number of clients connected to the server
        // Free the SocketAsyncEventArg so they can be reused by another client
        e.UserToken = new AsyncClientProfile();
        SocketEventPool.Push(e);
    }



    /// <summary>  
    /// 对数据进行打包,然后再发送  
    /// </summary>  
    /// <param name="client"></param>  
    /// <param name="message"></param>  
    /// <returns></returns>  
    public void SendMessage(AsyncClientProfile client, byte[] message)
    {
        if (client == null || client.Socket == null || !client.Socket.Connected)
            return;
        try
        {
            //对要发送的消息,制定简单协议,头4字节指定包的大小,方便客户端接收(协议可以自己定)  
            var buffer = new byte[message.Length + 4];
            var length = BitConverter.GetBytes(message.Length);
            Array.Copy(length, buffer, 4);
            Array.Copy(message, 0, buffer, 4, message.Length);
            //token.Socket.Send(buff);  //这句也可以发送, 可根据自己的需要来选择  
            //新建异步发送对象, 发送消息  
            var sendArg = new SocketAsyncEventArgs
            {
                UserToken = client
            };
            sendArg.SetBuffer(buffer, 0, buffer.Length);  //将数据放置进去.  
            client.Socket.SendAsync(sendArg);
        }
        catch (Exception ex)
        {
            //TODO: use log for ex
        }
    }
}
