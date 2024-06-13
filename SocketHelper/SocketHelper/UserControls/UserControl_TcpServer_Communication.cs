using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace SocketHelper
{
    public partial class UserControl_TcpServer_Communication : UserControl
    {
        public int Port;
        public string RemoteInfo;
        private Socket socketClient;
        public UserControl_TcpServer_Communication(string remoteInfo,int port,Socket  socket)
        {
            InitializeComponent();
            this.Port = port;
            this.RemoteInfo = remoteInfo;
            this.socketClient = socket;
            btn_Connect_Click(null,null);

            this.lbl_RemoteAddress.Text  = remoteInfo;
            this.lbl_LocalAddress .Text = port.ToString();
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            if(btn_Connect .Text =="连接")
            {
                btn_Connect.Text = "断开";
            }
            else
            {
                GlobalDelegate.deleteTcpServerCommunicationDelegate(RemoteInfo);
            }
        }


        public  void updateReceiveStr(string str)
        {
            this.Invoke(new Action(() => {

                richTextBox_Receive.AppendText(str);
            }));
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            NetworkStream networkStreamWrite = new NetworkStream(socketClient);
            string strSend = tbx_Send .Text .Trim ();
            byte[] sendBytes = Encoding.ASCII.GetBytes(strSend);
            networkStreamWrite.Write(sendBytes, 0, sendBytes.Length);
        }
    }
}
