using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketTool
{
    public class TcpClient_Sync
    {

        string Ip;
        int Port;
     public    Socket socketClient;
        public bool isConnected = false;
        byte[] receiveBuffer = new byte[1024];
        public TcpClient_Sync(string ip, int port)
        {
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Ip = ip;
            this.Port = port;
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool Connect(out string errorMsg)
        {
            bool result = false;
            errorMsg = string.Empty;
            try
            {
                IPAddress iPAddress = IPAddress.Parse(Ip);
                IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, this.Port);
                socketClient.Connect(iPEndPoint);
                result = true;
                isConnected = result;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return result;
        }
        /// <summary>
        /// 断开服务器连接
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool DisConnect(out string errorMsg)
        {
            bool result = false;
            errorMsg = string.Empty;
            isConnected = false ;
            try
            {
                socketClient.Shutdown(SocketShutdown.Both);
                socketClient.Close();
                socketClient.Dispose();
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return result;
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
                NetworkStream networkStream = new NetworkStream(socketClient);
                networkStream.Write(bytes, 0, bytes.Length);
                networkStream.Flush();
                networkStream.Dispose();
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
                NetworkStream networkStream = new NetworkStream(socketClient);
                int readBytes = networkStream.Read(receiveBuffer, 0, receiveBuffer.Length);
                if(readBytes==0)
                {
                    isConnected = false;
                }
                else
                {
                    readByteArray = new byte[readBytes];
                    Array.Copy(receiveBuffer, readByteArray, readBytes);
                    networkStream.Dispose();
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
