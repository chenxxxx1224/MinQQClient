using System;
using System.Windows.Forms;
using MinQQClient.HelperClass;
using MinQQClient.ViewModel;

namespace MinQQClient.View
{
    public partial class AddFriendForm : Sunny.UI.UIForm
    {
        private TcpClientHelper _client;
        private AddFriendViewModel _viewModel;

        public AddFriendForm(TcpClientHelper client)
        {
            InitializeComponent();
            _client = client;
            _viewModel = new AddFriendViewModel(_client);
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string userIdStr = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(userIdStr))
            {
                MessageBox.Show("请输入用户ID！", "提示");
                return;
            }

            if (!int.TryParse(userIdStr, out int userId))
            {
                MessageBox.Show("用户ID必须是数字！", "提示");
                return;
            }

            await _viewModel.SearchAsync(userId, text => txtResult.Text = text);
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            await _viewModel.AddAsync(() => this.Close());
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _viewModel.Cleanup();
            base.OnFormClosing(e);
        }
    }
}
