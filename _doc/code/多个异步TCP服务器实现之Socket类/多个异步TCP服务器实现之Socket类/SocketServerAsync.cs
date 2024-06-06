using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace 多个异步TCP服务器实现之Socket类
{
    public  class SocketServerAsync
    {
        Socket socketServer;
        Dictionary<string, Socket> socketClientDic = new Dictionary<string, Socket>();
        Action<string> UpdateUiDelegate;
        Action<string> AddClientlistDelegate;
        Action<string> RemoveClientlistDelegate;

        public SocketServerAsync (IPAddress iPAddress, int port, Action<string> UpdateUi, Action<string> AddClientlist, Action<string> RemoveClientlist)
        {
            UpdateUiDelegate = UpdateUi;
            AddClientlistDelegate = AddClientlist;
            RemoveClientlistDelegate = RemoveClientlist;

            EndPoint endPoint = new IPEndPoint(iPAddress, port);
            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketServer.Bind(endPoint);
            socketServer.Listen(20);
            Task task = new Task(AcceptMessage);
            task.Start();
        }


        /// <summary>
        /// 接收客户端的连接
        /// </summary>
        private void AcceptMessage()
        {
            IAsyncResult iAsyncResult = socketServer.BeginAccept(EndAcceptAsyncCallBack, null);
        }



        /// <summary>
        /// 结束客户端的连接
        /// </summary>
        /// <param name="ar"></param>
        private void EndAcceptAsyncCallBack(IAsyncResult ar)
        {
            try
            {
               Socket socketClient = socketServer.EndAccept(ar);
                string remoteInfo = socketClient.RemoteEndPoint.ToString();
                if (socketClientDic.ContainsKey(remoteInfo))//如果已经存在则替换
                {
                    socketClientDic[remoteInfo] = socketClient;
                }
                else
                {
                    socketClientDic.Add(remoteInfo, socketClient);//如果不存在则加入
                }
                AddClientlistDelegate(remoteInfo);
                UpdateUiDelegate($"客户端已连接{socketClient.RemoteEndPoint.ToString()}");
                RecievDataAsync(socketClient);//接收本客户端的数据
                AcceptMessage();//等待别的客户端连接
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// 接收客户端的数据
        /// </summary>
        private void RecievDataAsync(Socket socketClient)
        {
            NetworkStream networkStream = new NetworkStream(socketClient);
            byte[] buffer = new byte[1024];
            ReceiveDataParam receiveDataParam = new ReceiveDataParam(buffer, socketClient);
            IAsyncResult iAsyncResult = networkStream.BeginRead(buffer, 0, buffer.Length, EndReadAsyncCallBack, receiveDataParam);
            UpdateUiDelegate("服务器可以先做别的事情");
        }

        /// <summary>
        /// 处理客户端发送的数据
        /// </summary>
        /// <param name="ar"></param>
        private void EndReadAsyncCallBack(IAsyncResult ar)
        {
            ReceiveDataParam receiveDataParam = (ReceiveDataParam) ar.AsyncState;
            string remoteInfo = receiveDataParam.SocketClient.RemoteEndPoint.ToString();
            NetworkStream networkStream = new NetworkStream(receiveDataParam.SocketClient );
            try
            {
                int readByteNums = networkStream.EndRead(ar);
                if (readByteNums == 0)
                {
                    UpdateUiDelegate("与客户端断开连接");
                  
                    if (socketClientDic.ContainsKey(remoteInfo))
                    {
                        socketClientDic.Remove(remoteInfo);
                        RemoveClientlistDelegate(remoteInfo);
                    }
                    AcceptMessage();
                }
                else
                {
                    byte[] readReadBytes = new byte[readByteNums];
                    byte[] receiveData = (byte[])receiveDataParam.Bytes ;
                    Array.Copy(receiveData, 0, readReadBytes, 0, readByteNums);
                    string receiveStr = Encoding.ASCII.GetString(receiveData);
                    UpdateUiDelegate($"接收到客户端：{remoteInfo}的数据：{receiveStr}"   );
                    RecievDataAsync(receiveDataParam.SocketClient);
                }
            }
            catch (Exception ex)
            {
                UpdateUiDelegate($"与客户端断开连接,{ex.Message }");
                if (socketClientDic.ContainsKey(remoteInfo))
                {
                    socketClientDic.Remove(remoteInfo);
                    RemoveClientlistDelegate(remoteInfo);
                }
                AcceptMessage();
            }
        }


        /// <summary>
        /// 停止监听
        /// </summary>
        public void StopMonitor()
        {
            socketServer.Close();
            foreach (var item in socketClientDic)
            {
                item.Value.Close();
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="remoteInfo"></param>
        /// <param name="str"></param>
        public void SendData(string remoteInfo, string str)
        {
            Socket socket = socketClientDic[remoteInfo];
            NetworkStream networkStream = new NetworkStream(socket);
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            try
            {
                networkStream.Write(bytes, 0, bytes.Length);
                networkStream.Flush();
            }
            catch
            {
                UpdateUiDelegate("发送失败");
            }
        }

    }


  public   class ReceiveDataParam
    {
        public byte[] Bytes;
        public Socket SocketClient;

      public   ReceiveDataParam(byte[] bytes, Socket socketClient)
        {
            this.Bytes = bytes;
            this.SocketClient = socketClient;
        }
    }
}
