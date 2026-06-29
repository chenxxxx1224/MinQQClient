using MinQQServer.Models;
using MinQQServer.Network;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace MinQQServer.Handlers
{
    public static class MessageDispatcher
    {
        private static readonly ChatHandler _chatHandler = new ChatHandler();
        private static readonly LoginHandler _loginHandler = new LoginHandler(_chatHandler);
        private static readonly FriendHandler _friendHandler = new FriendHandler();
        private static readonly RegisterHandler _registerHandler = new RegisterHandler();

        public static async Task Process(string jsonStr, ClientInfo clientInfo, TcpServer server)
        {
            try
            {
                var msg = JsonSerializer.Deserialize<NetMessage>(jsonStr);
                if (msg == null) return;

                Console.WriteLine($"[收到消息] 来自用户ID={clientInfo.UserId} 类型={msg.MsgType} 内容={msg.Content}");

                switch (msg.MsgType)
                {
                    case 1:  // 登录
                        await _loginHandler.Handle(msg.Content, clientInfo, server);
                        break;
                    case 2:  // 聊天消息
                        await _chatHandler.Handle(msg.Content, clientInfo, server);
                        break;
                    case 3:  // 请求在线用户列表
                        await _chatHandler.HandleOnlineUsersRequest(clientInfo, server);
                        break;
                    case 4:  // 搜索用户
                        await _friendHandler.HandleSearchUser(msg.Content, clientInfo, server);
                        break;
                    case 5:  // 发送好友请求
                        await _friendHandler.HandleFriendRequest(msg.Content, clientInfo, server);
                        break;
                    case 6:  // 获取好友申请列表
                        await _friendHandler.HandleGetFriendRequests(clientInfo, server);
                        break;
                    case 7:  // 处理好友申请
                        await _friendHandler.HandleProcessFriendRequest(msg.Content, clientInfo, server);
                        break;
                    case 10:  // 注册
                        await _registerHandler.Handle(msg.Content, clientInfo, server);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JSON解析失败]: {ex.Message}");
            }
        }
    }
}
