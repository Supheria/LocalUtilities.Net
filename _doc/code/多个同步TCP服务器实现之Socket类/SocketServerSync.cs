using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace 多个同步TCP服务器实现之Socket类
{
    public class SocketServerSync
    {
        Socket socketServer;
        Dictionary<string, Socket> socketClientDic = new Dictionary<string, Socket>();
        Action<string> UpdateUiDelegate;
        Action<string> AddClientlistDelegate;
        Action<string> RemoveClientlistDelegate;

        public SocketServerSync(IPAddress iPAddress, int port, Action<string> UpdateUi, Action<string> AddClientlist, Action<string> RemoveClientlist)
        {
            UpdateUiDelegate = UpdateUi;
            AddClientlistDelegate = AddClientlist;
            RemoveClientlistDelegate = RemoveClientlist;

            EndPoint endPoint = new IPEndPoint(iPAddress, port);
            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketServer.Bind(endPoint);
            socketServer.Listen(20);
            Task.Run( new Action ( StartMonitor));
        }
        /// <summary>
        /// 开始监听客户端连接
        /// </summary>
        public void StartMonitor()
        {
            while (true)
            {
                try
                {
                    Socket socketClient = socketServer.Accept();
                    Task.Run(() => StartReceive(socketClient));
                }
                catch (Exception ex)
                {
                    break;
                }
            }

        }

        /// <summary>
        /// 监听客户端发送的数据
        /// </summary>
        /// <param name="socketClient"></param>
        public void StartReceive(Socket socketClient)
        {

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

            while (true)
            {
                try
                {
                    NetworkStream networkStream = new NetworkStream(socketClient);
                    byte[] buffer = new byte[1024];
                    int nums = networkStream.Read(buffer, 0, buffer.Length);
                    if (nums == 0)
                    {
                        UpdateUiDelegate("与客户端断开连接");
                        if (socketClientDic.ContainsKey (remoteInfo))
                        {
                            socketClientDic.Remove(remoteInfo);
                            RemoveClientlistDelegate(remoteInfo);
                        }
                        break;
                    }
                    string receiveStr = Encoding.ASCII.GetString(buffer, 0, nums);

                    UpdateUiDelegate($"接收到客户端：{remoteInfo}的数据：{receiveStr}");
                }
                catch (Exception ex)
                {
                    UpdateUiDelegate("与客户端断开连接");
                    if (socketClientDic.ContainsKey(remoteInfo))
                    {
                        socketClientDic.Remove(remoteInfo);
                        RemoveClientlistDelegate(remoteInfo);
                    }
                    break;
                }
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
        public void SendData(  string remoteInfo, string str)
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
}
