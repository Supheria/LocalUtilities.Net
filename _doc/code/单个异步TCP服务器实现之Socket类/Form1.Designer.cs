namespace 单个异步TCP服务器实现之Socket类
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
            this.btn_Listen = new System.Windows.Forms.Button();
            this.tbx_IP = new System.Windows.Forms.TextBox();
            this.tbx_Port = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rtbx_Status = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.rtbx_Receive = new System.Windows.Forms.RichTextBox();
            this.rtbx_Send = new System.Windows.Forms.RichTextBox();
            this.btn_Send = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Listen
            // 
            this.btn_Listen.Location = new System.Drawing.Point(84, 121);
            this.btn_Listen.Name = "btn_Listen";
            this.btn_Listen.Size = new System.Drawing.Size(101, 31);
            this.btn_Listen.TabIndex = 0;
            this.btn_Listen.Text = "开始监听";
            this.btn_Listen.UseVisualStyleBackColor = true;
            this.btn_Listen.Click += new System.EventHandler(this.btn_Listen_Click);
            // 
            // tbx_IP
            // 
            this.tbx_IP.Location = new System.Drawing.Point(84, 38);
            this.tbx_IP.Name = "tbx_IP";
            this.tbx_IP.Size = new System.Drawing.Size(101, 21);
            this.tbx_IP.TabIndex = 2;
            this.tbx_IP.Text = "127.0.0.1";
            // 
            // tbx_Port
            // 
            this.tbx_Port.Location = new System.Drawing.Point(84, 83);
            this.tbx_Port.Name = "tbx_Port";
            this.tbx_Port.Size = new System.Drawing.Size(101, 21);
            this.tbx_Port.TabIndex = 3;
            this.tbx_Port.Text = "100";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "服务器IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "监听端口";
            // 
            // rtbx_Status
            // 
            this.rtbx_Status.Location = new System.Drawing.Point(261, 23);
            this.rtbx_Status.Name = "rtbx_Status";
            this.rtbx_Status.Size = new System.Drawing.Size(527, 111);
            this.rtbx_Status.TabIndex = 6;
            this.rtbx_Status.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(500, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "服务器状态";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 210);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "接收信息：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 363);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "发送信息：";
            // 
            // rtbx_Receive
            // 
            this.rtbx_Receive.Location = new System.Drawing.Point(104, 167);
            this.rtbx_Receive.Name = "rtbx_Receive";
            this.rtbx_Receive.Size = new System.Drawing.Size(684, 111);
            this.rtbx_Receive.TabIndex = 10;
            this.rtbx_Receive.Text = "";
            // 
            // rtbx_Send
            // 
            this.rtbx_Send.Location = new System.Drawing.Point(104, 314);
            this.rtbx_Send.Name = "rtbx_Send";
            this.rtbx_Send.Size = new System.Drawing.Size(684, 111);
            this.rtbx_Send.TabIndex = 11;
            this.rtbx_Send.Text = "";
            // 
            // btn_Send
            // 
            this.btn_Send.Location = new System.Drawing.Point(334, 459);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(75, 31);
            this.btn_Send.TabIndex = 12;
            this.btn_Send.Text = "发送信息";
            this.btn_Send.UseVisualStyleBackColor = true;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 502);
            this.Controls.Add(this.btn_Send);
            this.Controls.Add(this.rtbx_Send);
            this.Controls.Add(this.rtbx_Receive);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rtbx_Status);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbx_Port);
            this.Controls.Add(this.tbx_IP);
            this.Controls.Add(this.btn_Listen);
            this.Name = "Form1";
            this.Text = "单个异步TCP服务器实现之Socket类";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Listen;
        private System.Windows.Forms.TextBox tbx_IP;
        private System.Windows.Forms.TextBox tbx_Port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox rtbx_Status;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox rtbx_Receive;
        private System.Windows.Forms.RichTextBox rtbx_Send;
        private System.Windows.Forms.Button btn_Send;
    }
}

