using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Text.Json;

namespace MinQQClient
{
    public partial class FriendRequestForm : Sunny.UI.UIForm
    {
        private Client client;
        private List<FriendRequestInfo> requests = new List<FriendRequestInfo>();
        private System.Threading.Timer refreshTimer;

        // 用于等待消息响应的同步原语
        private AutoResetEvent loadResponseEvent = new AutoResetEvent(false);
        private AutoResetEvent processResponseEvent = new AutoResetEvent(false);
        private NetMessage loadResponse;
        private NetMessage processResponse;

        public FriendRequestForm(Client client)
        {
            InitializeComponent();
            this.client = client;

            // 订阅消息事件2
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
            // 获取申请列表响应 (MsgType = 6)
            if (msg.MsgType == 6 && loadResponse == null)
            {
                loadResponse = msg;
                loadResponseEvent.Set();
            }
            // 处理申请响应 (MsgType = 7)
            else if (msg.MsgType == 7 && processResponse == null)
            {
                processResponse = msg;
                processResponseEvent.Set();
            }
        }

        // 加载好友申请列表
        public void LoadRequests()
        {
            try
            {
                // 重置响应状态
                loadResponse = null;

                // 发送请求（MsgType = 6）
                client.SendMessageAsync(6, "");

                // 等待响应（最多等5秒）
                bool gotResponse = loadResponseEvent.WaitOne(5000);

                if (gotResponse && loadResponse != null && loadResponse.MsgType == 6)
                {
                    // 格式：requestId:fromUserId:fromUsername,requestId:fromUserId:fromUsername,...
                    UpdateRequestsList(loadResponse.Content);
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
                // 重置响应状态
                processResponse = null;

                // 发送响应（MsgType = 7, 格式：requestId:accept）
                client.SendMessageAsync(7, $"{request.RequestId}:accept");

                // 等待响应（最多等5秒）
                bool gotResponse = processResponseEvent.WaitOne(5000);

                if (gotResponse && processResponse != null && processResponse.MsgType == 7)
                {
                    if (processResponse.Content == "success")
                    {
                        MessageBox.Show($"已同意 {request.FromUsername} 的好友请求！", "提示");
                        LoadRequests();  // 刷新列表
                    }
                    else
                    {
                        MessageBox.Show("操作失败：" + processResponse.Content, "提示");
                    }
                }
                else
                {
                    MessageBox.Show("操作超时，请重试！", "提示");
                }
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
                // 重置响应状态
                processResponse = null;

                // 发送响应（MsgType = 7, 格式：requestId:reject）
                client.SendMessageAsync(7, $"{request.RequestId}:reject");

                // 等待响应（最多等5秒）
                bool gotResponse = processResponseEvent.WaitOne(5000);

                if (gotResponse && processResponse != null && processResponse.MsgType == 7)
                {
                    if (processResponse.Content == "success")
                    {
                        MessageBox.Show($"已拒绝 {request.FromUsername} 的好友请求！", "提示");
                        LoadRequests();  // 刷新列表
                    }
                    else
                    {
                        MessageBox.Show("操作失败：" + processResponse.Content, "提示");
                    }
                }
                else
                {
                    MessageBox.Show("操作超时，请重试！", "提示");
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
            // 取消订阅
            client.OnMessageReceived -= OnServerMessage;
            loadResponseEvent.Dispose();
            processResponseEvent.Dispose();
            refreshTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
