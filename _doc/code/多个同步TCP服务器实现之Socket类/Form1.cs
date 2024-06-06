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

namespace 多个同步TCP服务器实现之Socket类
{
    public partial class Form1 : Form
    {
        SocketServerSync socketServerSync;
        delegate  void  UpdateDelegate(string str);
        CancellationTokenSource tokenSource ;
        CancellationToken token;

        Action<string> UpdateUiDelegate;
        Action<string> AddClientlistDelegate;
        Action<string> RemoveClientlistDelegate;
        public Form1()
        {
            InitializeComponent();
            UpdateUiDelegate = UpdateUi;
            AddClientlistDelegate = AddClientList;
            RemoveClientlistDelegate = RemoveClientList;
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

                socketServerSync = new SocketServerSync(iPAddress,port , UpdateUiDelegate, AddClientlistDelegate, RemoveClientlistDelegate);
                btn_Listen.Text = "停止监听";
            }
            else
            {
                btn_Listen.Text = "开始监听";
                socketServerSync.StopMonitor();
            }
        }

       
      
       
        private void UpdateUi(string str)
        {
            this.Invoke(new Action (()=> {

                rtbx_Receive .AppendText(string.Concat(str, Environment.NewLine));
            }));
           
        }


        /// <summary>
        /// 添加到客户端列表
        /// </summary>
        /// <param name="remoteInfo"></param>
        private void AddClientList(string remoteInfo)
        {
            this.Invoke(new Action(() => {

                cbx_ClientList.Items.Add(remoteInfo);
                cbx_ClientList.SelectedItem = remoteInfo;
            }));
        }

        /// <summary>
        /// 移除客户端列表
        /// </summary>
        /// <param name="remoteInfo"></param>
        private void RemoveClientList(string remoteInfo)
        {
            this.Invoke(new Action(() => {

                cbx_ClientList.Items.Remove(remoteInfo);
            }));
        }

    

        private void btn_Send_Click(object sender, EventArgs e)
        {
            string clientStr = cbx_ClientList.SelectedItem.ToString();
            string sendStr = rtbx_Send.Text;
            socketServerSync.SendData(clientStr, sendStr);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }
    }
}
