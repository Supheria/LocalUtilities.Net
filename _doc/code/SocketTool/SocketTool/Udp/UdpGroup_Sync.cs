using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketTool
{
   public  class UdpGroup_Sync
    {
        Socket socket;
        IPAddress address ;
        public int Port;
        IPEndPoint multiIpEndpoint;
        Action<string> UpdateReceiveStr;//更新接收的数据到界面
        EndPoint remoteIPEndPoint;
        public UdpGroup_Sync(string ip,int port, Action<string> updateReceiveStr)
        {
            UpdateReceiveStr = updateReceiveStr;
            this.Port = port;
            address = IPAddress.Parse(ip);//组播地址
            multiIpEndpoint = new IPEndPoint(IPAddress.Any, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(multiIpEndpoint);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(address));
            Task.Run(new Action (ReceiveMessage));  
        }
        /// <summary>
        /// 接收多播组数据
        /// </summary>
        public  void ReceiveMessage()
        {
             remoteIPEndPoint = (EndPoint)multiIpEndpoint;
            byte[] bytes = new byte[1024];
            while (true)
            {
                socket.ReceiveFrom(bytes, ref remoteIPEndPoint);//这样就指定了Udp服务器只能接受"234.5.6.1"和"234.5.6.2"这两个地址的客户端发来的数据
                string str = Encoding.ASCII.GetString(bytes);
                UpdateReceiveStr(str);
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="str"></param>
        public void SendMessage(string str)
        {

            socket.SendTo(Encoding.ASCII.GetBytes(str), SocketFlags.None, remoteIPEndPoint);
        }

    }
}
