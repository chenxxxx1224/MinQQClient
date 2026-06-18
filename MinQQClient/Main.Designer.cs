namespace MinQQClient
{
    partial class Main
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lstOnlineUsers = new System.Windows.Forms.ListBox();
            this.rtbChatHistory = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            //
            // panel1
            //
            this.panel1.Controls.Add(this.btnApply);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1379, 123);
            this.panel1.TabIndex = 0;
            //
            // btnApply
            //
            this.btnApply.Location = new System.Drawing.Point(1180, 35);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(90, 50);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "好友申请";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            //
            // btnAdd
            //
            this.btnAdd.Location = new System.Drawing.Point(1070, 35);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 50);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "添加好友";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // lblTitle
            //
            this.lblTitle.Location = new System.Drawing.Point(20, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(400, 123);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "label1";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSend);
            this.panel2.Controls.Add(this.txtMessage);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 844);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1379, 182);
            this.panel2.TabIndex = 2;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(1118, 44);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(178, 116);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click_1);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(322, 44);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(709, 116);
            this.txtMessage.TabIndex = 0;
            // 
            // lstOnlineUsers
            // 
            this.lstOnlineUsers.Dock = System.Windows.Forms.DockStyle.Left;
            this.lstOnlineUsers.FormattingEnabled = true;
            this.lstOnlineUsers.ItemHeight = 18;
            this.lstOnlineUsers.Location = new System.Drawing.Point(0, 123);
            this.lstOnlineUsers.Name = "lstOnlineUsers";
            this.lstOnlineUsers.Size = new System.Drawing.Size(207, 721);
            this.lstOnlineUsers.TabIndex = 3;
            this.lstOnlineUsers.SelectedIndexChanged += new System.EventHandler(this.lstOnlineUsers_SelectedIndexChanged);
            // 
            // rtbChatHistory
            // 
            this.rtbChatHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbChatHistory.Location = new System.Drawing.Point(207, 123);
            this.rtbChatHistory.Name = "rtbChatHistory";
            this.rtbChatHistory.ReadOnly = true;
            this.rtbChatHistory.Size = new System.Drawing.Size(1172, 721);
            this.rtbChatHistory.TabIndex = 4;
            this.rtbChatHistory.Text = "";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1379, 1026);
            this.Controls.Add(this.rtbChatHistory);
            this.Controls.Add(this.lstOnlineUsers);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Main";
            this.Text = "qq主界面";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListBox lstOnlineUsers;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.RichTextBox rtbChatHistory;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnApply;
    }
}