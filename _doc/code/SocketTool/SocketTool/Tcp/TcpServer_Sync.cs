using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketTool
{
  public   class TcpServer_Sync
    {
        Dictionary<string, Socket> dicSocket = new Dictionary<string, Socket>();
        Socket socket;
        private int Port;
        byte[] ReceiveBuffer = new byte[2024];
        TcpServer_Sync tcpServer_Sync;

        
        public   TcpServer_Sync(int port)
        {
            this.Port = port;
        }

        public void StartMonitor()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any , Port);
            socket.Bind(iPEndPoint);
            socket.Listen(10);
            Thread thread = new Thread(new ParameterizedThreadStart(Run));
            thread.IsBackground = true;
            thread.Start(socket);
        }

        /// <summary>
        /// 监控客户端连接
        /// </summary>
        /// <param name="obj"></param>
        private void Run(object obj)
        {
            while (true)
            {
                Socket socket = (Socket)obj;
                Socket socketClient = socket.Accept();
                string key = socketClient.RemoteEndPoint.ToString();
             

                dicSocket.Add(key, socketClient);


                GlobalDelegate.AddTcpServerClientDelegate(socketClient, key, Port);


               Thread thread = new Thread(new ParameterizedThreadStart(Communication));
                thread.IsBackground = true;
                thread.Start(socketClient);
            }
        }

        /// <summary>
        /// 和客户端通信
        /// </summary>
        /// <param name="socketClient"></param>
        private void Communication(object socketClient)
        {
            while (true)
            {
                try
                {
                    Socket socket = (Socket)socketClient;
                    NetworkStream networkStream = new NetworkStream(socket);
                    int readByteNums = networkStream.Read(ReceiveBuffer, 0, ReceiveBuffer.Length);
                    networkStream.Dispose();
                    if (readByteNums == 0)
                    {
                        string key = socket.RemoteEndPoint.ToString();
                        if (dicSocket.ContainsKey(key))
                        {
                            dicSocket.Remove(key);
                        }
                    }
                    else
                    {
                        byte[] realBytes = new byte[readByteNums];
                        Array.Copy(ReceiveBuffer, realBytes, readByteNums);
                        string str = Encoding.ASCII.GetString(realBytes);
                        GlobalDelegate.updateTcpServerReceiveStrDelegate(socket.RemoteEndPoint.ToString () , str);

                      
                    }
                }
                catch (Exception ex)
                {
                    Socket socket = (Socket)socketClient;
                    string key = socket.RemoteEndPoint.ToString();
                    if (dicSocket.ContainsKey(key))
                    {
                        dicSocket.Remove(key);
                    }
                }
            }
        }
    }
}
