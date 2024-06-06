using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketTool
{
    public partial class UserControl_UdpClient : UserControl
    {
        UdpClient_Sync udpClient_Sync;
        public string Ip;
        public int RemotePort;
        public int LocalPort;

        public UserControl_UdpClient(string remoteIp, int remotePort, int localPort)
        {
            InitializeComponent();
            this.Ip = remoteIp;
            this.RemotePort = remotePort;
            this.LocalPort = localPort;
            udpClient_Sync = new UdpClient_Sync(localPort, remoteIp, remotePort);
            this.lbl_LocalAddress.Text = localPort.ToString ();
            this.lbl_RemoteAddress.Text = remoteIp+":"+ remotePort;
            Task.Run( new Action ( ReceiveData));
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            string errorMsg = string.Empty;
            string sendStr = tbx_Send.Text.Trim();
            byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
            udpClient_Sync.SendData(sendBytes, out errorMsg);
            UpdateLog(sendStr, DataDir.Send );
        }

        private void ReceiveData()
        {

            while (true )
            {
                bool result = false;
                string errorMsg = string.Empty;
                byte[] bytes = udpClient_Sync.ReceiveData(out result, out errorMsg);
                if(result )
                {
                    string str = Encoding.ASCII.GetString(bytes);
                    UpdateLog(str, DataDir.Receive );
                }
            }
          
        }
        private void UpdateLog(string str, DataDir dataDir)
        {
            string endStr = string.Empty;
            if (dataDir == DataDir.Receive)
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
}
