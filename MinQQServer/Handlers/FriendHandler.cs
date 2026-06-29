using System;
using MinQQServer.Models;
using MinQQServer.Network;
using MinQQServer.Services;
using System.Threading.Tasks;

namespace MinQQServer.Handlers
{
    public class FriendHandler
    {
        private readonly FriendService _friendService = new FriendService();
        private readonly DataAccess.UserDao _userDao = new DataAccess.UserDao();

        public async Task HandleSearchUser(string content, ClientInfo clientInfo, TcpServer server)
        {
            string[] parts = content.Split('|');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int userId))
            {
                var result = _userDao.GetByAccount("", ""); // 占位 - 实际应根据ID查询
                // 改为直接查询
                string sql = $"SELECT UserID, Username FROM [User] WHERE UserID = {userId}";
                var dt = DataAccess.Dao.getData(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int foundUserId = Convert.ToInt32(dt.Rows[0]["UserID"]);
                    string foundUsername = dt.Rows[0]["Username"].ToString();
                    await SendResponseAsync(clientInfo.Stream, 4, $"success|{foundUserId}|{foundUsername}");
                }
                else
                {
                    await SendResponseAsync(clientInfo.Stream, 4, "fail|用户不存在");
                }
            }
            else
            {
                await SendResponseAsync(clientInfo.Stream, 4, "fail|参数错误");
            }
        }

        public async Task HandleFriendRequest(string content, ClientInfo clientInfo, TcpServer server)
        {
            if (!int.TryParse(content, out int toUserId))
            {
                await SendResponseAsync(clientInfo.Stream, 5, "fail|参数错误");
                return;
            }

            string status = _friendService.SendRequestStatus(clientInfo.UserId, toUserId);

            if (status == "success")
            {
                string notifyContent = $"{clientInfo.UserId}:{clientInfo.Username}";
                await server.SendToUserAsync(toUserId, 8, notifyContent);
            }

            await SendResponseAsync(clientInfo.Stream, 5, status);
        }

        public async Task HandleGetFriendRequests(ClientInfo clientInfo, TcpServer server)
        {
            string requestList = _friendService.GetPendingRequestsText(clientInfo.UserId);
            await SendResponseAsync(clientInfo.Stream, 6, requestList);
        }

        public async Task HandleProcessFriendRequest(string content, ClientInfo clientInfo, TcpServer server)
        {
            string[] parts = content.Split(':');
            if (parts.Length < 2)
            {
                await SendResponseAsync(clientInfo.Stream, 7, "fail|参数错误");
                return;
            }

            if (!int.TryParse(parts[0], out int requestId))
            {
                await SendResponseAsync(clientInfo.Stream, 7, "fail|参数错误");
                return;
            }

            string action = parts[1].ToLower();
            var (ok, message, fromUserId) = _friendService.ProcessRequest(requestId, clientInfo.UserId, action);

            if (ok)
            {
                if (action == "accept")
                {
                    await server.SendToUserAsync(fromUserId, 8, $"accepted:{clientInfo.UserId}:{clientInfo.Username}|refresh");
                    await server.SendToUserAsync(clientInfo.UserId, 8, $"friend_added:{fromUserId}|refresh");
                }
            }

            await SendResponseAsync(clientInfo.Stream, 7, message);
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
