using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketTool
{
    public partial class UserControl_UdpGroup : UserControl
    {
        UdpGroup_Sync udpGroup_Sync;
        public int LocalPort;
        public string GroupIp;
        Action<string> UpdateRecevieStrDelgate;
        public UserControl_UdpGroup(string ip, int localPort)
        {
            InitializeComponent();
            this.LocalPort = localPort;
            this.GroupIp = ip;
            UpdateRecevieStrDelgate = UpdateRecevieStr;
            udpGroup_Sync = new UdpGroup_Sync(GroupIp, LocalPort, UpdateRecevieStrDelgate);
            this.lbl_GroupAddress.Text  = ip;
            this.lbl_GroupPort.Text = LocalPort.ToString ();
        }


        private void UpdateRecevieStr(string str)
        {
            this.Invoke(new Action(() =>
            {
                richTextBox_Receive.AppendText(str);

            }));
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            string errorMsg = string.Empty;
            string sendStr = tbx_Send.Text.Trim();
            udpGroup_Sync.SendMessage(sendStr);
        }
    }
}
