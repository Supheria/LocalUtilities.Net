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
    public partial class UserControl_TcpServer : UserControl
    {
        public  int Port;
        TcpServer_Sync tcpServer_Sync;
        public UserControl_TcpServer(int port)
        {
            InitializeComponent();
            lbl_LocalPort.Text = port.ToString();
            this.Port = port;
            tcpServer_Sync= new TcpServer_Sync(Port);
            btn_Connect_Click(null,null );
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            if(btn_Connect .Text =="连接")
            {
                tcpServer_Sync.StartMonitor();
                btn_Connect.Text = "断开";
            }
            else
            {

            }
           
        }
    }
}
