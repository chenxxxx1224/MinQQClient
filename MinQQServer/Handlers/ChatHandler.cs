using System;
using MinQQServer.Models;
using MinQQServer.Network;
using MinQQServer.Services;
using System.Threading.Tasks;

namespace MinQQServer.Handlers
{
    public class ChatHandler
    {
        private readonly MessageService _messageService = new MessageService();
        private readonly FriendService _friendService = new FriendService();

        public async Task Handle(string content, ClientInfo clientInfo, TcpServer server)
        {
            string[] parts = content.Split(new string[] { ":" }, 2, System.StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                int receiverId = int.Parse(parts[0]);
                string message = parts[1];

                bool areFriends = AreFriends(clientInfo.UserId, receiverId);
                if (!areFriends)
                {
                    await SendResponseAsync(clientInfo.Stream, 2, "not_friend|你们还不是好友，无法发送消息");
                    return;
                }

                _messageService.Save(clientInfo.UserId, receiverId, message);

                string forwardContent = $"{clientInfo.UserId}:{message}";
                await server.SendToUserAsync(receiverId, 2, forwardContent);
            }
        }

        public async Task HandleOnlineUsersRequest(ClientInfo clientInfo, TcpServer server)
        {
            string onlineFriends = _friendService.GetFriendListText(clientInfo.UserId);
            await SendResponseAsync(clientInfo.Stream, 3, onlineFriends);
        }

        private bool AreFriends(int userA, int userB)
        {
            // 通过 FriendService 间接调用 FriendDao
            // 此处直接调用 Dao 以避免循环依赖
            string checkFriendSql = $@"
                SELECT COUNT(*) FROM [Friend] 
                WHERE (UserID = {userA} AND FriendUserID = {userB})
                   OR (UserID = {userB} AND FriendUserID = {userA})";
            var result = DataAccess.Dao.getScalar(checkFriendSql);
            return result != null && Convert.ToInt32(result) > 0;
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
