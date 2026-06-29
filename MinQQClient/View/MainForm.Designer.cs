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
            this.panel1 = new Sunny.UI.UIPanel();
            this.btnApply = new Sunny.UI.UIButton();
            this.btnAdd = new Sunny.UI.UIButton();
            this.lblTitle = new Sunny.UI.UILabel();
            this.panel2 = new Sunny.UI.UIPanel();
            this.btnSend = new Sunny.UI.UIButton();
            this.txtMessage = new Sunny.UI.UITextBox();
            this.lstOnlineUsers = new Sunny.UI.UIListBox();
            this.rtbChatHistory = new Sunny.UI.UIRichTextBox();
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
            this.panel1.Font = new System.Drawing.Font("宋体", 10F);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(900, 60);
            this.panel1.TabIndex = 0;
            this.panel1.Text = null;
            // 
            // btnApply
            // 
            this.btnApply.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApply.Font = new System.Drawing.Font("宋体", 10F);
            this.btnApply.Location = new System.Drawing.Point(780, 8);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(90, 40);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "好友申请";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.Font = new System.Drawing.Font("宋体", 10F);
            this.btnAdd.Location = new System.Drawing.Point(680, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 40);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "添加好友";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("宋体", 10F);
            this.lblTitle.Location = new System.Drawing.Point(10, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "label1";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSend);
            this.panel2.Controls.Add(this.txtMessage);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Font = new System.Drawing.Font("宋体", 10F);
            this.panel2.Location = new System.Drawing.Point(0, 530);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(900, 100);
            this.panel2.TabIndex = 2;
            this.panel2.Text = null;
            // 
            // btnSend
            // 
            this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend.Font = new System.Drawing.Font("宋体", 10F);
            this.btnSend.Location = new System.Drawing.Point(720, 10);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(150, 70);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "发送";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click_1);
            // 
            // txtMessage
            // 
            this.txtMessage.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMessage.Font = new System.Drawing.Font("宋体", 10F);
            this.txtMessage.Location = new System.Drawing.Point(10, 10);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ShowText = false;
            this.txtMessage.Size = new System.Drawing.Size(700, 70);
            this.txtMessage.TabIndex = 0;
            // 
            // lstOnlineUsers
            // 
            this.lstOnlineUsers.Dock = System.Windows.Forms.DockStyle.Left;
            this.lstOnlineUsers.Font = new System.Drawing.Font("宋体", 10F);
            this.lstOnlineUsers.FormattingEnabled = true;
            this.lstOnlineUsers.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.lstOnlineUsers.ItemHeight = 18;
            this.lstOnlineUsers.Location = new System.Drawing.Point(0, 60);
            this.lstOnlineUsers.MinimumSize = new System.Drawing.Size(1, 1);
            this.lstOnlineUsers.Name = "lstOnlineUsers";
            this.lstOnlineUsers.Padding = new System.Windows.Forms.Padding(2);
            this.lstOnlineUsers.ShowText = true;
            this.lstOnlineUsers.Size = new System.Drawing.Size(200, 470);
            this.lstOnlineUsers.TabIndex = 3;
            this.lstOnlineUsers.SelectedIndexChanged += new System.EventHandler(this.lstOnlineUsers_SelectedIndexChanged);
            // 
            // rtbChatHistory
            // 
            this.rtbChatHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbChatHistory.FillColor = System.Drawing.Color.White;
            this.rtbChatHistory.Font = new System.Drawing.Font("宋体", 10F);
            this.rtbChatHistory.Location = new System.Drawing.Point(150, 60);
            this.rtbChatHistory.MinimumSize = new System.Drawing.Size(1, 1);
            this.rtbChatHistory.Name = "rtbChatHistory";
            this.rtbChatHistory.Padding = new System.Windows.Forms.Padding(2);
            this.rtbChatHistory.ReadOnly = true;
            this.rtbChatHistory.ShowText = false;
            this.rtbChatHistory.Size = new System.Drawing.Size(750, 470);
            this.rtbChatHistory.TabIndex = 4;
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(900, 630);
            this.Controls.Add(this.rtbChatHistory);
            this.Controls.Add(this.lstOnlineUsers);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Main";
            this.Text = "qq主界面";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIPanel panel1;
        private Sunny.UI.UIPanel panel2;
        private Sunny.UI.UIListBox lstOnlineUsers;
        private Sunny.UI.UILabel lblTitle;
        private Sunny.UI.UIRichTextBox rtbChatHistory;
        private Sunny.UI.UIButton btnSend;
        private Sunny.UI.UITextBox txtMessage;
        private Sunny.UI.UIButton btnAdd;
        private Sunny.UI.UIButton btnApply;
    }
}
