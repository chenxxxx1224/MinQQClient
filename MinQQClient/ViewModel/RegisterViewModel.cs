using System.Windows.Forms;
using MinQQClient.HelperClass;

namespace MinQQClient.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly TcpClientHelper _client;

        public RegisterViewModel(TcpClientHelper client)
        {
            _client = client;
        }

        public async void Register(string username, string password, string confirm, Form currentForm)
        {
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

            string content = $"{username}|{password}";
            await _client.SendMessageAsync(10, content);

            var response = await _client.ReceiveMessageAsync();

            if (response != null && response.Content.StartsWith("success"))
            {
                string[] parts = response.Content.Split('|');
                int newUserId = int.Parse(parts[1]);
                MessageBox.Show($"注册成功！您的用户ID是：{newUserId}，请牢记！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                currentForm.Close();
            }
            else
            {
                MessageBox.Show(response?.Content ?? "注册失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
