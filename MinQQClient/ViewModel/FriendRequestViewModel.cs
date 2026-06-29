using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using MinQQClient.HelperClass;
using MinQQClient.Model;

namespace MinQQClient.ViewModel
{
    public class FriendRequestViewModel : ViewModelBase
    {
        private readonly TcpClientHelper _client;
        private TaskCompletionSource<NetMessage> _loadTcs;
        private TaskCompletionSource<NetMessage> _processTcs;
        private System.Threading.Timer _refreshTimer;

        public List<FriendRequestInfo> Requests { get; private set; } = new List<FriendRequestInfo>();

        public FriendRequestViewModel(TcpClientHelper client)
        {
            _client = client;
            _client.OnMessageReceived += OnServerMessage;
        }

        public void StartAutoRefresh(System.Action refreshAction)
        {
            _refreshTimer = new System.Threading.Timer(_ => refreshAction?.Invoke(), null, 5000, 5000);
        }

        private void OnServerMessage(NetMessage msg)
        {
            if (msg.MsgType == 6)
            {
                var tcs = _loadTcs;
                _loadTcs = null;
                tcs?.TrySetResult(msg);
            }
            else if (msg.MsgType == 7)
            {
                var tcs = _processTcs;
                _processTcs = null;
                tcs?.TrySetResult(msg);
            }
        }

        public async Task<string> LoadRequestsAsync()
        {
            try
            {
                _loadTcs = new TaskCompletionSource<NetMessage>();
                await _client.SendMessageAsync(6, "");

                var completed = await Task.WhenAny(_loadTcs.Task, Task.Delay(5000));
                if (completed == _loadTcs.Task)
                {
                    var resp = await _loadTcs.Task;
                    return resp.Content;
                }
            }
            catch
            {
                // 忽略错误
            }
            return null;
        }

        public List<FriendRequestInfo> ParseRequests(string content)
        {
            var list = new List<FriendRequestInfo>();
            if (!string.IsNullOrEmpty(content))
            {
                string[] items = content.Split(',');
                foreach (string item in items)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    string[] parts = item.Split(':');
                    if (parts.Length >= 3)
                    {
                        list.Add(new FriendRequestInfo
                        {
                            RequestId = int.Parse(parts[0]),
                            FromUserId = int.Parse(parts[1]),
                            FromUsername = parts[2]
                        });
                    }
                }
            }
            Requests = list;
            return list;
        }

        public async Task<string> ProcessRequestAsync(FriendRequestInfo request, string action)
        {
            try
            {
                _processTcs = new TaskCompletionSource<NetMessage>();
                await _client.SendMessageAsync(7, $"{request.RequestId}:{action}");

                var completed = await Task.WhenAny(_processTcs.Task, Task.Delay(5000));
                if (completed == _processTcs.Task)
                {
                    var resp = await _processTcs.Task;
                    return resp.Content;
                }
            }
            catch (System.Exception ex)
            {
                return "fail|" + ex.Message;
            }
            return null;
        }

        public void Cleanup()
        {
            _client.OnMessageReceived -= OnServerMessage;
            _refreshTimer?.Dispose();
        }
    }
}
