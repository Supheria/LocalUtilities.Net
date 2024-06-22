using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Net;

public class AsyncUserToken
{
    public SocketAsyncEventArgs ReceiveAsyncArgs { get; } = new();

    public SocketAsyncEventArgs SendAsyncArgs { get; } = new();

    public DynamicBufferManager ReceiveBuffer { get; } = new(ConstTabel.InitBufferSize);

    public AsyncSendBufferManager SendBuffer { get; } = new(ConstTabel.InitBufferSize);

    public IocpProtocol? Protocol { get; private set; } = null;

    public Socket? AcceptSocket { get; private set; }

    public SocketInfo SocketInfo { get; } = new();

    public AsyncUserToken(int asyncReceiveBufferSize, Action<SocketAsyncEventArgs> processReceive, Action<SocketAsyncEventArgs> processSend)
    {
        ReceiveAsyncArgs.UserToken = this;
        ReceiveAsyncArgs.SetBuffer(new byte[asyncReceiveBufferSize], 0, asyncReceiveBufferSize);
        SendAsyncArgs.UserToken = this;
        ReceiveAsyncArgs.Completed += IO_Completed;
        SendAsyncArgs.Completed += IO_Completed;
        void IO_Completed(object? sender, SocketAsyncEventArgs asyncEventArgs)
        {
            if (asyncEventArgs.UserToken is not AsyncUserToken userToken)
                return;
            SocketInfo.Active();
            try
            {
                lock (userToken)
                {
                    if (asyncEventArgs.LastOperation == SocketAsyncOperation.Receive)
                        processReceive(asyncEventArgs);
                    else if (asyncEventArgs.LastOperation == SocketAsyncOperation.Send)
                        processSend(asyncEventArgs);
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
    }

    public void ProcessAccept(Socket acceptSocket)
    {
        AcceptSocket = acceptSocket;
        ReceiveAsyncArgs.AcceptSocket = acceptSocket;
        SendAsyncArgs.AcceptSocket = acceptSocket;
        SocketInfo.Connect(acceptSocket);
    }

    [MemberNotNullWhen(true, nameof(Protocol))]
    [MemberNotNullWhen(true, nameof(AcceptSocket))]
    public bool ProcessReceive(IocpServer server, [NotNullWhen(true)] out byte[]? receiveBuffer)
    {
        receiveBuffer = null;
        if (AcceptSocket is null)
            return false;
        if (ReceiveAsyncArgs.Buffer is null || ReceiveAsyncArgs.BytesTransferred <= 0 || ReceiveAsyncArgs.SocketError is not SocketError.Success)
            return false;
        if (!BuildProtocol(server))
            return false;
        SocketInfo.Active();
        receiveBuffer = ReceiveAsyncArgs.Buffer;
        return true;
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
            IocpProtocolTypes.Upload => new UploadProtocol(server, this),
            IocpProtocolTypes.Download => new DownloadProtocol(server, this),
            IocpProtocolTypes.RemoteStream => new FileProcessProtocol(server, this),
            IocpProtocolTypes.Throughput => new ThroughputProtocol(server, this),
            _ => null
        };
        return Protocol is not null;
    }

    public void ProcessSend()
    {
        if (Protocol is null)
            return;
        SocketInfo.Active();
        Protocol.ProcessSend();
    }

    public void Close(Action<IocpProtocol?>? onClosingProtocol)
    {
        try
        {
            AcceptSocket?.Shutdown(SocketShutdown.Both);
        }
        catch (Exception ex)
        {
            //Program.Logger.ErrorFormat("CloseClientSocket Disconnect client {0} error, message: {1}", socketInfo, ex.Message);
        }
        AcceptSocket?.Close();
        AcceptSocket = null;
        ReceiveAsyncArgs.AcceptSocket = null;
        SendAsyncArgs.AcceptSocket = null;
        ReceiveBuffer.Clear(ReceiveBuffer.DataCount);
        SendBuffer.ClearPacks();
        onClosingProtocol?.Invoke(Protocol);
        Protocol?.Dispose();
        Protocol = null;
        SocketInfo.Disconnect();
    }
}
