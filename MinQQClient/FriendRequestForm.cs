using System;
using System.Collections.Generic;
using System.Threading;
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

            // 订阅消息事件
            client.OnMessageReceived += OnServerMessage;

            // 加载好友申请列表
            LoadRequests();

            // 每5秒自动刷新一次
            refreshTimer = new System.Threading.Timer(_ =>
            {
                this.BeginInvoke(new Action(LoadRequests));
            }, null, 5000, 5000);
        }

        // 处理服务端消息
        private void OnServerMessage(NetMessage msg)
        {
            // 获取申请列表响应 (MsgType = 6)，直接更新列表
            if (msg.MsgType == 6)
            {
                UpdateRequestsList(msg.Content);
            }
            // 处理申请响应 (MsgType = 7)
            else if (msg.MsgType == 7)
            {
                // 直接在事件回调中处理响应，不需要 WaitOne
                if (msg.Content == "success")
                {
                    MessageBox.Show("操作成功！", "提示");
                }
                else
                {
                    MessageBox.Show("操作失败：" + msg.Content, "提示");
                }
                // 刷新列表
                LoadRequests();
            }
        }

        // 加载好友申请列表（非阻塞方式，定时器调用）
        public void LoadRequests()
        {
            try
            {
                // 发送请求（MsgType = 6），不等待响应
                // 响应会通过 OnMessageReceived 事件接收，然后更新列表
                client.SendMessageAsync(6, "");
            }
            catch (Exception ex)
            {
                // 忽略错误
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
        private void btnAccept_Click(object sender, EventArgs e)
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
                // 响应会在 OnServerMessage 中处理
                client.SendMessageAsync(7, $"{request.RequestId}:accept");
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败：" + ex.Message, "错误");
            }
        }

        // 拒绝好友请求
        private void btnReject_Click(object sender, EventArgs e)
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
                // 响应会在 OnServerMessage 中处理
                client.SendMessageAsync(7, $"{request.RequestId}:reject");
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
            // 取消订阅
            client.OnMessageReceived -= OnServerMessage;
            refreshTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
