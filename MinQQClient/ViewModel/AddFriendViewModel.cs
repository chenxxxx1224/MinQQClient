using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MinQQClient.HelperClass;
using MinQQClient.Model;

namespace MinQQClient.ViewModel
{
    public class AddFriendViewModel : ViewModelBase
    {
        private readonly TcpClientHelper _client;
        private TaskCompletionSource<NetMessage> _searchTcs;
        private TaskCompletionSource<NetMessage> _addTcs;

        public int? SearchResultUserId { get; private set; }
        public string SearchResultUsername { get; private set; }

        public AddFriendViewModel(TcpClientHelper client)
        {
            _client = client;
            _client.OnMessageReceived += OnServerMessage;
        }

        private void OnServerMessage(NetMessage msg)
        {
            if (msg == null) return;
            if (msg.MsgType == 4)
            {
                var tcs = _searchTcs;
                _searchTcs = null;
                tcs?.TrySetResult(msg);
            }
            else if (msg.MsgType == 5)
            {
                var tcs = _addTcs;
                _addTcs = null;
                tcs?.TrySetResult(msg);
            }
        }

        public async Task SearchAsync(int userId, System.Action<string> updateResult)
        {
            if (userId == _client.UserId)
            {
                ShowMessageBox("不能添加自己为好友！", "提示");
                return;
            }

            try
            {
                _searchTcs = new TaskCompletionSource<NetMessage>();
                await _client.SendMessageAsync(4, $"search|{userId}").ConfigureAwait(false);

                var completed = await Task.WhenAny(_searchTcs.Task, Task.Delay(5000)).ConfigureAwait(false);
                if (completed != _searchTcs.Task)
                {
                    InvokeOnUI(() => ShowMessageBox("搜索超时，请重试！", "提示"));
                    return;
                }

                var resp = await _searchTcs.Task.ConfigureAwait(false);
                string resultText;
                if (resp != null && !string.IsNullOrEmpty(resp.Content))
                {
                    string[] parts = resp.Content.Split(new[] { '|' }, 3);
                    if (parts.Length >= 3 && parts[0] == "success")
                    {
                        SearchResultUserId = int.Parse(parts[1]);
                        SearchResultUsername = parts[2];
                        resultText = $"用户ID：{SearchResultUserId}\r\n用户名：{SearchResultUsername}\r\n\r\n点击【添加】发送好友请求";
                    }
                    else
                    {
                        SearchResultUserId = null;
                        SearchResultUsername = null;
                        resultText = "未找到该用户";
                    }
                }
                else
                {
                    SearchResultUserId = null;
                    SearchResultUsername = null;
                    resultText = "服务器响应异常";
                }

                InvokeOnUI(() => updateResult?.Invoke(resultText));
            }
            catch (System.Exception ex)
            {
                InvokeOnUI(() => ShowMessageBox("搜索失败：" + ex.Message, "错误"));
            }
        }

        public async Task AddAsync(System.Action closeForm)
        {
            if (!SearchResultUserId.HasValue)
            {
                ShowMessageBox("请先搜索用户！", "提示");
                return;
            }

            try
            {
                _addTcs = new TaskCompletionSource<NetMessage>();
                await _client.SendMessageAsync(5, SearchResultUserId.Value.ToString()).ConfigureAwait(false);

                var completed = await Task.WhenAny(_addTcs.Task, Task.Delay(5000)).ConfigureAwait(false);
                if (completed != _addTcs.Task)
                {
                    InvokeOnUI(() => ShowMessageBox("操作超时，请重试！", "提示"));
                    return;
                }

                var resp = await _addTcs.Task.ConfigureAwait(false);
                string content = resp?.Content ?? "";
                if (content == "success")
                {
                    InvokeOnUI(() =>
                    {
                        MessageBox.Show("好友请求已发送！", "提示");
                        closeForm?.Invoke();
                    });
                }
                else if (content == "already_friends")
                {
                    InvokeOnUI(() => MessageBox.Show("该用户已经是您的好友！", "提示"));
                }
                else if (content == "already_sent")
                {
                    InvokeOnUI(() => MessageBox.Show("您已发送过好友请求，请等待对方处理！", "提示"));
                }
                else
                {
                    InvokeOnUI(() => MessageBox.Show("发送失败：" + content, "提示"));
                }
            }
            catch (System.Exception ex)
            {
                InvokeOnUI(() => MessageBox.Show("添加失败：" + ex.Message, "错误"));
            }
        }

        private void InvokeOnUI(Action action)
        {
            if (action == null) return;
            if (SynchronizationContext.Current != null)
            {
                action();
                return;
            }
            var form = Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null;
            if (form != null && form.InvokeRequired)
            {
                form.BeginInvoke(action);
            }
            else if (form != null)
            {
                form.BeginInvoke(action);
            }
        }

        private void ShowMessageBox(string text, string caption)
        {
            InvokeOnUI(() => MessageBox.Show(text, caption));
        }

        public void Cleanup()
        {
            _client.OnMessageReceived -= OnServerMessage;
        }
    }
}
