using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinQQClient
{
    public partial class Main : Form
    {
        private Client client;
        private List<int> onlineUserIds = new List<int>();  // 在线用户ID列表
        private Dictionary<int, string> onlineUsers = new Dictionary<int, string>();  // 用户ID -> 用户名
        private int currentChatUserId = -1;  // 当前聊天对象ID
        private Dictionary<int, List<ChatMessage>> chatHistories = new Dictionary<int, List<ChatMessage>>();  // 聊天记录

        public Main(Client client)
        {
            InitializeComponent();
            this.client = client;

            // 设置标题
            lblTitle.Text = $"欢迎，{client.Username}";
            this.Text = $"MinQQ - {client.Username}";

            // 初始化聊天记录
            chatHistories[0] = new List<ChatMessage>();  // 0表示群聊或系统消息

            // 启动后台接收消息
            Task.Run(() => ReceiveMessagesLoop());

            // 主动获取在线用户列表
            LoadOnlineUsers();
        }

        // 加载在线用户列表
        public async void LoadOnlineUsers()
        {
            try
            {
                // 向服务端请求在线用户列表（MsgType = 3）
                await client.SendMessageAsync(3, "");
            }
            catch (Exception ex)
            {
                AppendSystemMessage($"加载在线用户失败：{ex.Message}");
            }
        }

        // 后台持续接收消息的循环
        private async Task ReceiveMessagesLoop()
        {
            byte[] buffer = new byte[4096];
            string remainingData = "";

            try
            {
                var stream = client.GetStream();
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    remainingData += Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    int newlineIndex;
                    while ((newlineIndex = remainingData.IndexOf('\n')) != -1)
                    {
                        string completeJson = remainingData.Substring(0, newlineIndex).Trim();
                        remainingData = remainingData.Substring(newlineIndex + 1);

                        if (!string.IsNullOrEmpty(completeJson))
                        {
                            var msg = JsonSerializer.Deserialize<NetMessage>(completeJson);
                            HandleServerMessage(msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendSystemMessage($"连接已断开：{ex.Message}");
            }
        }

        // 处理收到的消息
        private void HandleServerMessage(NetMessage msg)
        {
            switch (msg.MsgType)
            {
                case 3:  // 在线用户列表
                    UpdateOnlineUsers(msg.Content);
                    break;
                case 2:  // 收到聊天消息
                    ReceiveChatMessage(msg.Content);
                    break;
                case 100:  // 登录成功响应（不处理）
                    break;
                case 101:  // 登录失败响应（不处理）
                    break;
            }
        }

        // 更新在线用户列表
        private void UpdateOnlineUsers(string content)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateOnlineUsers), content);
                return;
            }

            // 格式：userId:username,userId:username,...
            lstOnlineUsers.Items.Clear();
            onlineUsers.Clear();
            onlineUserIds.Clear();

            if (!string.IsNullOrEmpty(content))
            {
                string[] users = content.Split(',');
                foreach (string user in users)
                {
                    if (string.IsNullOrEmpty(user)) continue;
                    string[] parts = user.Split(':');
                    if (parts.Length >= 2)
                    {
                        int userId = int.Parse(parts[0]);
                        string username = parts[1];

                        // 不显示自己
                        if (userId == client.UserId) continue;

                        onlineUsers[userId] = username;
                        onlineUserIds.Add(userId);
                        lstOnlineUsers.Items.Add(username);
                    }
                }
            }

            // 默认选中第一个用户
            if (lstOnlineUsers.Items.Count > 0)
            {
                lstOnlineUsers.SelectedIndex = 0;
            }
        }

        // 收到聊天消息
        private void ReceiveChatMessage(string content)
        {
            // 格式：senderId:message
            string[] parts = content.Split(new string[] { ":" }, 2, StringSplitOptions.None);
            if (parts.Length < 2) return;

            int senderId = int.Parse(parts[0]);
            string message = parts[1];

            // 获取发送者用户名
            string senderName = senderId == client.UserId ? "我" :
                               (onlineUsers.ContainsKey(senderId) ? onlineUsers[senderId] : "未知用户");

            // 添加到聊天记录
            if (!chatHistories.ContainsKey(senderId))
            {
                chatHistories[senderId] = new List<ChatMessage>();
            }
            chatHistories[senderId].Add(new ChatMessage { IsMine = false, SenderName = senderName, Content = message });

            // 如果是当前聊天对象，显示消息
            if (senderId == currentChatUserId)
            {
                AppendChatMessage(false, senderName, message);
            }
            else
            {
                // 未读提示
                AppendSystemMessage($"收到来自 {senderName} 的消息");
            }
        }

        // 发送消息
        private async void btnSend_Click_1(object sender, EventArgs e)
        {
            string message = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (currentChatUserId == -1)
            {
                MessageBox.Show("请先选择一个聊天对象！", "提示");
                return;
            }

            try
            {
                // 格式：receiverId:message
                string content = $"{currentChatUserId}:{message}";
                await client.SendMessageAsync(2, content);

                // 显示自己发送的消息
                string targetName = onlineUsers.ContainsKey(currentChatUserId) ? onlineUsers[currentChatUserId] : "未知用户";
                AppendChatMessage(true, "我", message);

                // 保存到聊天记录
                if (!chatHistories.ContainsKey(currentChatUserId))
                {
                    chatHistories[currentChatUserId] = new List<ChatMessage>();
                }
                chatHistories[currentChatUserId].Add(new ChatMessage { IsMine = true, SenderName = "我", Content = message });

                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送失败：" + ex.Message, "错误");
            }
        }

        // 选择聊天对象
        private void lstOnlineUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstOnlineUsers.SelectedIndex >= 0 && lstOnlineUsers.SelectedIndex < onlineUserIds.Count)
            {
                currentChatUserId = onlineUserIds[lstOnlineUsers.SelectedIndex];
                string targetName = onlineUsers[currentChatUserId];
                lblTitle.Text = $"正在和 {targetName} 聊天";

                // 显示与该用户的聊天记录
                DisplayChatHistory();
            }
        }

        // 显示当前聊天记录
        private void DisplayChatHistory()
        {
            rtbChatHistory.Clear();

            if (chatHistories.ContainsKey(currentChatUserId))
            {
                foreach (var msg in chatHistories[currentChatUserId])
                {
                    AppendChatMessage(msg.IsMine, msg.SenderName, msg.Content, addNewLine: false);
                }
            }
        }

        // 添加聊天消息到富文本框
        private void AppendChatMessage(bool isMine, string senderName, string content, bool addNewLine = true)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendChatMessage(isMine, senderName, content, addNewLine)));
                return;
            }

            Color color = isMine ? Color.Blue : Color.Green;
            string prefix = isMine ? "我" : senderName;

            rtbChatHistory.SelectionColor = color;
            rtbChatHistory.AppendText($"[{prefix}] {content}");
            if (addNewLine)
            {
                rtbChatHistory.AppendText(Environment.NewLine);
            }
            rtbChatHistory.ScrollToCaret();
        }

        // 添加系统消息
        private void AppendSystemMessage(string content)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendSystemMessage), content);
                return;
            }

            rtbChatHistory.SelectionColor = Color.Gray;
            rtbChatHistory.AppendText($"[系统] {content}{Environment.NewLine}");
            rtbChatHistory.ScrollToCaret();
        }

       
    }

    // 聊天消息结构
    public class ChatMessage
    {
        public bool IsMine { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
    }
}
