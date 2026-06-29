namespace MinQQClient
{
    partial class LoginForm
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
            this.btn_login = new Sunny.UI.UIButton();
            this.btn_register = new Sunny.UI.UIButton();
            this.lbl_user = new Sunny.UI.UILabel();
            this.lbl_pwd = new Sunny.UI.UILabel();
            this.txt_user = new Sunny.UI.UITextBox();
            this.txt_pwd = new Sunny.UI.UITextBox();
            this.SuspendLayout();
            // 
            // btn_login
            // 
            this.btn_login.Location = new System.Drawing.Point(120, 280);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(120, 50);
            this.btn_login.TabIndex = 0;
            this.btn_login.Text = "登录";
            this.btn_login.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_register
            // 
            this.btn_register.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_register.Location = new System.Drawing.Point(280, 280);
            this.btn_register.Name = "btn_register";
            this.btn_register.Size = new System.Drawing.Size(120, 50);
            this.btn_register.TabIndex = 0;
            this.btn_register.Text = "注册";
            this.btn_register.Click += new System.EventHandler(this.btn_register_Click);
            // 
            // lbl_user
            // 
            this.lbl_user.AutoSize = true;
            this.lbl_user.Location = new System.Drawing.Point(90, 100);
            this.lbl_user.Name = "lbl_user";
            this.lbl_user.Size = new System.Drawing.Size(60, 30);
            this.lbl_user.TabIndex = 1;
            this.lbl_user.Text = "账号：";
            // 
            // lbl_pwd
            // 
            this.lbl_pwd.AutoSize = true;
            this.lbl_pwd.Location = new System.Drawing.Point(90, 180);
            this.lbl_pwd.Name = "lbl_pwd";
            this.lbl_pwd.Size = new System.Drawing.Size(60, 30);
            this.lbl_pwd.TabIndex = 1;
            this.lbl_pwd.Text = "密码：";
            // 
            // txt_user
            // 
            this.txt_user.Location = new System.Drawing.Point(180, 95);
            this.txt_user.Name = "txt_user";
            this.txt_user.Size = new System.Drawing.Size(220, 30);
            this.txt_user.TabIndex = 2;
            // 
            // txt_pwd
            // 
            this.txt_pwd.Location = new System.Drawing.Point(180, 175);
            this.txt_pwd.Name = "txt_pwd";
            this.txt_pwd.Size = new System.Drawing.Size(220, 30);
            this.txt_pwd.TabIndex = 2;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 380);
            this.Controls.Add(this.txt_pwd);
            this.Controls.Add(this.txt_user);
            this.Controls.Add(this.lbl_pwd);
            this.Controls.Add(this.lbl_user);
            this.Controls.Add(this.btn_register);
            this.Controls.Add(this.btn_login);
            this.Name = "LoginForm";
            this.Text = "QQ登录界面";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sunny.UI.UIButton btn_login;
        private Sunny.UI.UIButton btn_register;
        private Sunny.UI.UILabel lbl_user;
        private Sunny.UI.UILabel lbl_pwd;
        private Sunny.UI.UITextBox txt_user;
        private Sunny.UI.UITextBox txt_pwd;
    }
}
