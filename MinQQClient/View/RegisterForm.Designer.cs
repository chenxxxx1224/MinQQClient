namespace MinQQClient.View
{
    partial class RegisterForm
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
            this.lbl_title = new Sunny.UI.UILabel();
            this.lbl_username = new Sunny.UI.UILabel();
            this.lbl_password = new Sunny.UI.UILabel();
            this.lbl_confirm = new Sunny.UI.UILabel();
            this.txt_username = new Sunny.UI.UITextBox();
            this.txt_password = new Sunny.UI.UITextBox();
            this.txt_confirm = new Sunny.UI.UITextBox();
            this.btn_register = new Sunny.UI.UIButton();
            this.btn_cancel = new Sunny.UI.UIButton();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold);
            this.lbl_title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lbl_title.Location = new System.Drawing.Point(0, 35);
            this.lbl_title.Name = "lbl_title";
            this.lbl_title.Size = new System.Drawing.Size(400, 40);
            this.lbl_title.TabIndex = 0;
            this.lbl_title.Text = "用户注册";
            this.lbl_title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_username
            // 
            this.lbl_username.AutoSize = true;
            this.lbl_username.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_username.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lbl_username.Location = new System.Drawing.Point(46, 111);
            this.lbl_username.Name = "lbl_username";
            this.lbl_username.Size = new System.Drawing.Size(106, 24);
            this.lbl_username.TabIndex = 1;
            this.lbl_username.Text = "用户名：";
            // 
            // lbl_password
            // 
            this.lbl_password.AutoSize = true;
            this.lbl_password.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_password.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lbl_password.Location = new System.Drawing.Point(70, 181);
            this.lbl_password.Name = "lbl_password";
            this.lbl_password.Size = new System.Drawing.Size(82, 24);
            this.lbl_password.TabIndex = 1;
            this.lbl_password.Text = "密码：";
            // 
            // lbl_confirm
            // 
            this.lbl_confirm.AutoSize = true;
            this.lbl_confirm.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_confirm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lbl_confirm.Location = new System.Drawing.Point(23, 245);
            this.lbl_confirm.Name = "lbl_confirm";
            this.lbl_confirm.Size = new System.Drawing.Size(130, 24);
            this.lbl_confirm.TabIndex = 1;
            this.lbl_confirm.Text = "确认密码：";
            // 
            // txt_username
            // 
            this.txt_username.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_username.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_username.Location = new System.Drawing.Point(160, 105);
            this.txt_username.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_username.MinimumSize = new System.Drawing.Size(1, 16);
            this.txt_username.Name = "txt_username";
            this.txt_username.Padding = new System.Windows.Forms.Padding(5);
            this.txt_username.ShowText = false;
            this.txt_username.Size = new System.Drawing.Size(200, 30);
            this.txt_username.TabIndex = 2;
            this.txt_username.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_username.Watermark = "";
            // 
            // txt_password
            // 
            this.txt_password.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_password.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_password.Location = new System.Drawing.Point(160, 175);
            this.txt_password.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_password.MinimumSize = new System.Drawing.Size(1, 16);
            this.txt_password.Name = "txt_password";
            this.txt_password.Padding = new System.Windows.Forms.Padding(5);
            this.txt_password.ShowText = false;
            this.txt_password.Size = new System.Drawing.Size(200, 30);
            this.txt_password.TabIndex = 2;
            this.txt_password.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_password.Watermark = "";
            // 
            // txt_confirm
            // 
            this.txt_confirm.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_confirm.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_confirm.Location = new System.Drawing.Point(160, 245);
            this.txt_confirm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_confirm.MinimumSize = new System.Drawing.Size(1, 16);
            this.txt_confirm.Name = "txt_confirm";
            this.txt_confirm.Padding = new System.Windows.Forms.Padding(5);
            this.txt_confirm.ShowText = false;
            this.txt_confirm.Size = new System.Drawing.Size(200, 30);
            this.txt_confirm.TabIndex = 2;
            this.txt_confirm.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txt_confirm.Watermark = "";
            // 
            // btn_register
            // 
            this.btn_register.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_register.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_register.Location = new System.Drawing.Point(100, 320);
            this.btn_register.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_register.Name = "btn_register";
            this.btn_register.Size = new System.Drawing.Size(100, 45);
            this.btn_register.TabIndex = 3;
            this.btn_register.Text = "注册";
            this.btn_register.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_register.Click += new System.EventHandler(this.btn_register_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_cancel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_cancel.Location = new System.Drawing.Point(220, 320);
            this.btn_cancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(100, 45);
            this.btn_cancel.TabIndex = 3;
            this.btn_cancel.Text = "取消";
            this.btn_cancel.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // RegisterForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(420, 420);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_register);
            this.Controls.Add(this.txt_confirm);
            this.Controls.Add(this.txt_password);
            this.Controls.Add(this.txt_username);
            this.Controls.Add(this.lbl_confirm);
            this.Controls.Add(this.lbl_password);
            this.Controls.Add(this.lbl_username);
            this.Controls.Add(this.lbl_title);
            this.MaximizeBox = false;
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "用户注册";
            this.ZoomScaleRect = new System.Drawing.Rectangle(22, 22, 420, 420);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sunny.UI.UILabel lbl_title;
        private Sunny.UI.UILabel lbl_username;
        private Sunny.UI.UILabel lbl_password;
        private Sunny.UI.UILabel lbl_confirm;
        private Sunny.UI.UITextBox txt_username;
        private Sunny.UI.UITextBox txt_password;
        private Sunny.UI.UITextBox txt_confirm;
        private Sunny.UI.UIButton btn_register;
        private Sunny.UI.UIButton btn_cancel;
    }
}
