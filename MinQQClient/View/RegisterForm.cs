using System;
using System.Windows.Forms;
using MinQQClient.HelperClass;
using MinQQClient.ViewModel;

namespace MinQQClient.View
{
    public partial class RegisterForm : Sunny.UI.UIForm
    {
        private TcpClientHelper _client;
        private RegisterViewModel _viewModel;

        public RegisterForm(TcpClientHelper client)
        {
            InitializeComponent();
            _client = client;
            _viewModel = new RegisterViewModel(_client);
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            string username = txt_username.Text.Trim();
            string password = txt_password.Text.Trim();
            string confirm = txt_confirm.Text.Trim();
            _viewModel.Register(username, password, confirm, this);
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
