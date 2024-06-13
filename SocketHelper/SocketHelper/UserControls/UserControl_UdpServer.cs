using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketHelper
{
    public partial class UserControl_UdpServer : UserControl
    {
        UdpServer_Sync udpServer_Sync;
      public   int LocalPort;
        Action<string> UpdateRecevieStrDelgate;
        public UserControl_UdpServer(int localPort)
        {
            InitializeComponent();
            this.LocalPort = localPort;
            UpdateRecevieStrDelgate = UpdateRecevieStr;
            udpServer_Sync = new UdpServer_Sync(LocalPort, UpdateRecevieStrDelgate);
            lbl_LocalAddress.Text = localPort.ToString ();
        }


        private void UpdateRecevieStr(string str)
        {
            this.Invoke(new Action (()=> {
             lbl_RemoteAddress .Text =   udpServer_Sync.remoteIpEndPoint.ToString();
             
                richTextBox_Receive.AppendText(str);

            }));
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            string errorMsg = string.Empty;
            string sendStr = tbx_Send.Text.Trim();
            byte[] sendBytes= Encoding.ASCII.GetBytes(sendStr);
            udpServer_Sync.SendData(sendBytes,out errorMsg );
        }
    }
}
