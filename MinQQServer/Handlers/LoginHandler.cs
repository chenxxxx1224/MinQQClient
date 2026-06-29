using System;
using System.Threading.Tasks;
using MinQQServer.Models;
using MinQQServer.Network;
using MinQQServer.Services;

namespace MinQQServer.Handlers
{
    public class LoginHandler
    {
        private readonly UserService _userService = new UserService();
        private readonly FriendService _friendService = new FriendService();
        private readonly MessageService _messageService = new MessageService();
        private readonly ChatHandler _chatHandler;

        public LoginHandler(ChatHandler chatHandler)
        {
            _chatHandler = chatHandler;
        }

        public async Task Handle(string content, ClientInfo clientInfo, TcpServer server)
        {
            string[] parts = content.Split('|');
            if (parts.Length >= 2)
            {
                string account = parts[0].Trim();
                string password = parts[1].Trim();

                var user = _userService.Login(account, password);
                if (user != null)
                {
                    clientInfo.UserId = user.UserID;
                    clientInfo.Username = user.Username;

                    await SendResponseAsync(clientInfo.Stream, 100, $"success|{user.UserID}|{user.Username}");
                    await _chatHandler.HandleOnlineUsersRequest(clientInfo, server);
                }
                else
                {
                    await SendResponseAsync(clientInfo.Stream, 101, "fail");
                }
            }
            else
            {
                await SendResponseAsync(clientInfo.Stream, 101, "fail");
            }

            await SendOfflineMessagesAsync(clientInfo);
        }

        private async Task SendOfflineMessagesAsync(ClientInfo clientInfo)
        {
            var result = _messageService.GetOfflineMessages(clientInfo.UserId);
            int count = 0;
            if (result != null && result.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in result.Rows)
                {
                    int senderId = Convert.ToInt32(row["SenderID"]);
                    string senderName = row["SenderName"].ToString();
                    string content = row["Content"].ToString();
                    string sendTime = row["SendTime"].ToString();
                    int messageId = Convert.ToInt32(row["MessageID"]);

                    string msgContent = $"{senderId}|{senderName}|{content}|{sendTime}";
                    await SendResponseAsync(clientInfo.Stream, 9, msgContent);
                    _messageService.MarkRead(messageId);
                    count++;
                }
            }
            await SendResponseAsync(clientInfo.Stream, 11, count.ToString());
        }

        private async Task SendResponseAsync(System.Net.Sockets.NetworkStream stream, int msgType, string content)
        {
            var response = new NetMessage { MsgType = msgType, Content = content };
            string json = System.Text.Json.JsonSerializer.Serialize(response);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(json + "\n");
            await stream.WriteAsync(data, 0, data.Length);
            await stream.FlushAsync();
        }
    }
}
