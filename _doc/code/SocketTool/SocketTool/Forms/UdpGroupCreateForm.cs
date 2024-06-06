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
    public partial class UdpGroupCreateForm : Form
    {
        public string Ip;
        public int Port;
        public UdpGroupCreateForm()
        {
            InitializeComponent();
            InitialUi();
        }
        private void InitialUi()
        {
            numericUpDown_Port.Maximum = 65535;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = tbx_Ip.Text.Trim();
                IPAddress iPAddress = IPAddress.Parse(ip);
                Ip = ip;
                int port = Convert.ToInt32(numericUpDown_Port.Value);
                Port = port;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
