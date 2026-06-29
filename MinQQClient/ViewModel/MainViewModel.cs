using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MinQQClient.HelperClass;
using MinQQClient.Model;

namespace MinQQClient.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly TcpClientHelper _client;
        private readonly object _friendListLock = new object();
        private bool _isUpdatingList = false;

        public Dictionary<int, string> OnlineUsers { get; private set; } = new Dictionary<int, string>();
        public List<int> OnlineUserIds { get; private set; } = new List<int>();
        public Dictionary<int, List<ChatMessage>> ChatHistories { get; private set; } = new Dictionary<int, List<ChatMessage>>();
        public Dictionary<int, int> UnreadCounts { get; private set; } = new Dictionary<int, int>();

        public int CurrentChatUserId { get; private set; } = -1;

        private string _title;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public event Action<string> ShowSystemMessageRequested;
        public event Action<bool, string, string> ShowChatMessageRequested;
        public event Action<List<string>> UpdateUserListRequested;
        public event Action<int, string> UpdateBadgeRequested;

        public MainViewModel(TcpClientHelper client)
        {
            _client = client;
            Title = $"欢迎，{client.Username}";
            ChatHistories[0] = new List<ChatMessage>();
        }

        public async Task StartReceiveLoop()
        {
            await Task.Run(() => ReceiveMessagesLoop());
        }

        private async Task ReceiveMessagesLoop()
        {
            byte[] buffer = new byte[4096];
            string remainingData = "";

            try
            {
                var stream = _client.GetStream();
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    remainingData += System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    int newlineIndex;
                    while ((newlineIndex = remainingData.IndexOf('\n')) != -1)
                    {
                        string completeJson = remainingData.Substring(0, newlineIndex).Trim();
                        remainingData = remainingData.Substring(newlineIndex + 1);

                        if (!string.IsNullOrEmpty(completeJson))
                        {
                            try
                            {
                                var msg = System.Text.Json.JsonSerializer.Deserialize<NetMessage>(completeJson);
                                HandleServerMessage(msg);
                                _client.RaiseMessageReceived(msg);
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
                ShowSystemMessageRequested?.Invoke($"连接已断开：{ex.Message}");
            }
        }

        private void HandleServerMessage(NetMessage msg)
        {
            switch (msg.MsgType)
            {
                case 3: UpdateOnlineUsers(msg.Content); break;
                case 2: ReceiveChatMessage(msg.Content); break;
                case 8: HandleFriendRequestNotification(msg.Content); break;
                case 9: ReceiveOfflineMessage(msg.Content); break;
                case 11: HandleOfflineMessagesComplete(msg.Content); break;
                case 100: break;
                case 101: break;
            }
        }

        public async void LoadOnlineUsers()
        {
            try
            {
                await _client.SendMessageAsync(3, "");
            }
            catch (Exception ex)
            {
                ShowSystemMessageRequested?.Invoke($"加载在线用户失败：{ex.Message}");
            }
        }

        private void UpdateOnlineUsers(string content)
        {
            _isUpdatingList = true;

            lock (_friendListLock)
            {
                OnlineUsers.Clear();
                OnlineUserIds.Clear();

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

                            if (userId == _client.UserId) continue;

                            OnlineUsers[userId] = username;
                            OnlineUserIds.Add(userId);
                        }
                    }
                }
            }

            // 统一刷新（保留未读红点）
            RefreshUserListWithBadges();

            if (OnlineUserIds.Count > 0 && CurrentChatUserId == -1)
            {
                CurrentChatUserId = OnlineUserIds[0];
                string targetName = OnlineUsers[CurrentChatUserId];
                Title = $"正在和 {targetName} 聊天";
            }

            Task.Delay(200).ContinueWith(_ => _isUpdatingList = false);
        }

        private void ReceiveOfflineMessage(string content)
        {
            string[] parts = content.Split('|');
            if (parts.Length < 4) return;

            int senderId = int.Parse(parts[0]);
            string senderName = parts[1];
            string message = parts[2];
            string sendTime = parts[3];

            lock (_friendListLock)
            {
                if (!OnlineUsers.ContainsKey(senderId))
                {
                    OnlineUsers[senderId] = senderName;
                    OnlineUserIds.Add(senderId);
                }
            }

            if (!ChatHistories.ContainsKey(senderId))
            {
                ChatHistories[senderId] = new List<ChatMessage>();
            }
            ChatHistories[senderId].Add(new ChatMessage { IsMine = false, SenderName = senderName, Content = message });

            if (!UnreadCounts.ContainsKey(senderId))
            {
                UnreadCounts[senderId] = 0;
            }
            UnreadCounts[senderId]++;

            // 刷新好友列表显示红点
            RefreshUserListWithBadges();
            ShowSystemMessageRequested?.Invoke($"收到离线消息 来自 {senderName}: {message}");
        }

        private void HandleOfflineMessagesComplete(string content)
        {
            try
            {
                int count = int.Parse(content);
                ShowSystemMessageRequested?.Invoke($"已接收 {count} 条离线消息");
            }
            catch { }
        }

        private void HandleFriendRequestNotification(string content)
        {
            bool needRefresh = content.Contains("|refresh");
            if (needRefresh) content = content.Replace("|refresh", "");

            if (content.StartsWith("accepted:"))
            {
                string[] parts = content.Substring("accepted:".Length).Split(':');
                if (parts.Length >= 2)
                {
                    string username = parts[1];
                    ShowSystemMessageRequested?.Invoke($"{username} 已同意您的好友请求！现在可以开始聊天了。");
                }
                LoadOnlineUsers();
            }
            else if (content.StartsWith("friend_added:"))
            {
                ShowSystemMessageRequested?.Invoke($"已添加好友！");
                LoadOnlineUsers();
            }
            else
            {
                string[] parts = content.Split(':');
                if (parts.Length >= 2)
                {
                    string fromUsername = parts[1];
                    ShowSystemMessageRequested?.Invoke($"{fromUsername} 向你发送了好友请求，请在【好友申请】中处理！");
                }
            }
        }

        private void ReceiveChatMessage(string content)
        {
            if (content.StartsWith("not_friend|"))
            {
                string errorMsg = content.Substring("not_friend|".Length);
                ShowSystemMessageRequested?.Invoke(errorMsg);
                return;
            }

            string[] parts = content.Split(new string[] { ":" }, 2, StringSplitOptions.None);
            if (parts.Length < 2) return;

            int senderId = int.Parse(parts[0]);
            string message = parts[1];

            string senderName = senderId == _client.UserId ? "我" :
                               (OnlineUsers.ContainsKey(senderId) ? OnlineUsers[senderId] : "未知用户");

            if (!ChatHistories.ContainsKey(senderId))
            {
                ChatHistories[senderId] = new List<ChatMessage>();
            }
            ChatHistories[senderId].Add(new ChatMessage { IsMine = false, SenderName = senderName, Content = message });

            // 未读+1
            if (!UnreadCounts.ContainsKey(senderId))
            {
                UnreadCounts[senderId] = 0;
            }
            UnreadCounts[senderId]++;

            if (senderId == CurrentChatUserId)
            {
                ShowChatMessageRequested?.Invoke(false, senderName, message);
                // 当前正在和这个人聊，红点不会显示
                UnreadCounts[senderId] = 0;
            }
            else
            {
                // 不在当前聊天窗口，刷新用户列表显示红点
                RefreshUserListWithBadges();
                ShowSystemMessageRequested?.Invoke($"收到来自 {senderName} 的消息");
            }
        }

        private void RefreshUserListWithBadges()
        {
            var displayItems = new List<string>();
            lock (_friendListLock)
            {
                foreach (var userId in OnlineUserIds)
                {
                    string username = OnlineUsers.ContainsKey(userId) ? OnlineUsers[userId] : "未知用户";
                    int unread = UnreadCounts.ContainsKey(userId) ? UnreadCounts[userId] : 0;
                    string displayText = unread > 0 ? $"{username} 🔴{unread}" : username;
                    displayItems.Add(displayText);
                }
            }
            UpdateUserListRequested?.Invoke(displayItems);
        }

        public async void SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            if (CurrentChatUserId == -1) return;

            try
            {
                string content = $"{CurrentChatUserId}:{message}";
                await _client.SendMessageAsync(2, content);

                ShowChatMessageRequested?.Invoke(true, "我", message);

                if (!ChatHistories.ContainsKey(CurrentChatUserId))
                {
                    ChatHistories[CurrentChatUserId] = new List<ChatMessage>();
                }
                ChatHistories[CurrentChatUserId].Add(new ChatMessage { IsMine = true, SenderName = "我", Content = message });
            }
            catch (Exception ex)
            {
                throw new Exception("发送失败：" + ex.Message);
            }
        }

        public void SelectFriend(int index)
        {
            if (_isUpdatingList) return;
            if (index < 0 || index >= OnlineUserIds.Count) return;

            int selectedFriendId = OnlineUserIds[index];

            if (UnreadCounts.ContainsKey(selectedFriendId) && UnreadCounts[selectedFriendId] > 0)
            {
                UnreadCounts[selectedFriendId] = 0;
                UpdateBadgeRequested?.Invoke(selectedFriendId, OnlineUsers[selectedFriendId]);
            }

            CurrentChatUserId = selectedFriendId;
            string targetName = OnlineUsers[CurrentChatUserId];
            Title = $"正在和 {targetName} 聊天";

            DisplayChatHistory();
        }

        public List<ChatMessage> GetCurrentChatHistory()
        {
            if (ChatHistories.ContainsKey(CurrentChatUserId))
            {
                return ChatHistories[CurrentChatUserId];
            }
            return new List<ChatMessage>();
        }

        private void DisplayChatHistory()
        {
            // 由 View 在切换时清空并重新显示
        }
    }
}
