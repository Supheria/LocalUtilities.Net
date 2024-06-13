
namespace SocketHelper
{
    partial class UdpClientCreateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.numericUpDown_Port = new System.Windows.Forms.NumericUpDown();
            this.tbx_Ip = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown_LocalPort = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Port)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_LocalPort)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(235, 157);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(83, 36);
            this.btn_Cancel.TabIndex = 16;
            this.btn_Cancel.Text = "取消";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_Ok
            // 
            this.btn_Ok.Location = new System.Drawing.Point(132, 157);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(83, 36);
            this.btn_Ok.TabIndex = 15;
            this.btn_Ok.Text = "确定";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // numericUpDown_Port
            // 
            this.numericUpDown_Port.Location = new System.Drawing.Point(148, 70);
            this.numericUpDown_Port.Name = "numericUpDown_Port";
            this.numericUpDown_Port.Size = new System.Drawing.Size(216, 21);
            this.numericUpDown_Port.TabIndex = 14;
            // 
            // tbx_Ip
            // 
            this.tbx_Ip.Location = new System.Drawing.Point(145, 36);
            this.tbx_Ip.Name = "tbx_Ip";
            this.tbx_Ip.Size = new System.Drawing.Size(216, 21);
            this.tbx_Ip.TabIndex = 13;
            this.tbx_Ip.Text = "0.0.0.0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(77, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 12;
            this.label2.Text = "对方端口：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(77, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "对方IP:";
            // 
            // numericUpDown_LocalPort
            // 
            this.numericUpDown_LocalPort.Location = new System.Drawing.Point(148, 113);
            this.numericUpDown_LocalPort.Name = "numericUpDown_LocalPort";
            this.numericUpDown_LocalPort.Size = new System.Drawing.Size(216, 21);
            this.numericUpDown_LocalPort.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(77, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 17;
            this.label3.Text = "本地端口：";
            // 
            // UdpClientCreateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.OldLace;
            this.ClientSize = new System.Drawing.Size(452, 210);
            this.Controls.Add(this.numericUpDown_LocalPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.numericUpDown_Port);
            this.Controls.Add(this.tbx_Ip);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "UdpClientCreateForm";
            this.Text = "创建UDP客户端";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Port)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_LocalPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.NumericUpDown numericUpDown_Port;
        private System.Windows.Forms.TextBox tbx_Ip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown_LocalPort;
        private System.Windows.Forms.Label label3;
    }
}