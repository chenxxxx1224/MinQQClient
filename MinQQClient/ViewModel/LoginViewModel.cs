using System.Windows.Forms;
using MinQQClient.HelperClass;
using MinQQClient.View;

namespace MinQQClient.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly TcpClientHelper _client;

        public LoginViewModel(TcpClientHelper client)
        {
            _client = client;
        }

        public async void Connect()
        {
            try
            {
                await _client.ConnectAsync();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("连接服务器失败：" + ex.Message);
            }
        }

        public async void Login(string account, string password, Form currentForm)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("请输入账号和密码！", "提示");
                return;
            }

            string content = $"{account}|{password}";
            await _client.SendMessageAsync(1, content);

            var response = await _client.ReceiveMessageAsync();

            if (response != null && response.Content.StartsWith("success"))
            {
                string[] parts = response.Content.Split('|');
                _client.UserId = int.Parse(parts[1]);
                _client.Username = parts[2];

                MessageBox.Show("登录成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                currentForm.Hide();
                MainForm mainForm = new MainForm(_client);
                mainForm.ShowDialog();
                currentForm.Close();
            }
            else
            {
                MessageBox.Show("账号或密码错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
