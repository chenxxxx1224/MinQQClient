using System;
using System.Windows.Forms;

namespace MinQQClient
{
    public partial class LoginForm : Sunny.UI.UIForm
    {
        private Client client;

        public LoginForm()
        {
            InitializeComponent();
            client = new Client();
            ConnectToServer();
        }

        private async void ConnectToServer()
        {
            try
            {
                await client.ConnectAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接服务器失败：" + ex.Message);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string account = txt_user.Text.Trim();
            string password = txt_pwd.Text.Trim();

            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("请输入账号和密码！", "提示");
                return;
            }

            string content = $"{account}|{password}";

            await client.SendMessageAsync(1, content);

            NetMessage response = await client.ReceiveMessageAsync();

            if (response != null && response.Content.StartsWith("success"))
            {
                // 格式：success|userId|username
                string[] parts = response.Content.Split('|');
                client.UserId = int.Parse(parts[1]);
                client.Username = parts[2];

                MessageBox.Show("登录成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 跳转到主界面，传递client对象
                this.Hide();
                Main mainForm = new Main(client);
                mainForm.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("账号或密码错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            // 打开注册窗口
            RegisterForm registerForm = new RegisterForm(client);
            registerForm.ShowDialog();
        }
    }
}
