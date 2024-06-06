using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketTool
{
    public partial class UdpClientCreateForm : Form
    {
        public string Ip;
        public int RemotePort;
        public int LocalPort;
        public UdpClientCreateForm()
        {
            InitializeComponent();
            InitialUi();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = tbx_Ip.Text.Trim();
                int remotePort = Convert.ToInt32(numericUpDown_Port.Value);
                int localPort = Convert.ToInt32(numericUpDown_LocalPort.Value);
                IPAddress iPAddress = IPAddress.Parse(ip);
                Ip = ip;
                RemotePort = remotePort;
                LocalPort = localPort;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitialUi()
        {
            numericUpDown_LocalPort.Maximum = 65535;
            numericUpDown_Port.Maximum = 65535;
        }
    }
}
