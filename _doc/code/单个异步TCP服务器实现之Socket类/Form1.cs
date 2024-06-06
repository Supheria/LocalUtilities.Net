using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 单个异步TCP服务器实现之Socket类
{
    public partial class Form1 : Form
    {
        Socket socketServer;
        Socket socketClient;
        delegate void UpdateDelegate(string str);
     

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_Listen_Click(object sender, EventArgs e)
        {
            if (btn_Listen.Text == "开始监听")
            {
                string ipStr = tbx_IP.Text.Trim();
                IPAddress iPAddress;
                bool isSuccess = IPAddress.TryParse(ipStr, out iPAddress);
                if (!isSuccess)
                    return;
                string portStr = tbx_Port.Text.Trim();
                int port;
                isSuccess = int.TryParse(portStr, out port);
                if (!isSuccess)
                    return;

                EndPoint endPoint = new IPEndPoint(iPAddress, port);
                socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketServer.Bind(endPoint);
                socketServer.Listen(20);
                UpdateUi($"正在监听{iPAddress.ToString()},{port}");
                btn_Listen.Text = "停止监听";
              
                Task task = new Task(AcceptMessage);
                task.Start();
            }
            else
            {
                btn_Listen.Text = "开始监听";
                try
                {
                    socketClient.Close();
                    socketServer.Close();
                }
                catch
                {

                }
            }
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
                socketClient = socketServer.EndAccept(ar);
                UpdateUi($"客户端已连接{socketClient.RemoteEndPoint.ToString()}");
                RecievDataAsync();
            }
            catch (Exception  ex )
            {

            }
        }

        /// <summary>
        /// 接收客户端的数据
        /// </summary>
        private void RecievDataAsync()
        {
            NetworkStream networkStream = new NetworkStream(socketClient);
            byte[] buffer = new byte[1024];
            IAsyncResult iAsyncResult = networkStream.BeginRead(buffer, 0, buffer.Length, EndReadAsyncCallBack, buffer);
            UpdateUi("服务器可以先做别的事情");
        }

        /// <summary>
        /// 处理客户端发送的数据
        /// </summary>
        /// <param name="ar"></param>
        private void EndReadAsyncCallBack(IAsyncResult ar)
        {
            NetworkStream networkStream = new NetworkStream(socketClient);
            try
            {
                int readByteNums = networkStream.EndRead(ar);
                if (readByteNums == 0)
                {

                    UpdateUi("与客户端断开连接");
                    AcceptMessage();
                }
                else
                {
                    byte[] readReadBytes = new byte[readByteNums];
                    byte[] receiveData = (byte[])ar.AsyncState;
                    Array.Copy(receiveData, 0, readReadBytes, 0, readByteNums);
                    UpdateReceive(Encoding.ASCII.GetString(receiveData));
                    RecievDataAsync();
                }
            }
            catch (Exception ex)
            {
                UpdateUi($"与客户端断开连接,{ex.Message }");
                AcceptMessage();
            }
        }



        private void UpdateReceive(string str)
        {
            this.Invoke(new Action(() =>
            {
                rtbx_Receive.AppendText(string.Concat(str, Environment.NewLine));
            }));
        }

        private void UpdateUi(string str)
        {
            this.Invoke(new Action(() =>
            {

                rtbx_Status.AppendText(string.Concat(str, Environment.NewLine));
            }));
        }

        private void btn_StopListen_Click(object sender, EventArgs e)
        {
            try
            {
                socketServer.Close();
                if (socketClient.Connected)
                {
                    socketClient.Close();
                }
            }
            catch (Exception ex)
            {
                UpdateUi("监听尚未开始，关闭无效");
            }
            btn_Listen.Enabled = true;
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            NetworkStream networkStream = new NetworkStream(socketClient);
            byte[] bytes = Encoding.ASCII.GetBytes(rtbx_Send.Text);
            try
            {
                networkStream.Write(bytes, 0, bytes.Length);
            }
            catch
            {
                UpdateUi("发送失败");
            }
        }

        

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                socketServer.Close();
                if (socketClient.Connected)
                {
                    socketClient.Close();
                }
            }
            catch (Exception ex)
            {
                UpdateUi("监听尚未开始，关闭无效");
            }
        }
    }
}
