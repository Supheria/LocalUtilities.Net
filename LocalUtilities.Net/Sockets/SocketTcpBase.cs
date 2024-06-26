﻿using System.Net;
using System.Net.Sockets;

namespace LocalUtilities.Net.Sockets
{
    /// <summary>
    /// Socket Tcp基类。
    /// </summary>
    /// <typeparam name="TIn">输入类型。</typeparam>
    /// <typeparam name="TOut">输出类型。</typeparam>
    public abstract class SocketTcpBase : SocketStreamBase, IDisposable
    {
        /// <summary>
        /// 实例化TCP客户端。
        /// </summary>
        /// <param name="socket">Socket套接字。</param>
        /// <param name="socketHandler">Socket处理器。</param>
        protected SocketTcpBase(Socket socket, ISocketStreamHandler socketHandler)
            : this(socket, socketHandler, new SocketNetworkStreamProvider()) { }

        /// <summary>
        /// 实例化TCP客户端。
        /// </summary>
        /// <param name="socket">Socket套接字。</param>
        /// <param name="socketHandler">Socket处理器。</param>
        /// <param name="streamProvider">Socket网络流提供者。</param>
        protected SocketTcpBase(Socket socket, ISocketStreamHandler socketHandler, ISocketStreamProvider streamProvider)
            : base(socket, socketHandler, streamProvider)
        {
            ArgumentNullException.ThrowIfNull(socket);
            ArgumentNullException.ThrowIfNull(socketHandler);
            ArgumentNullException.ThrowIfNull(streamProvider);
            socket.NoDelay = true;
        }

        /// <summary>
        /// 获取Socket是否已连接。
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                return Socket.Connected;
            }
            protected set
            {
                throw new NotSupportedException();
            }
        }

        #region 断开连接

        /// <summary>
        /// 断开与服务器的连接。
        /// </summary>
        public override void Disconnect()
        {
            //判断是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            Socket.Disconnect(false);
            Disconnected(true);
        }

        /// <summary>
        /// 异步断开与服务器的连接。
        /// </summary>
        public override async Task DisconnectAsync()
        {
            //判断是否已连接
            if (!IsConnected)
                throw new SocketException(10057);
            await Task.Factory.FromAsync(Socket.BeginDisconnect(true, null, null), Socket.EndDisconnect);
            Disconnected(true);
        }

        #endregion

        #region 其它

        /// <summary>
        /// 释放资源。
        /// </summary>
        protected override void Disposed()
        {
            if (IsConnected)
                Socket.Disconnect(false);
            Socket.Close();
        }

        /// <summary>
        /// 获取远程终结点地址。
        /// </summary>
        public sealed override IPEndPoint RemoteEndPoint
        {
            get
            {
                if (IsConnected)
                    return (IPEndPoint)Socket.RemoteEndPoint;
                return null;
            }
        }

        /// <summary>
        /// 获取本地终结点地址。
        /// </summary>
        public sealed override IPEndPoint LocalEndPoint
        {
            get
            {
                if (IsConnected)
                    return (IPEndPoint)Socket.LocalEndPoint;
                return null;
            }
        }

        #endregion
    }
}
