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

namespace 单个异步TCP客户端实现之TcpClient类
{
    public partial class Form1 : Form
    {
        TcpClient tcpClient;
        bool isConnected = false;
        byte[] receiveData = new byte[1024];
        public Form1()
        {
            InitializeComponent();
            InitialUi();
        }

        private void button_Connect_Click(object sender, EventArgs e)
        {
            try
            {

                if (button_Connect.Text == "连接")
                {
                    tcpClient = new TcpClient();
                    string ip = tbx_ip.Text.Trim();
                    int port = Convert.ToInt32(numericUpDown_Port.Value);
                    IPAddress iPAddress = IPAddress.Parse(ip);

                    IAsyncResult asyncResult = tcpClient.BeginConnect(ip, port, null, null);//异步连接
                    //可以干一些别的事情
                    bool success = asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));//设置连接超时时间为2秒
                    tcpClient.EndConnect(asyncResult);
                    MessageBox.Show("连接成功");
                    isConnected = true;
                    button_Connect.Text = "断开";
                    Task.Run(new Action(ReceiveData));
                }
                else
                {
                    button_Connect.Text = "连接";
                    isConnected = false;
                    tcpClient.Close();
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
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                NetworkStream networkStream = tcpClient.GetStream();
                string sendStr = tbx_Send.Text.Trim();
                byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
                IAsyncResult asyncResult = networkStream.BeginWrite(sendBytes, 0, sendBytes.Length, null, null);//异步写入
                //可以干一些别的事情
                networkStream.EndWrite(asyncResult);
            }
            else
            {
                MessageBox.Show("服务器未连接");
            }
        }

        /// <summary>
        /// 接收服务器的数据线程
        /// </summary>
        private void ReceiveData()
        {

            while (true)
            {
                try
                {
                    NetworkStream networkStream = tcpClient.GetStream();
                    IAsyncResult iAsyncResult = networkStream.BeginRead(receiveData, 0, receiveData.Length, null, null);//异步读取
                    //干一些别的事情
                    int readByteNums = networkStream.EndRead(iAsyncResult);
                    byte[] readReadBytes = new byte[readByteNums];
                    Array.Copy(receiveData, 0, readReadBytes, 0, readByteNums);
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



    }
}
