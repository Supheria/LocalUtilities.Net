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

namespace 单个同步TCP服务器实现之Socket类
{
    public partial class Form1 : Form
    {
        Socket socketServer;
        Socket socketClient;
        delegate  void  UpdateDelegate(string str);
        CancellationTokenSource tokenSource ;
        CancellationToken token;

        public Form1()
        {
            InitializeComponent();
           
        }

        private void btn_Listen_Click(object sender, EventArgs e)
        {
            if(btn_Listen .Text =="开始监听" )
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


                tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
                Task task = new Task(AcceptMessage, token);
                task.Start();
                
            }
            else
            {
                btn_Listen.Text = "开始监听";
                tokenSource.Cancel();
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
        /// 接收客户端的数据
        /// </summary>
        private void AcceptMessage()
        {
            socketClient = socketServer.Accept();
            UpdateUi($"客户端已连接{socketClient.RemoteEndPoint.ToString()}");

            while (true)
            {
                try
                {
                    NetworkStream networkStream = new NetworkStream(socketClient);
                    byte[] buffer = new byte[1024];
                   int nums= networkStream.Read(buffer,0, buffer.Length );
                    if(nums ==0)
                    {
                        UpdateUi("与客户端断开连接");
                        AcceptMessage();
                    }
                    UpdateReceive(Encoding .ASCII .GetString (buffer,0,nums ));

                    if(token.IsCancellationRequested )
                    {
                        break;
                    }
                }
                catch(Exception ex)
                {
                    UpdateUi("与客户端断开连接");
                    AcceptMessage();
                }
            }
        }
        private void UpdateReceive(string str)
        {
            this.Invoke(new Action (()=> {

                rtbx_Receive.AppendText(string.Concat(str, Environment.NewLine));
            }) );
        }
       
        private void UpdateUi(string str)
        {
            this.Invoke(new Action (()=> {

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
                    tokenSource.Cancel();
                }
            }
            catch(Exception ex)
            {
                UpdateUi("监听尚未开始，关闭无效");
            }
            btn_Listen.Enabled = true ;

        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            NetworkStream networkStream = new NetworkStream(socketClient );
            byte[] bytes = Encoding.ASCII.GetBytes(rtbx_Send .Text );
            try
            {
                networkStream.Write(bytes, 0, bytes.Length);
                networkStream.Flush();
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
                    tokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                UpdateUi("监听尚未开始，关闭无效");
            }
        }
    }
}
