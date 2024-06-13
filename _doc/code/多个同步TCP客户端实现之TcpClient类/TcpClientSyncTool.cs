using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace 同步tcp客户端
{
    public class TcpClientAsyncTool
    {
        TcpClient tcpClient;
        public bool isConnected = false;
        IPEndPoint iPEndPoint;
        byte[] receiveBuffer = new byte[1024];
        public TcpClientAsyncTool(string ip, int port, int sendTimeout = 2000, int receiveTimeout = 2000)
        {
            tcpClient = new TcpClient();
            IPAddress iPAddress = IPAddress.Parse(ip);
            iPEndPoint = new IPEndPoint(iPAddress, port);
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
                tcpClient.Connect(iPEndPoint);
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
        /// 断开连接
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool DisConnect(out string errorMsg)
        {
            bool result = false;
            errorMsg = string.Empty;
            try
            {
                tcpClient.Close();
                isConnected = result;
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
        public bool SendData(byte[] sendBytes, out string errorMsg)
        {
            bool result = false;
            errorMsg = string.Empty;
            try
            {
                NetworkStream networkStream = tcpClient.GetStream();
                networkStream.Write(sendBytes, 0, sendBytes.Length);
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
                NetworkStream networkStream = tcpClient.GetStream();
                int readBytes = networkStream.Read(receiveBuffer, 0, receiveBuffer.Length);
                readByteArray = new byte[readBytes];
                Array.Copy(receiveBuffer, readByteArray, readBytes);
                result = true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message + ex.StackTrace;
            }
            return readByteArray;
        }
    }
}
