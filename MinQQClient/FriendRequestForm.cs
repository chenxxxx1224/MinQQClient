using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.Json;

namespace MinQQClient
{
    public partial class FriendRequestForm : Form
    {
        private Client client;
        private List<FriendRequestInfo> requests = new List<FriendRequestInfo>();
        private System.Threading.Timer refreshTimer;

        public FriendRequestForm(Client client)
        {
            InitializeComponent();
            this.client = client;

            // 加载好友申请列表
            LoadRequests();

            // 每5秒自动刷新一次
            refreshTimer = new System.Threading.Timer(_ =>
            {
                this.BeginInvoke(new Action(LoadRequests));
            }, null, 5000, 5000);
        }

        // 加载好友申请列表
        public async void LoadRequests()
        {
            try
            {
                // 发送请求（MsgType = 6）
                await client.SendMessageAsync(6, "");

                var response = await client.ReceiveMessageAsync();

                if (response != null && response.MsgType == 6)
                {
                    // 格式：requestId:fromUserId:fromUsername,requestId:fromUserId:fromUsername,...
                    UpdateRequestsList(response.Content);
                }
            }
            catch (Exception ex)
            {
                // 忽略错误，避免频繁刷新报错
            }
        }

        // 更新好友申请列表
        private void UpdateRequestsList(string content)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateRequestsList), content);
                return;
            }

            lstRequests.Items.Clear();
            requests.Clear();

            if (!string.IsNullOrEmpty(content))
            {
                string[] items = content.Split(',');
                foreach (string item in items)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    string[] parts = item.Split(':');
                    if (parts.Length >= 3)
                    {
                        int requestId = int.Parse(parts[0]);
                        int fromUserId = int.Parse(parts[1]);
                        string fromUsername = parts[2];

                        requests.Add(new FriendRequestInfo
                        {
                            RequestId = requestId,
                            FromUserId = fromUserId,
                            FromUsername = fromUsername
                        });

                        lstRequests.Items.Add($"{fromUsername} (ID: {fromUserId})");
                    }
                }
            }

            // 显示状态
            if (lstRequests.Items.Count == 0)
            {
                lstRequests.Items.Add("暂无好友申请");
            }
        }

        // 同意好友请求
        private async void btnAccept_Click(object sender, EventArgs e)
        {
            if (lstRequests.SelectedIndex < 0 || lstRequests.SelectedIndex >= requests.Count)
            {
                MessageBox.Show("请选择要处理的好友申请！", "提示");
                return;
            }

            var request = requests[lstRequests.SelectedIndex];

            try
            {
                // 发送响应（MsgType = 7, 格式：requestId:accept）
                await client.SendMessageAsync(7, $"{request.RequestId}:accept");

                var response = await client.ReceiveMessageAsync();

                if (response != null && response.MsgType == 7)
                {
                    if (response.Content == "success")
                    {
                        MessageBox.Show($"已同意 {request.FromUsername} 的好友请求！", "提示");
                        LoadRequests();  // 刷新列表
                    }
                    else
                    {
                        MessageBox.Show("操作失败：" + response.Content, "提示");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败：" + ex.Message, "错误");
            }
        }

        // 拒绝好友请求
        private async void btnReject_Click(object sender, EventArgs e)
        {
            if (lstRequests.SelectedIndex < 0 || lstRequests.SelectedIndex >= requests.Count)
            {
                MessageBox.Show("请选择要处理的好友申请！", "提示");
                return;
            }

            var request = requests[lstRequests.SelectedIndex];

            try
            {
                // 发送响应（MsgType = 7, 格式：requestId:reject）
                await client.SendMessageAsync(7, $"{request.RequestId}:reject");

                var response = await client.ReceiveMessageAsync();

                if (response != null && response.MsgType == 7)
                {
                    if (response.Content == "success")
                    {
                        MessageBox.Show($"已拒绝 {request.FromUsername} 的好友请求！", "提示");
                        LoadRequests();  // 刷新列表
                    }
                    else
                    {
                        MessageBox.Show("操作失败：" + response.Content, "提示");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败：" + ex.Message, "错误");
            }
        }

        // 刷新按钮
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRequests();
        }

        // 关闭
        private void btnClose_Click(object sender, EventArgs e)
        {
            refreshTimer?.Dispose();
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            refreshTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }

    // 好友申请信息
    public class FriendRequestInfo
    {
        public int RequestId { get; set; }
        public int FromUserId { get; set; }
        public string FromUsername { get; set; }
    }
}
