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
    public partial class UdpServerCreateForm : Form
    {
        public int Port;
        public UdpServerCreateForm()
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
