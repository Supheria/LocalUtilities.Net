using System.Net;

namespace LocalUtilities.Net.Sockets
{
    public class SocketUdpClient : SocketDgramBase
    {
        public SocketUdpClient(SocketUdpContext context, ISocketDgramHandler socketHandler)
            : base(context, socketHandler)
        {
            Context = context;
            _IsConnected = true;
        }

        private bool _IsConnected;
        public override bool IsConnected
        {
            get
            {
                return _IsConnected;
            }
        }

        public override void Disconnect()
        {
            Context.OnDisconnect();
            OnDisconnected();
            _IsConnected = false;
        }

        public override Task DisconnectAsync()
        {
            return Task.Run(() =>
            {
                Context.OnDisconnect();
                OnDisconnected();
                _IsConnected = false;
            });
        }

        public override IPEndPoint RemoteEndPoint
        {
            get { return Context.RemoteEndPoint; }
        }

        public override IPEndPoint LocalEndPoint
        {
            get { return Context.LocalEndPoint; }
        }
    }
}
