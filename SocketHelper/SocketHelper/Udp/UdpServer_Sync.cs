using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketHelper
{
    public class UdpServer_Sync
    {

        Action<string> UpdateReceiveStr;//udp接收的数据更新到界面上

       public  Socket socketClient;
        public bool isConnected = false;
        byte[] receiveBuffer = new byte[1024];

      public   EndPoint remoteIpEndPoint;
        public UdpServer_Sync(int localPort,Action<string> updateReceiveStr)
        {
            this.UpdateReceiveStr = updateReceiveStr;
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, localPort);
            socketClient.Bind(iPEndPoint);

            remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Task.Run(new Action(ReceiveData));
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
        public void  ReceiveData()
        {
            bool result = false;
            string errorMsg = string.Empty;
            byte[] readByteArray = null;

            while (true )
            {
                try
                {
                    int readBytes = socketClient.ReceiveFrom(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ref remoteIpEndPoint);
                    if (readBytes == 0)
                    {
                        isConnected = false;
                    }
                    else
                    {
                        readByteArray = new byte[readBytes];
                        Array.Copy(receiveBuffer, readByteArray, readBytes);
                        string receiveStr = Encoding.ASCII.GetString(readByteArray);
                        UpdateReceiveStr(receiveStr);
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message + ex.StackTrace;
                }
            }
          
            
        }


    }
}
