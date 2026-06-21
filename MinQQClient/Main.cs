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
    public partial class Main : Sunny.UI.UIForm
    {
        private Client client;
        private List<int> onlineUserIds = new List<int>();  // 在线用户ID列表
        private Dictionary<int, string> onlineUsers = new Dictionary<int, string>();  // 用户ID -> 用户名
        private int currentChatUserId = -1;  // 当前聊天对象ID
        private Dictionary<int, List<ChatMessage>> chatHistories = new Dictionary<int, List<ChatMessage>>();  // 聊天记录
        private Dictionary<int, int> unreadCounts = new Dictionary<int, int>();  // 好友未读消息计数
        private readonly object _friendListLock = new object();  // 好友列表同步锁
        private bool _isUpdatingList = false;  // 防止事件重入的标志
        private DateTime _lastSelectedIndexChanged = DateTime.MinValue;  // 上次触发时间
        private const int SELECTED_INDEX_CHANGED_THRESHOLD_MS = 50;  // 触发间隔阈值（毫秒）

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

            // 注意：好友列表由 HandleLogin 成功登录后自动返回，无需主动请求
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
                            try
                            {
                                var msg = JsonSerializer.Deserialize<NetMessage>(completeJson);
                                HandleServerMessage(msg);
                                // 触发消息事件，通知其他订阅者（如 AddFriendForm、FriendRequestForm）
                                client.RaiseMessageReceived(msg);
                            }
                            catch (Exception ex)
                            {
                                System.IO.File.AppendAllText("client_error.log", $"{DateTime.Now}: Handle message error: {ex}\n");
                            }
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
                case 8:  // 好友申请通知
                    HandleFriendRequestNotification(msg.Content);
                    break;
                case 9:  // 离线消息推送
                    ReceiveOfflineMessage(msg.Content);
                    break;
                case 11:  // 离线消息推送完毕
                    HandleOfflineMessagesComplete(msg.Content);
                    break;
                case 100:  // 登录成功响应（不处理）
                    break;
                case 101:  // 登录失败响应（不处理）
                    break;
            }
        }

        // 处理离线消息
        private void ReceiveOfflineMessage(string content)
        {
            // 格式：senderId|senderName|content|sendTime
            string[] parts = content.Split('|');
            if (parts.Length < 4) return;

            int senderId = int.Parse(parts[0]);
            string senderName = parts[1];
            string message = parts[2];
            string sendTime = parts[3];

            // 只更新数据，不更新 UI（UI 由 UpdateOnlineUsers 统一处理）
            // 检查好友是否已在数据结构中
            if (!onlineUsers.ContainsKey(senderId))
            {
                lock (_friendListLock)
                {
                    onlineUsers[senderId] = senderName;
                    onlineUserIds.Add(senderId);
                }
            }

            // 添加到聊天记录
            if (!chatHistories.ContainsKey(senderId))
            {
                chatHistories[senderId] = new List<ChatMessage>();
            }
            chatHistories[senderId].Add(new ChatMessage { IsMine = false, SenderName = senderName, Content = message });

            // 增加未读计数
            if (!unreadCounts.ContainsKey(senderId))
            {
                unreadCounts[senderId] = 0;
            }
            unreadCounts[senderId]++;

            // 显示系统消息提示
            AppendSystemMessage($"收到离线消息 来自 {senderName}: {message}");
        }

        // 离线消息推送完毕
        private void HandleOfflineMessagesComplete(string content)
        {
            try
            {
                int count = int.Parse(content);
                AppendSystemMessage($"已接收 {count} 条离线消息");
            }
            catch { }
        }

        // 添加好友到列表（带徽章）
        private void AddFriendToListBox(int friendId, string friendName, int unreadCount)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => AddFriendToListBox(friendId, friendName, unreadCount)));
                return;
            }

            // 防止重复添加：检查好友是否已经在 UI 列表中
            bool alreadyInList = false;
            for (int i = 0; i < lstOnlineUsers.Items.Count; i++)
            {
                string item = lstOnlineUsers.Items[i].ToString();
                if (item == friendName || item.StartsWith(friendName + " "))
                {
                    alreadyInList = true;
                    break;
                }
            }

            if (alreadyInList) return;

            // 直接添加带徽章的文本（不使用 Items[index] 修改）
            string displayText = unreadCount > 0 ? $"{friendName} 🔴{unreadCount}" : friendName;
            lstOnlineUsers.Items.Add(displayText);
        }

        // 更新好友列表徽章显示
        private void UpdateFriendBadge(int friendId)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(UpdateFriendBadge), friendId);
                return;
            }

            // 设置标志，防止 RemoveAt+Insert 触发 SelectedIndexChanged 时清空未读计数
            _isUpdatingList = true;

            lock (_friendListLock)
            {
                if (!onlineUsers.ContainsKey(friendId) || !onlineUserIds.Contains(friendId))
                {
                    _isUpdatingList = false;
                    return;
                }

                string username = onlineUsers[friendId];
                int index = onlineUserIds.IndexOf(friendId);

                if (index < 0 || index >= lstOnlineUsers.Items.Count)
                {
                    _isUpdatingList = false;
                    return;
                }

                int unread = unreadCounts.ContainsKey(friendId) ? unreadCounts[friendId] : 0;
                string newText = unread > 0 ? $"{username} 🔴{unread}" : username;

                // 使用 Remove + Add 替换文本
                lstOnlineUsers.Items.RemoveAt(index);
                lstOnlineUsers.Items.Insert(index, newText);
            }

            _isUpdatingList = false;
        }

        // 处理好友申请通知
        private void HandleFriendRequestNotification(string content)
        {
            // 检查是否需要刷新好友列表
            bool needRefresh = content.Contains("|refresh");
            if (needRefresh)
            {
                content = content.Replace("|refresh", "");
            }

            if (content.StartsWith("accepted:"))
            {
                // 对方同意了好友请求
                string[] parts = content.Substring("accepted:".Length).Split(':');
                if (parts.Length >= 2)
                {
                    string username = parts[1];
                    AppendSystemMessage($"{username} 已同意您的好友请求！现在可以开始聊天了。");
                }
                // 刷新好友列表
                LoadOnlineUsers();
            }
            else if (content.StartsWith("friend_added:"))
            {
                // 自己同意了对方的好友请求，刷新自己的好友列表
                AppendSystemMessage($"已添加好友！");
                LoadOnlineUsers();
            }
            else
            {
                // 收到好友请求
                string[] parts = content.Split(':');
                if (parts.Length >= 2)
                {
                    string fromUsername = parts[1];
                    AppendSystemMessage($"{fromUsername} 向你发送了好友请求，请在【好友申请】中处理！");
                }
            }
        }

        // 更新在线用户列表
        private void UpdateOnlineUsers(string content)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateOnlineUsers), content);
                return;
            }

            // 先设置标志，再清空列表
            _isUpdatingList = true;

            lstOnlineUsers.Items.Clear();

            lock (_friendListLock)
            {
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

                            // 检查是否有未读消息，有则显示徽章
                            int unread = unreadCounts.ContainsKey(userId) ? unreadCounts[userId] : 0;
                            string displayText = unread > 0 ? $"{username} 🔴{unread}" : username;
                            lstOnlineUsers.Items.Add(displayText);
                        }
                    }
                }

                // 默认选中第一个用户
                if (lstOnlineUsers.Items.Count > 0)
                {
                    lstOnlineUsers.SelectedIndex = 0;
                }
            }

            // 延迟重置标志
            Task.Delay(200).ContinueWith(_ => _isUpdatingList = false);
        }

        // 收到聊天消息
        private void ReceiveChatMessage(string content)
        {
            // 检查是否是错误响应
            if (content.StartsWith("not_friend|"))
            {
                string errorMsg = content.Substring("not_friend|".Length);
                AppendSystemMessage(errorMsg);
                return;
            }

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

            // 如果是当前聊天对象，显示消息；否则增加未读计数
            if (senderId == currentChatUserId)
            {
                AppendChatMessage(false, senderName, message);
            }
            else
            {
                // 增加未读计数并更新徽章
                if (!unreadCounts.ContainsKey(senderId))
                {
                    unreadCounts[senderId] = 0;
                }
                unreadCounts[senderId]++;
                UpdateFriendBadge(senderId);

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
            // 防止短时间内多次触发
            var now = DateTime.Now;
            if ((now - _lastSelectedIndexChanged).TotalMilliseconds < SELECTED_INDEX_CHANGED_THRESHOLD_MS)
            {
                return; // 忽略过快触发的事件
            }
            _lastSelectedIndexChanged = now;

            if (_isUpdatingList) return;  // 防止在更新列表时重入

            if (lstOnlineUsers.SelectedIndex >= 0 && lstOnlineUsers.SelectedIndex < onlineUserIds.Count)
            {
                int selectedFriendId = onlineUserIds[lstOnlineUsers.SelectedIndex];

                // 清空该好友的未读计数（只在用户主动选择时清零）
                if (unreadCounts.ContainsKey(selectedFriendId) && unreadCounts[selectedFriendId] > 0)
                {
                    unreadCounts[selectedFriendId] = 0;
                    UpdateFriendBadge(selectedFriendId);
                }

                currentChatUserId = selectedFriendId;
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
                    AppendChatMessage(msg.IsMine, msg.SenderName, msg.Content, addNewLine: true);
                }
            }
        }

        // 添加聊天消息到富文本框
        private void AppendChatMessage(bool isMine, string senderName, string content, bool addNewLine = true)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => AppendChatMessage(isMine, senderName, content, addNewLine)));
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
                BeginInvoke(new Action<string>(AppendSystemMessage), content);
                return;
            }

            rtbChatHistory.SelectionColor = Color.Gray;
            rtbChatHistory.AppendText($"[系统] {content}{Environment.NewLine}");
            rtbChatHistory.ScrollToCaret();
        }

        // 添加好友按钮点击事件
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddFriend addFriendForm = new AddFriend(client);
            addFriendForm.ShowDialog();
        }

        // 好友申请按钮点击事件
        private void btnApply_Click(object sender, EventArgs e)
        {
            FriendRequestForm requestForm = new FriendRequestForm(client);
            requestForm.ShowDialog();
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
