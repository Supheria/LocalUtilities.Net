using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketHelper
{
    public partial class UserControl_TcpClient : UserControl
    {
        TcpClient_Sync tcpClient_Sync;

         string Ip;
        int Port;
        public UserControl_TcpClient(string ip,int port)
        {
            InitializeComponent();
         
            this.Ip = ip;
            this.Port = port;
            this.lbl_LocalAddress.Text  = "0.0.0.1:0";
            this.lbl_RemoteAddress.Text = string.Join(":",ip,port );
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            string errorMsg = string.Empty;
            try
            {
                if (btn_Connect.Text == "连接")
                {
                   
                    tcpClient_Sync = new TcpClient_Sync(Ip, Port);
                    if (tcpClient_Sync.Connect(out errorMsg))
                    {
                        btn_Connect.Text = "断开";
                        //MessageBox.Show("连接成功");
                        lbl_LocalAddress.Text = tcpClient_Sync.socketClient.LocalEndPoint.ToString ().Split (':').Last ();

                        Thread thread = new Thread(new ParameterizedThreadStart(ReceiveData));
                        thread.IsBackground = true;
                        thread.Start();
                    }
                    else
                    {
                        MessageBox.Show("连接失败");
                    }
                }
                else
                {
                    btn_Connect.Text = "连接";
                    tcpClient_Sync.DisConnect(out errorMsg);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败:{ex.Message }");
            }
        }

       

        /// <summary>
        /// 接收数据
        /// </summary>
        private void ReceiveData(object socket)
        {
            while (true)
            {
                try
                {
                    string errorMsg = string.Empty;
                    bool result = false;
                    byte[] readBytes = tcpClient_Sync.ReceiveData(out result, out errorMsg);
                    if(result )
                    {
                        string str = Encoding.ASCII.GetString(readBytes);
                        UpdateLog(str, DataDir.Receive );
                    }
                    else
                    {

                    }
                   
                    if (result == false)
                    {
                        break;//接收数据失败
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox_Receive.AppendText(ex .Message );
                    }));
                    break;
                }
            }

        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            try
            {
                string errorMsg = string.Empty;
                string str = tbx_Send.Text.Trim();
                byte[] sendBytes = Encoding.ASCII.GetBytes(str);
                tcpClient_Sync.SendData(sendBytes, out errorMsg);
                UpdateLog(str,DataDir.Send );
            }
            catch (Exception ex)
            {

            }
        }

        private void UpdateLog(string str, DataDir dataDir)
        {
            string endStr = string.Empty;
            if (dataDir==DataDir.Receive )
            {
                endStr = DateTime.Now.ToString("HH:mm:ss") + " 接收数据：" + str + "\n";
            }
            else
            {
                 endStr = DateTime.Now.ToString("HH:mm:ss") + " 发送数据：" + str + "\n";
            }
           
            this.Invoke(new Action(() =>
            {
                richTextBox_Receive.AppendText(endStr);
            }));
        }
    }


    public enum DataDir
    {
        Send,
        Receive
    }
}
