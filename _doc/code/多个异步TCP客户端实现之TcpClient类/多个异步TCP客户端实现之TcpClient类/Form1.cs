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
using 同步tcp客户端;

namespace 多个异步TCP客户端实现之TcpClient类
{
    public partial class Form1 : Form
    {
        TcpClientAsyncTool tcpClient;
        TcpClientAsyncTool tcpClient2;
        bool isConnected = false;
        bool isConnected2 = false;
        byte[] receiveData = new byte[1024];
        Action action;
        public Form1()
        {
            InitializeComponent();
            action = ConnectTest;
            InitialUi();
        }


        private void ConnectTest()
        {
            Console.WriteLine("异步连接时测试使用");
        }

        private void button_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                string errorMsg = string.Empty;

                if (button_Connect.Text == "连接")
                {
                    string ip = tbx_ip.Text.Trim();
                    int port = Convert.ToInt32(numericUpDown_Port.Value);
                    tcpClient = new TcpClientAsyncTool(ip, port);

                   if( tcpClient.Connect(action,out errorMsg))
                    {
                        MessageBox.Show("连接成功");
                        isConnected = true;
                        button_Connect.Text = "断开";
                        Task.Run(new Action(ReceiveData));
                    }
                    else
                    {
                        MessageBox.Show($"连接失败:{errorMsg}");
                    }
                    
                }
                else
                {
                    button_Connect.Text = "连接";
                    isConnected = false;
                    tcpClient.DisConnect(out errorMsg);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败：{ex.Message }");
            }

        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitialUi()
        {
            numericUpDown_Port.Maximum = int.MaxValue;
            numericUpDown_Port2.Maximum = int.MaxValue;
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                string errorMsg = string.Empty;
                string sendStr = tbx_Send.Text.Trim();
                byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
                tcpClient.SendData(sendBytes, out errorMsg);

            }
            else
            {
                MessageBox.Show("服务器未连接");
            }
        }

        /// <summary>
        /// 客户端1接收服务器的数据线程
        /// </summary>
        private void ReceiveData()
        {

            while (true)
            {
                try
                {
                    bool result = false;
                    string errorMsg = string.Empty;
                    byte[] readReadBytes = tcpClient.ReceiveData(out result, out errorMsg);
                    UpdateReceiveData(readReadBytes);
                }
                catch (Exception ex)
                {

                }
                if (isConnected == false)
                {
                    break;
                }
            }

        }

        /// <summary>
        /// 客户端2接收服务器的数据线程
        /// </summary>
        private void ReceiveData2()
        {

            while (true)
            {
                try
                {
                    bool result = false;
                    string errorMsg = string.Empty;
                    byte[] readReadBytes = tcpClient2.ReceiveData(out result, out errorMsg);
                    UpdateReceiveData2(readReadBytes);
                }
                catch (Exception ex)
                {

                }
                if (isConnected == false)
                {
                    break;
                }
            }

        }

        /// <summary>
        /// 更新接收到的数据到界面
        /// </summary>
        /// <param name="bytes"></param>
        private void UpdateReceiveData(byte[] bytes)
        {
            string str = Encoding.ASCII.GetString(bytes);
            this.Invoke(new Action(() =>
            {
                rtbx_Receive.AppendText(str);
            }));

        }



        /// <summary>
        /// 更新接收到的数据到界面
        /// </summary>
        /// <param name="bytes"></param>
        private void UpdateReceiveData2(byte[] bytes)
        {
            string str = Encoding.ASCII.GetString(bytes);
            this.Invoke(new Action(() =>
            {
                rtbx_Receive2.AppendText(str);
            }));

        }

        private void button_Connect2_Click(object sender, EventArgs e)
        {
            try
            {
                string errorMsg = string.Empty;
                if (button_Connect2.Text == "连接")
                {
                    string ip = tbx_ip2.Text.Trim();
                    int port = Convert.ToInt32(numericUpDown_Port2.Value);
                    tcpClient2 = new TcpClientAsyncTool(ip, port);

                    if (tcpClient2.Connect(action,out errorMsg))
                    {
                        MessageBox.Show("连接成功");
                        isConnected2 = true;
                        button_Connect2.Text = "断开";
                        Task.Run(new Action(ReceiveData2));
                    }
                    else
                    {
                        MessageBox.Show($"连接失败：{errorMsg }");
                    }
                   
                }
                else
                {
                    button_Connect2.Text = "连接";
                    isConnected2 = false;
                    tcpClient2.DisConnect (out errorMsg);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败：{ex.Message }");
            }
        }

        private void btn_Send2_Click(object sender, EventArgs e)
        {
            if (isConnected2)
            {
                string errorMsg = string.Empty;
                string sendStr = tbx_Send2.Text.Trim();
                byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
                tcpClient2.SendData(sendBytes, out errorMsg);
            }
            else
            {
                MessageBox.Show("服务器未连接");
            }
        }
    }
}
