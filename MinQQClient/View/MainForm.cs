using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MinQQClient.HelperClass;
using MinQQClient.Model;
using MinQQClient.ViewModel;

namespace MinQQClient.View
{
    public partial class MainForm : Sunny.UI.UIForm
    {
        private readonly TcpClientHelper _client;
        private MainViewModel _viewModel;
        private bool _isUpdatingList = false;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(TcpClientHelper client)
        {
            InitializeComponent();

            _client = client;
            _viewModel = new MainViewModel(client);

            this.Text = $"MinQQ - {client.Username}";

            BindViewModel();
            BindEvents();

            this.Load += async (s, e) =>
            {
                await _viewModel.StartReceiveLoop();
                _viewModel.LoadOnlineUsers();
            };
        }

        private void BindViewModel()
        {
            lblTitle.DataBindings.Clear();
            lblTitle.DataBindings.Add("Text", _viewModel, nameof(MainViewModel.Title));
        }

        private void BindEvents()
        {
            _viewModel.ShowSystemMessageRequested += msg => SafeInvoke(() => AppendSystemMessage(msg));
            _viewModel.ShowChatMessageRequested += (isMine, name, content) => SafeInvoke(() => AppendChatMessage(isMine, name, content));
            _viewModel.UpdateUserListRequested += items => SafeInvoke(() => UpdateUserList(items));
            _viewModel.UpdateBadgeRequested += (userId, name) => SafeInvoke(() => UpdateBadgeForUser(userId, name));
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.Title))
                {
                    SafeInvoke(() => lblTitle.Text = _viewModel.Title);
                }
            };
        }

        private void SafeInvoke(Action action)
        {
            if (InvokeRequired)
            {
                BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        private void AppendSystemMessage(string message)
        {
            rtbChatHistory.SelectionColor = Color.Gray;
            rtbChatHistory.AppendText($"[系统] {DateTime.Now:HH:mm:ss} {message}\n");
            rtbChatHistory.SelectionColor = rtbChatHistory.ForeColor;
        }

        private void AppendChatMessage(bool isMine, string senderName, string content)
        {
            if (isMine)
            {
                rtbChatHistory.SelectionColor = Color.Blue;
                rtbChatHistory.SelectionAlignment = HorizontalAlignment.Right;
                rtbChatHistory.AppendText($"[我] {DateTime.Now:HH:mm:ss}\n");
                rtbChatHistory.AppendText($"{content}\n\n");
                rtbChatHistory.SelectionAlignment = HorizontalAlignment.Left;
            }
            else
            {
                rtbChatHistory.SelectionColor = Color.Green;
                rtbChatHistory.AppendText($"[{senderName}] {DateTime.Now:HH:mm:ss}\n");
                rtbChatHistory.AppendText($"{content}\n\n");
            }
            rtbChatHistory.SelectionColor = rtbChatHistory.ForeColor;
        }

        private void UpdateUserList(List<string> displayItems)
        {
            _isUpdatingList = true;
            int prevIndex = lstOnlineUsers.SelectedIndex;
            lstOnlineUsers.Items.Clear();
            foreach (var item in displayItems)
            {
                lstOnlineUsers.Items.Add(item);
            }
            if (prevIndex >= 0 && prevIndex < lstOnlineUsers.Items.Count)
            {
                lstOnlineUsers.SelectedIndex = prevIndex;
            }
            else if (lstOnlineUsers.Items.Count > 0)
            {
                lstOnlineUsers.SelectedIndex = 0;
                _viewModel.SelectFriend(0);
            }
            _isUpdatingList = false;
        }

        private void UpdateBadgeForUser(int userId, string username)
        {
            int idx = _viewModel.OnlineUserIds.IndexOf(userId);
            if (idx < 0) return;
            int unread = _viewModel.UnreadCounts.ContainsKey(userId) ? _viewModel.UnreadCounts[userId] : 0;
            string text = unread > 0 ? $"{username} 🔴{unread}" : username;
            lstOnlineUsers.Items[idx] = text;
        }

        private void btnSend_Click_1(object sender, EventArgs e)
        {
            string message = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("请输入消息内容！", "提示");
                return;
            }

            try
            {
                _viewModel.SendMessage(message);
                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var addForm = new AddFriendForm(_client))
            {
                addForm.ShowDialog();
            }
            _viewModel.LoadOnlineUsers();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            using (var reqForm = new FriendRequestForm(_client))
            {
                reqForm.ShowDialog();
            }
            _viewModel.LoadOnlineUsers();
        }

        private void lstOnlineUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingList) return;
            int index = lstOnlineUsers.SelectedIndex;
            if (index < 0) return;

            _viewModel.SelectFriend(index);

            // 切换聊天对象后清空并重载历史
            rtbChatHistory.Clear();
            var history = _viewModel.GetCurrentChatHistory();
            foreach (var msg in history)
            {
                AppendChatMessage(msg.IsMine, msg.SenderName, msg.Content);
            }
        }
    }
}
