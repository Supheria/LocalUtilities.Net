using Net;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Net;

public class AsyncUserToken
{
    public IocpServer Server { get; }

    public Socket? AcceptSocket { get; private set; } = null;

    public SocketAsyncEventArgs ReceiveAsyncArgs { get;} = new();

    public SocketAsyncEventArgs SendAsyncArgs { get; } = new();

    public DynamicBufferManager ReceiveBuffer { get; } = new(ConstTabel.InitBufferSize);

    public AsyncSendBufferManager SendBuffer { get; } = new(ConstTabel.InitBufferSize);

    public IocpServerProtocol? Protocol { get; private set; } = null;

    public delegate void AsyncUserTokenEvent();

    public event AsyncUserTokenEvent? OnClosed;

    public SocketInfo SocketInfo { get; } = new();

    public AsyncUserToken(IocpServer server)
    {
        Server = server;
        ReceiveAsyncArgs.UserToken = this;
        SendAsyncArgs.UserToken = this;
        ReceiveAsyncArgs.SetBuffer(new byte[ReceiveBuffer.BufferSize], 0, ReceiveBuffer.BufferSize);
        ReceiveAsyncArgs.Completed += CompleteIO;
        SendAsyncArgs.Completed += CompleteIO;
    }

    private void CompleteIO(object? sender, SocketAsyncEventArgs args)
    {
        if (args.UserToken is not AsyncUserToken userToken)
            return;
        SocketInfo.Active();
        try
        {
            lock (userToken)
            {
                if (args.LastOperation is SocketAsyncOperation.Receive)
                    ProcessReceive();
                else if (args.LastOperation is SocketAsyncOperation.Send)
                    ProcessReceive();
                else
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }
        catch (Exception ex)
        {
            //Program.Logger.ErrorFormat("IO_Completed {0} error, message: {1}", userToken.ConnectSocket, ex.Message);
            //Program.Logger.Error(ex.StackTrace);
        }
    }

    public void ReceiveAsync()
    {
        if (AcceptSocket is not null && !AcceptSocket.ReceiveAsync(ReceiveAsyncArgs))
            ProcessReceive();
    }

    public void SendAsync(byte[] buffer, int offset, int count)
    {
        SendAsyncArgs.SetBuffer(buffer, offset, count);
        if (AcceptSocket is not null && !AcceptSocket.SendAsync(SendAsyncArgs))
            ProcessSend();
    }

    [MemberNotNullWhen(true, nameof(AcceptSocket))]
    public bool ProcessAccept(Socket? acceptSocket)
    {
        if (acceptSocket is null)
            return false;
        AcceptSocket = acceptSocket;
        // 设置TCP Keep-alive数据包的发送间隔为10秒
        AcceptSocket.IOControl(IOControlCode.KeepAliveValues, KeepAlive(1, 1000 * 10, 1000 * 10), null);
        ReceiveAsyncArgs.AcceptSocket = acceptSocket;
        SendAsyncArgs.AcceptSocket = acceptSocket;
        SocketInfo.Connect(acceptSocket);
        return true;
    }

    /// <summary>
    /// keep alive 设置
    /// </summary>
    /// <param name="onOff">是否开启（1为开，0为关）</param>
    /// <param name="keepAliveTime">当开启keep-alive后，经过多长时间（ms）开启侦测</param>
    /// <param name="keepAliveInterval">多长时间侦测一次（ms）</param>
    /// <returns>keep alive 输入参数</returns>
    private static byte[] KeepAlive(int onOff, int keepAliveTime, int keepAliveInterval)
    {
        byte[] buffer = new byte[12];
        BitConverter.GetBytes(onOff).CopyTo(buffer, 0);
        BitConverter.GetBytes(keepAliveTime).CopyTo(buffer, 4);
        BitConverter.GetBytes(keepAliveInterval).CopyTo(buffer, 8);
        return buffer;
    }

    [MemberNotNullWhen(true, nameof(Protocol))]
    [MemberNotNullWhen(true, nameof(AcceptSocket))]
    public void ProcessReceive()
    {
        if (AcceptSocket is null)
            goto CLOSE;
        if (ReceiveAsyncArgs.Buffer is null || ReceiveAsyncArgs.BytesTransferred <= 0 || ReceiveAsyncArgs.SocketError is not SocketError.Success)
            goto CLOSE;
        if (!BuildProtocol(Server))
            goto CLOSE;
        SocketInfo.Active();
        var offset = ReceiveAsyncArgs.Offset + 1;
        var size = ReceiveAsyncArgs.BytesTransferred - 1;
        // 处理接收数据
        if (size > 0 && !Protocol.ProcessReceive(ReceiveAsyncArgs.Buffer, offset, size))
            goto CLOSE;
        bool willRaiseEvent = AcceptSocket.ReceiveAsync(ReceiveAsyncArgs); //投递接收请求
        if (!AcceptSocket.ReceiveAsync(ReceiveAsyncArgs))
            ProcessReceive(ReceiveAsyncArgs);
        return;
        CLOSE:
        Close();
    }

    [MemberNotNullWhen(true, nameof(Protocol))]
    private bool BuildProtocol(IocpServer server)
    {
        if (Protocol is not null)
            return true;
        if (ReceiveAsyncArgs.Buffer is null)
            return false;
        var protocolType = (IocpProtocolTypes)ReceiveAsyncArgs.Buffer[ReceiveAsyncArgs.Offset];
        Protocol = protocolType switch
        {
            //IocpProtocolTypes.Upload => new UploadProtocol(server, this),
            //IocpProtocolTypes.Download => new DownloadProtocol(server, this),
            //IocpProtocolTypes.RemoteStream => new FileProcessProtocol(server, this),
            //IocpProtocolTypes.Throughput => new ThroughputProtocol(server, this),
            IocpProtocolTypes.FullHandler => new ServerFullHandlerProtocol(server, this),
            _ => null
        };
        if (Protocol is not null)
        {
            //ServerInstance.Logger.InfoFormat("Building socket invoke element {0}.Local Address: {1}, Remote Address: {2}",
            //    Protocol, AcceptSocket.LocalEndPoint, AcceptSocket.RemoteEndPoint);
            return true;
        }
        return false;
        //return Protocol is not null;
    }

    public void ProcessSend()
    {
        if (Protocol is null)
            goto CLOSE;
        if (SendAsyncArgs.SocketError is not SocketError.Success)
            goto CLOSE;
        SocketInfo.Active();
        Protocol.ProcessSend();
        return;
        CLOSE:
        Close();
    }

    public void Close()
    {
        if (AcceptSocket is null)
            return;
        try
        {
            AcceptSocket.Shutdown(SocketShutdown.Both);
        }
        catch (Exception ex)
        {
            //Program.Logger.ErrorFormat("CloseClientSocket Disconnect client {0} error, message: {1}", socketInfo, ex.Message);
        }
        AcceptSocket.Close();
        AcceptSocket = null;
        ReceiveAsyncArgs.AcceptSocket = null;
        SendAsyncArgs.AcceptSocket = null;
        ReceiveBuffer.Clear(ReceiveBuffer.DataCount);
        SendBuffer.ClearPacket();
        Protocol?.Dispose();
        Protocol = null;
        SocketInfo.Disconnect();
        OnClosed?.Invoke();
    }
}
