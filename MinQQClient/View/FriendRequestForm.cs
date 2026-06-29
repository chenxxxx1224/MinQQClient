using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using MinQQClient.HelperClass;
using MinQQClient.Model;
using MinQQClient.ViewModel;

namespace MinQQClient.View
{
    public partial class FriendRequestForm : Sunny.UI.UIForm
    {
        private TcpClientHelper _client;
        private FriendRequestViewModel _viewModel;
        private List<FriendRequestInfo> _requests = new List<FriendRequestInfo>();

        public FriendRequestForm()
        {
            InitializeComponent();
        }

        public FriendRequestForm(TcpClientHelper client)
        {
            InitializeComponent();
            _client = client;
            _viewModel = new FriendRequestViewModel(_client);

            LoadRequests();
            _viewModel.StartAutoRefresh(LoadRequests);
        }

        private void LoadRequests()
        {
            Task.Run(async () =>
            {
                string content = await _viewModel.LoadRequestsAsync();
                if (content == null) return;
                if (IsDisposed) return;
                BeginInvoke(new Action<string>(UpdateRequestsList), content);
            });
        }

        private void UpdateRequestsList(string content)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateRequestsList), content);
                return;
            }

            lstRequests.Items.Clear();
            _requests.Clear();
            _requests = _viewModel.ParseRequests(content);

            foreach (var req in _requests)
            {
                lstRequests.Items.Add($"{req.FromUsername} (ID: {req.FromUserId})");
            }

            if (lstRequests.Items.Count == 0)
            {
                lstRequests.Items.Add("暂无好友申请");
            }
        }

        private async void btnAccept_Click(object sender, EventArgs e)
        {
            if (lstRequests.SelectedIndex < 0 || lstRequests.SelectedIndex >= _requests.Count)
            {
                MessageBox.Show("请选择要处理的好友申请！", "提示");
                return;
            }

            var request = _requests[lstRequests.SelectedIndex];
            string result = await _viewModel.ProcessRequestAsync(request, "accept");

            if (result == "success")
            {
                MessageBox.Show($"已同意 {request.FromUsername} 的好友请求！", "提示");
                LoadRequests();
            }
            else if (result != null)
            {
                MessageBox.Show("操作失败：" + result, "提示");
            }
            else
            {
                MessageBox.Show("操作超时，请重试！", "提示");
            }
        }

        private async void btnReject_Click(object sender, EventArgs e)
        {
            if (lstRequests.SelectedIndex < 0 || lstRequests.SelectedIndex >= _requests.Count)
            {
                MessageBox.Show("请选择要处理的好友申请！", "提示");
                return;
            }

            var request = _requests[lstRequests.SelectedIndex];
            string result = await _viewModel.ProcessRequestAsync(request, "reject");

            if (result == "success")
            {
                MessageBox.Show($"已拒绝 {request.FromUsername} 的好友请求！", "提示");
                LoadRequests();
            }
            else if (result != null)
            {
                MessageBox.Show("操作失败：" + result, "提示");
            }
            else
            {
                MessageBox.Show("操作超时，请重试！", "提示");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRequests();
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
