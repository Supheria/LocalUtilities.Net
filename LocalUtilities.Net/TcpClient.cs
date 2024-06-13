using LocalUtilities.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalUtilities.Net;

public class TcpClient
{
    private const int BuffSize = 1024;

    private Socket? Core { get; set; }

    /// <summary>
    /// Listener endpoint
    /// </summary>
    private IPEndPoint? ServerRemoteEndPoint { get; set; } = null;

    /// <summary>
    /// Signals a connection
    /// </summary>
    private static AutoResetEvent? AutoConnectEvent { get; set; } = null;

    BufferManager BufferManager { get; }

    List<byte> Buffer { get; } = [];

    public bool IsConnect => Core is not null && Core.Connected;

    private List<SocketAsyncEventArgs> SendEventArgsList { get; } = [];

    private SocketAsyncEventArgs ReceiveEventArgs { get; } = new();

    public delegate void OnReceiveServerData(byte[] receiveBuff);
    public delegate void OnServerStop();

    public event OnReceiveServerData? ReceiveServerDataEvent;
    public event OnServerStop? ServerStopEvent;


    // Create an uninitialized client instance.
    // To start the send/receive processing call the
    // Connect method followed by SendReceive method.
    public TcpClient()
    {
        // Instantiates the endpoint and socket.
        BufferManager = new BufferManager(BuffSize * 2, BuffSize);
    }

    /// <summary>
    /// 连接到主机
    /// </summary>
    /// <returns>0.连接成功, 其他值失败,参考SocketError的值列表</returns>
    public void Connect(string serverAddress, int serverPort, out string message)
    {
        message = "";
        try
        {
            ServerRemoteEndPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            Core = new Socket(ServerRemoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var connectArgs = new SocketAsyncEventArgs
            {
                UserToken = Core,
                RemoteEndPoint = ServerRemoteEndPoint
            };
            connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ConnectCompleted);
            Core?.ConnectAsync(connectArgs);
            AutoConnectEvent = new(false);
            // 阻塞. 让程序在这里等待,直到连接响应后再返回连接结果
            AutoConnectEvent.WaitOne();
        }
        catch(Exception ex)
        {
            message = ex.Message;
        }
    }

    /// Disconnect from the host.
    public void Disconnect(out string message)
    {
        message = "";
        try
        {
            Core?.Disconnect(false);
            Core?.Close();
            AutoConnectEvent?.Close();
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }
    }

    // Calback for connect operation
    public void ConnectCompleted(object? sender, SocketAsyncEventArgs e)
    {
        // Signals the end of connection.
        AutoConnectEvent.Set(); //释放阻塞.
                                // Set the flag for socket connected.
                                //connected = (e.SocketError == SocketError.Success);
                                //如果连接成功,则初始化socketAsyncEventArgs
        if (e.SocketError is not SocketError.Success)
            return;
        BufferManager.Relocate();
        //发送参数
        //initSendArgs();
        //接收参数
        ReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
        ReceiveEventArgs.UserToken = e.UserToken;
        BufferManager.SetBuffer(ReceiveEventArgs);

        //启动接收,不管有没有,一定得启动.否则有数据来了也不知道.
        if (!e.ConnectSocket.ReceiveAsync(ReceiveEventArgs))
            ProcessReceive(ReceiveEventArgs);
    }

    void IO_Completed(object? sender, SocketAsyncEventArgs e)
    {
        SocketAsyncEventArgs mys = (SocketAsyncEventArgs)e;
        // determine which type of operation just completed and call the associated handler
        switch (e.LastOperation)
        {
            case SocketAsyncOperation.Receive:
                ProcessReceive(e);
                break;
            case SocketAsyncOperation.Send:
                //mys.IsUsing = false; //数据发送已完成.状态设为False
                ProcessSend(e);
                break;
            default:
                throw new ArgumentException("The last operation completed on the socket was not a receive or send");
        }
    }

