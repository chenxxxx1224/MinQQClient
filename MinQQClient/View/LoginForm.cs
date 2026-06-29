using System;
using System.Windows.Forms;
using MinQQClient.HelperClass;
using MinQQClient.ViewModel;

namespace MinQQClient.View
{
    public partial class LoginForm : Sunny.UI.UIForm
    {
        private TcpClientHelper _client;
        private LoginViewModel _viewModel;

        public LoginForm()
        {
            InitializeComponent();
            _client = new TcpClientHelper();
            _viewModel = new LoginViewModel(_client);
            _viewModel.Connect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string account = txt_user.Text.Trim();
            string password = txt_pwd.Text.Trim();
            _viewModel.Login(account, password, this);
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm(_client);
            registerForm.ShowDialog();
        }
    }
}
