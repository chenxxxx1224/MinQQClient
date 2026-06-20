using System;
using System.Windows.Forms;

namespace MinQQClient
{
    public partial class RegisterForm : Sunny.UI.UIForm
    {
        private Client client;

        public RegisterForm(Client client)
        {
            InitializeComponent();
            this.client = client;
        }

        private async void btn_register_Click(object sender, EventArgs e)
        {
            string username = txt_username.Text.Trim();
            string password = txt_password.Text.Trim();
            string confirm = txt_confirm.Text.Trim();

            // 验证输入
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("请输入用户名和密码！", "提示");
                return;
            }

            if (username.Length < 3 || password.Length < 3)
            {
                MessageBox.Show("用户名和密码长度不能少于3位！", "提示");
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("两次输入的密码不一致！", "提示");
                return;
            }

            // 发送注册请求（MsgType = 10）
            string content = $"{username}|{password}";
            await client.SendMessageAsync(10, content);

            NetMessage response = await client.ReceiveMessageAsync();

            if (response != null && response.Content.StartsWith("success"))
            {
                // 格式：success|userId
                string[] parts = response.Content.Split('|');
                int newUserId = int.Parse(parts[1]);
                MessageBox.Show($"注册成功！您的用户ID是：{newUserId}，请牢记！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();  // 关闭注册窗口
            }
            else
            {
                MessageBox.Show(response?.Content ?? "注册失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();  // 关闭注册窗口
        }
    }
}