    // This method is invoked when an asynchronous receive operation completes. 
    // If the remote host closed the connection, then the socket is closed.  
    // If data was received then the data is echoed back to the client.
    //
    private void ProcessReceive(SocketAsyncEventArgs e)
    {
        try
        {
            // check if the remote host closed the connection
            Socket token = (Socket)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                //读取数据
                byte[] data = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                lock (Buffer)
                {
                    Buffer.AddRange(data);
                }

                do
                {
                    //注意: 这里是需要和服务器有协议的,我做了个简单的协议,就是一个完整的包是包长(4字节)+包数据,便于处理,当然你可以定义自己需要的; 
                    //判断包的长度,前面4个字节.
                    byte[] lenBytes = Buffer.GetRange(0, 4).ToArray();
                    int packageLen = BitConverter.ToInt32(lenBytes, 0);
                    if (packageLen <= Buffer.Count - 4)
                    {
                        //包够长时,则提取出来,交给后面的程序去处理
                        byte[] rev = Buffer.GetRange(4, packageLen).ToArray();
                        //从数据池中移除这组数据,为什么要lock,你懂的
                        lock (Buffer)
                        {
                            Buffer.RemoveRange(0, packageLen + 4);
                        }
                        //将数据包交给前台去处理
                        DoReceiveEvent(rev);
                    }
                    else
                    {   //长度不够,还得继续接收,需要跳出循环
                        break;
                    }
                } while (Buffer.Count > 4);
                //注意:你一定会问,这里为什么要用do-while循环?   
                //如果当服务端发送大数据流的时候,e.BytesTransferred的大小就会比服务端发送过来的完整包要小,  
                //需要分多次接收.所以收到包的时候,先判断包头的大小.够一个完整的包再处理.  
                //如果服务器短时间内发送多个小数据包时, 这里可能会一次性把他们全收了.  
                //这样如果没有一个循环来控制,那么只会处理第一个包,  
                //剩下的包全部留在m_buffer中了,只有等下一个数据包过来后,才会放出一个来.
                //继续接收
                if (!token.ReceiveAsync(e))
                    this.ProcessReceive(e);
            }
            else
            {
                ProcessError(e);
            }
        }
        catch (Exception xe)
        {
            Console.WriteLine(xe.Message);
        }
    }

    // This method is invoked when an asynchronous send operation completes.  
    // The method issues another receive on the socket to read any additional 
    // data sent from the client
    //
    // <param name="e"></param>
    private void ProcessSend(SocketAsyncEventArgs e)
    {
        if (e.SocketError != SocketError.Success)
        {
            ProcessError(e);
        }
    }

    // Close socket in case of failure and throws
    // a SockeException according to the SocketError.
    private void ProcessError(SocketAsyncEventArgs e)
    {
        //var core = e.UserToken as Socket;
        if (IsConnect)
        {
            // close the socket associated with the client
            try
            {
                //core.Shutdown(SocketShutdown.Both);
                Core?.Close();
            }
            catch (Exception)
            {
                // throws if client process has already closed
            }
            //finally
            //{
            //    if (core.Connected)
            //    {
            //        core.Close();
            //    }
            //    //Core = null;
            //}
        }
        //这里一定要记得把事件移走,如果不移走,当断开服务器后再次连接上,会造成多次事件触发.
        foreach (SocketAsyncEventArgs arg in SendEventArgsList)
            arg.Completed -= IO_Completed;
        ReceiveEventArgs.Completed -= IO_Completed;

        ServerStopEvent?.Invoke();
    }

    public void Send(byte[] sendBuffer)
    {
        if (!IsConnect)
            return;
        //先对数据进行包装,就是把包的大小作为头加入,这必须与服务器端的协议保持一致,否则造成服务器无法处理数据.
        byte[] buff = new byte[sendBuffer.Length + 4];
        Array.Copy(BitConverter.GetBytes(sendBuffer.Length), buff, 4);
        Array.Copy(sendBuffer, 0, buff, 4, sendBuffer.Length);
        //查找有没有空闲的发送SocketEventArgsEx,有就直接拿来用,没有就创建新的.So easy!
        //SocketAsyncEventArgs sendArgs = SendEventArgsList.Find(a => a.IsUsing == false);
        using var sendArgs = new SocketAsyncEventArgs
        {
            UserToken = Core,
            RemoteEndPoint = ServerRemoteEndPoint
        };
        sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
        //要锁定,不锁定让别的线程抢走了就不妙了.
        lock (sendArgs)
        {
            sendArgs.SetBuffer(buff, 0, buff.Length);
        }
        Core?.SendAsync(sendArgs);
    }

    /// <summary>
    /// 使用新进程通知事件回调
    /// </summary>
    /// <param name="buff"></param>
    private void DoReceiveEvent(byte[] buff)
    {
        //if (ReceiveServerDataEvent == null) return;
        ReceiveServerDataEvent?.Invoke(buff); //可直接调用.
        //但我更喜欢用新的线程,这样不拖延接收新数据.
        //Thread thread = new(new ParameterizedThreadStart((obj) =>
        //{
        //    ReceiveServerDataEvent((byte[])obj);
        //}))
        //{
        //    IsBackground = true
        //};
        //thread.Start(buff);
    }
}
