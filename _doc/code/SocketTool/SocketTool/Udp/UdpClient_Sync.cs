using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketTool
{
   public  class UdpClient_Sync
    {
        string Ip;
        int Port;
       public  Socket socketClient;
        public bool isConnected = false;
        byte[] receiveBuffer = new byte[1024];
        EndPoint  remoteIpEndPoint;
        public UdpClient_Sync(int localPort,string ip, int port)
        {
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, localPort);
            socketClient.Bind(iPEndPoint);
            this.Ip = ip;
            this.Port = port;
            remoteIpEndPoint = new IPEndPoint(IPAddress .Parse (this.Ip ),this.Port );
        }
      

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool SendData(byte[] bytes, out string errorMsg)
        {
            bool result = false;
            errorMsg = string.Empty;
            try
            {
                socketClient.SendTo(bytes, bytes.Length, SocketFlags.None, remoteIpEndPoint);
                result = true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message + ex.StackTrace;
            }
            return result;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public byte[] ReceiveData(out bool result, out string errorMsg)
        {
            result = false;
            errorMsg = string.Empty;
            byte[] readByteArray = null;
            try
            {
                int readBytes =  socketClient.ReceiveFrom(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ref remoteIpEndPoint);
                if (readBytes == 0)
                {
                    isConnected = false;
                }
                else
                {
                    readByteArray = new byte[readBytes];
                    Array.Copy(receiveBuffer, readByteArray, readBytes);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message + ex.StackTrace;
            }
            return readByteArray;
        }

    }
}
