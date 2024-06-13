namespace 多个异步TCP服务器实现之Socket类
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
            this.cbx_ClientList = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_Send = new System.Windows.Forms.Button();
            this.rtbx_Send = new System.Windows.Forms.RichTextBox();
            this.rtbx_Receive = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbx_Port = new System.Windows.Forms.TextBox();
            this.tbx_IP = new System.Windows.Forms.TextBox();
            this.btn_Listen = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbx_ClientList
            // 
            this.cbx_ClientList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_ClientList.FormattingEnabled = true;
            this.cbx_ClientList.Location = new System.Drawing.Point(123, 474);
            this.cbx_ClientList.Name = "cbx_ClientList";
            this.cbx_ClientList.Size = new System.Drawing.Size(225, 20);
            this.cbx_ClientList.TabIndex = 26;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(31, 477);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 25;
            this.label6.Text = "客户端列表：";
            // 
            // btn_Send
            // 
            this.btn_Send.Location = new System.Drawing.Point(415, 468);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(75, 31);
            this.btn_Send.TabIndex = 24;
            this.btn_Send.Text = "发送信息";
            this.btn_Send.UseVisualStyleBackColor = true;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // rtbx_Send
            // 
            this.rtbx_Send.Location = new System.Drawing.Point(123, 302);
            this.rtbx_Send.Name = "rtbx_Send";
            this.rtbx_Send.Size = new System.Drawing.Size(684, 132);
            this.rtbx_Send.TabIndex = 23;
            this.rtbx_Send.Text = "";
            // 
            // rtbx_Receive
            // 
            this.rtbx_Receive.Location = new System.Drawing.Point(123, 113);
            this.rtbx_Receive.Name = "rtbx_Receive";
            this.rtbx_Receive.Size = new System.Drawing.Size(684, 158);
            this.rtbx_Receive.TabIndex = 22;
            this.rtbx_Receive.Text = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 359);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 21;
            this.label5.Text = "发送信息：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 178);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 20;
            this.label4.Text = "接收信息：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 19;
            this.label2.Text = "监听端口";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "服务器IP";
            // 
            // tbx_Port
            // 
            this.tbx_Port.Location = new System.Drawing.Point(103, 68);
            this.tbx_Port.Name = "tbx_Port";
            this.tbx_Port.Size = new System.Drawing.Size(101, 21);
            this.tbx_Port.TabIndex = 17;
            this.tbx_Port.Text = "100";
            // 
            // tbx_IP
            // 
            this.tbx_IP.Location = new System.Drawing.Point(103, 23);
            this.tbx_IP.Name = "tbx_IP";
            this.tbx_IP.Size = new System.Drawing.Size(101, 21);
            this.tbx_IP.TabIndex = 16;
            this.tbx_IP.Text = "127.0.0.1";
            // 
            // btn_Listen
            // 
            this.btn_Listen.Location = new System.Drawing.Point(281, 49);
            this.btn_Listen.Name = "btn_Listen";
            this.btn_Listen.Size = new System.Drawing.Size(101, 31);
            this.btn_Listen.TabIndex = 15;
            this.btn_Listen.Text = "开始监听";
            this.btn_Listen.UseVisualStyleBackColor = true;
            this.btn_Listen.Click += new System.EventHandler(this.btn_Listen_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 535);
            this.Controls.Add(this.cbx_ClientList);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btn_Send);
            this.Controls.Add(this.rtbx_Send);
            this.Controls.Add(this.rtbx_Receive);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbx_Port);
            this.Controls.Add(this.tbx_IP);
            this.Controls.Add(this.btn_Listen);
            this.Name = "Form1";
            this.Text = "单个异步TCP服务器监听多个客户端实现之Socket类";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbx_ClientList;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.RichTextBox rtbx_Send;
        private System.Windows.Forms.RichTextBox rtbx_Receive;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbx_Port;
        private System.Windows.Forms.TextBox tbx_IP;
        private System.Windows.Forms.Button btn_Listen;
    }
}

