
namespace 多个异步TCP客户端实现之TcpClient类
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rtbx_Receive = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbx_Send = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Send = new System.Windows.Forms.Button();
            this.numericUpDown_Port = new System.Windows.Forms.NumericUpDown();
            this.tbx_ip = new System.Windows.Forms.TextBox();
            this.button_Connect = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rtbx_Receive2 = new System.Windows.Forms.RichTextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbx_Send2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_Send2 = new System.Windows.Forms.Button();
            this.numericUpDown_Port2 = new System.Windows.Forms.NumericUpDown();
            this.tbx_ip2 = new System.Windows.Forms.TextBox();
            this.button_Connect2 = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Port)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Port2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rtbx_Receive);
            this.groupBox2.Location = new System.Drawing.Point(16, 119);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(414, 210);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "数据接收区：";
            // 
            // rtbx_Receive
            // 
            this.rtbx_Receive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbx_Receive.Location = new System.Drawing.Point(3, 17);
            this.rtbx_Receive.Name = "rtbx_Receive";
            this.rtbx_Receive.Size = new System.Drawing.Size(408, 190);
            this.rtbx_Receive.TabIndex = 0;
            this.rtbx_Receive.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbx_Send);
            this.groupBox1.Location = new System.Drawing.Point(20, 335);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(322, 210);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "数据发送区：";
            // 
            // tbx_Send
            // 
            this.tbx_Send.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbx_Send.Location = new System.Drawing.Point(3, 17);
            this.tbx_Send.Multiline = true;
            this.tbx_Send.Name = "tbx_Send";
            this.tbx_Send.Size = new System.Drawing.Size(316, 190);
            this.tbx_Send.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(201, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 15;
            this.label2.Text = "端口号：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(178, -27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "ip地址：";
            // 
            // btn_Send
            // 
            this.btn_Send.Location = new System.Drawing.Point(352, 425);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(78, 49);
            this.btn_Send.TabIndex = 13;
            this.btn_Send.Text = "发送";
            this.btn_Send.UseVisualStyleBackColor = true;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // numericUpDown_Port
            // 
            this.numericUpDown_Port.Location = new System.Drawing.Point(276, 74);
            this.numericUpDown_Port.Name = "numericUpDown_Port";
            this.numericUpDown_Port.Size = new System.Drawing.Size(117, 21);
            this.numericUpDown_Port.TabIndex = 12;
            // 
            // tbx_ip
            // 
            this.tbx_ip.Location = new System.Drawing.Point(276, 35);
            this.tbx_ip.Name = "tbx_ip";
            this.tbx_ip.Size = new System.Drawing.Size(117, 21);
            this.tbx_ip.TabIndex = 11;
            this.tbx_ip.Text = "127.0.0.1";
            // 
            // button_Connect
            // 
            this.button_Connect.Location = new System.Drawing.Point(23, 46);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(123, 49);
            this.button_Connect.TabIndex = 10;
            this.button_Connect.Text = "连接";
            this.button_Connect.UseVisualStyleBackColor = true;
            this.button_Connect.Click += new System.EventHandler(this.button_Connect_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(225, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 18;
            this.label3.Text = "IP：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(756, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 26;
            this.label4.Text = "IP：";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rtbx_Receive2);
            this.groupBox3.Location = new System.Drawing.Point(547, 119);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(414, 210);
            this.groupBox3.TabIndex = 25;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "数据接收区：";
            // 
            // rtbx_Receive2
            // 
            this.rtbx_Receive2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbx_Receive2.Location = new System.Drawing.Point(3, 17);
            this.rtbx_Receive2.Name = "rtbx_Receive2";
            this.rtbx_Receive2.Size = new System.Drawing.Size(408, 190);
            this.rtbx_Receive2.TabIndex = 0;
            this.rtbx_Receive2.Text = "";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbx_Send2);
            this.groupBox4.Location = new System.Drawing.Point(551, 335);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(322, 210);
            this.groupBox4.TabIndex = 24;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "数据发送区：";
            // 
            // tbx_Send2
            // 
            this.tbx_Send2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbx_Send2.Location = new System.Drawing.Point(3, 17);
            this.tbx_Send2.Multiline = true;
            this.tbx_Send2.Name = "tbx_Send2";
            this.tbx_Send2.Size = new System.Drawing.Size(316, 190);
            this.tbx_Send2.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(732, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 23;
            this.label5.Text = "端口号：";
            // 
            // btn_Send2
            // 
            this.btn_Send2.Location = new System.Drawing.Point(883, 425);
            this.btn_Send2.Name = "btn_Send2";
            this.btn_Send2.Size = new System.Drawing.Size(78, 49);
            this.btn_Send2.TabIndex = 22;
            this.btn_Send2.Text = "发送";
            this.btn_Send2.UseVisualStyleBackColor = true;
            this.btn_Send2.Click += new System.EventHandler(this.btn_Send2_Click);
            // 
            // numericUpDown_Port2
            // 
            this.numericUpDown_Port2.Location = new System.Drawing.Point(807, 74);
            this.numericUpDown_Port2.Name = "numericUpDown_Port2";
            this.numericUpDown_Port2.Size = new System.Drawing.Size(117, 21);
            this.numericUpDown_Port2.TabIndex = 21;
            // 
            // tbx_ip2
            // 
            this.tbx_ip2.Location = new System.Drawing.Point(807, 35);
            this.tbx_ip2.Name = "tbx_ip2";
            this.tbx_ip2.Size = new System.Drawing.Size(117, 21);
            this.tbx_ip2.TabIndex = 20;
            this.tbx_ip2.Text = "127.0.0.1";
            // 
            // button_Connect2
            // 
            this.button_Connect2.Location = new System.Drawing.Point(554, 46);
            this.button_Connect2.Name = "button_Connect2";
            this.button_Connect2.Size = new System.Drawing.Size(123, 49);
            this.button_Connect2.TabIndex = 19;
            this.button_Connect2.Text = "连接";
            this.button_Connect2.UseVisualStyleBackColor = true;
            this.button_Connect2.Click += new System.EventHandler(this.button_Connect2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(996, 566);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btn_Send2);
            this.Controls.Add(this.numericUpDown_Port2);
            this.Controls.Add(this.tbx_ip2);
            this.Controls.Add(this.button_Connect2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Send);
            this.Controls.Add(this.numericUpDown_Port);
            this.Controls.Add(this.tbx_ip);
            this.Controls.Add(this.button_Connect);
            this.Name = "Form1";
            this.Text = "多个异步Tcp客户端软件";
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Port)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Port2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox rtbx_Receive;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbx_Send;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.NumericUpDown numericUpDown_Port;
        private System.Windows.Forms.TextBox tbx_ip;
        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox rtbx_Receive2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tbx_Send2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_Send2;
        private System.Windows.Forms.NumericUpDown numericUpDown_Port2;
        private System.Windows.Forms.TextBox tbx_ip2;
        private System.Windows.Forms.Button button_Connect2;
    }
}

