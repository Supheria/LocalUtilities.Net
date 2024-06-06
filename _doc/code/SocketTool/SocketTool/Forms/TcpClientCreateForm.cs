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
    public partial class TcpClientCreateForm : Form
    {
        public string Ip;
        public int Port;
        public TcpClientCreateForm()
        {
            InitializeComponent();
            InitialUi();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = tbx_Ip.Text.Trim();
                int port = Convert.ToInt32(numericUpDown_Port.Value);
                IPAddress iPAddress = IPAddress.Parse(ip);
                Ip = ip;
                Port =port;
                this.DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message );
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel ;
        }

        private void InitialUi()
        {
            numericUpDown_Port.Maximum = 65535;
        }

        private void numericUpDown_Port_ValueChanged(object sender, EventArgs e)
        {

        }

        private void tbx_Ip_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
