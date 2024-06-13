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
        IPAddress iPAddress;
        int port;
        int connectTimeout;
        byte[] receiveBuffer = new byte[1024];
        public TcpClientAsyncTool(string ip, int port, int connectTimeout = 2000)
        {
            tcpClient = new TcpClient();
            this.iPAddress = IPAddress.Parse(ip);
            this.port = port;
            this.connectTimeout = connectTimeout;

        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool Connect(Action  connectDelegate,out string errorMsg)
        {
            bool result = false;
            errorMsg = string.Empty;
            try
            {
                IAsyncResult asyncResult = tcpClient.BeginConnect(iPAddress, port, null, null);
                connectDelegate();
                //可以干一些别的事情
                bool success = asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(connectTimeout));//设置连接超时时间为2秒
                tcpClient.EndConnect(asyncResult);
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
                IAsyncResult asyncResult = networkStream.BeginWrite(sendBytes, 0, sendBytes.Length, null, null);
                //可以干一些别的事情
                networkStream.EndWrite(asyncResult);
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
                IAsyncResult iAsyncResult = networkStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, null, null);
                //干一些别的事情
                int readBytes = networkStream.EndRead(iAsyncResult);
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
