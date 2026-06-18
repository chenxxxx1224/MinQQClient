using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Net.Sockets;
using MinQQServer;

public class NetMessage
{
    public int MsgType { get; set; }
    public string Content { get; set; }
}

public static class MessageHandler
{
    public static async Task Process(string jsonStr, ClientInfo clientInfo, TcpServer server)
    {
        try
        {
            var msg = JsonSerializer.Deserialize<NetMessage>(jsonStr);
            if (msg == null) return;

            switch (msg.MsgType)
            {
                case 1:  // 登录
                    await HandleLogin(msg.Content, clientInfo, server);
                    break;

                case 2:  // 聊天消息
                    await HandleChat(msg.Content, clientInfo, server);
                    break;

                case 3:  // 请求在线用户列表
                    await HandleOnlineUsersRequest(clientInfo, server);
                    break;

                case 4:  // 搜索用户
                    await HandleSearchUser(msg.Content, clientInfo, server);
                    break;

                case 5:  // 发送好友请求
                    await HandleFriendRequest(msg.Content, clientInfo, server);
                    break;

                case 6:  // 获取好友申请列表
                    await HandleGetFriendRequests(clientInfo, server);
                    break;

                case 7:  // 处理好友申请（同意/拒绝）
                    await HandleProcessFriendRequest(msg.Content, clientInfo, server);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[JSON解析失败]: {ex.Message}");
        }
    }

    // 处理登录
    private static async Task HandleLogin(string content, ClientInfo clientInfo, TcpServer server)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[收到登录请求] 账号信息: {content}");
        Console.ResetColor();

        string[] parts = content.Split('|');
        if (parts.Length >= 2)
        {
            string account = parts[0].Trim();
            string password = parts[1].Trim();

            Console.WriteLine($"准备验证账号: {account}, 密码: {password}");

            // 查询数据库获取用户信息
            string sql = $"SELECT UserID, Username FROM [User] WHERE Username = '{account}' AND Password = '{password}'";
            var result = Dao.getData(sql);

            if (result != null && result.Rows.Count > 0)
            {
                int userId = Convert.ToInt32(result.Rows[0]["UserID"]);
                string username = result.Rows[0]["Username"].ToString();

                // 保存用户信息到客户端连接
                clientInfo.UserId = userId;
                clientInfo.Username = username;

                Console.WriteLine($"用户 {username} (ID: {userId}) 登录成功！");

                // 返回登录成功（格式：success|userId|username）
                await SendResponseAsync(clientInfo.Stream, 100, $"success|{userId}|{username}");

                // 登录成功后，发送好友列表给新用户
                await HandleOnlineUsersRequest(clientInfo, server);

                // 不再广播所有在线用户（因为只有好友才能通信）
                // await server.BroadcastOnlineUsersAsync();
            }
            else
            {
                Console.WriteLine("账号或密码错误！");
                await SendResponseAsync(clientInfo.Stream, 101, "fail");
            }
        }
        else
        {
            Console.WriteLine("登录数据格式错误！");
            await SendResponseAsync(clientInfo.Stream, 101, "fail");
        }
    }

    // 处理聊天消息
    private static async Task HandleChat(string content, ClientInfo clientInfo, TcpServer server)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[收到聊天消息] {clientInfo.Username}: {content}");
        Console.ResetColor();

        // 格式：receiverId:message
        string[] parts = content.Split(new string[] { ":" }, 2, StringSplitOptions.None);
        if (parts.Length >= 2)
        {
            int receiverId = int.Parse(parts[0]);
            string message = parts[1];

            // 检查是否是好友
            string checkFriendSql = $@"
                SELECT COUNT(*) FROM [Friend] 
                WHERE (UserID = {clientInfo.UserId} AND FriendUserID = {receiverId})
                   OR (UserID = {receiverId} AND FriendUserID = {clientInfo.UserId})";
            var friendResult = Dao.getScalar(checkFriendSql);

            if (friendResult == null || Convert.ToInt32(friendResult) == 0)
            {
                Console.WriteLine($"[聊天消息] {clientInfo.Username} 尝试向非好友 {receiverId} 发送消息，已拒绝");
                await SendResponseAsync(clientInfo.Stream, 2, "not_friend|你们还不是好友，无法发送消息");
                return;
            }

            // 保存消息到数据库
            string sql = $"INSERT INTO [Message] (SenderID, ReceiverID, Content, SendTime, IsRead) VALUES ({clientInfo.UserId}, {receiverId}, '{message}', GETDATE(), 0)";
            Dao.CUD(sql);

            // 转发消息给目标用户（格式：senderId:message）
            string forwardContent = $"{clientInfo.UserId}:{message}";
            await server.SendToUserAsync(receiverId, 2, forwardContent);

            Console.WriteLine($"消息已转发给用户 {receiverId}");
        }
    }

    // 处理在线用户列表请求（只返回好友）
    private static async Task HandleOnlineUsersRequest(ClientInfo clientInfo, TcpServer server)
    {
        Console.WriteLine($"用户 {clientInfo.Username} 请求在线好友列表");

        // 查询该用户的所有好友
        string sql = $@"
            SELECT u.UserID, u.Username 
            FROM [Friend] f
            INNER JOIN [User] u ON f.FriendUserID = u.UserID
            WHERE f.UserID = {clientInfo.UserId}
            UNION
            SELECT u.UserID, u.Username 
            FROM [Friend] f
            INNER JOIN [User] u ON f.UserID = u.UserID
            WHERE f.FriendUserID = {clientInfo.UserId}";

        var result = Dao.getData(sql);
        var friends = new List<string>();

        if (result != null)
        {
            foreach (System.Data.DataRow row in result.Rows)
            {
                int friendId = Convert.ToInt32(row["UserID"]);
                string friendName = row["Username"].ToString();
                friends.Add($"{friendId}:{friendName}");
            }
        }

        string onlineFriends = string.Join(",", friends);
        await SendResponseAsync(clientInfo.Stream, 3, onlineFriends);
    }

    // 搜索用户
    private static async Task HandleSearchUser(string content, ClientInfo clientInfo, TcpServer server)
    {
        Console.WriteLine($"用户 {clientInfo.Username} 搜索用户: {content}");

        // 格式：search|userId
        string[] parts = content.Split('|');
        if (parts.Length >= 2 && int.TryParse(parts[1], out int userId))
        {
            string sql = $"SELECT UserID, Username FROM [User] WHERE UserID = {userId}";
            var result = Dao.getData(sql);

            if (result != null && result.Rows.Count > 0)
            {
                int foundUserId = Convert.ToInt32(result.Rows[0]["UserID"]);
                string foundUsername = result.Rows[0]["Username"].ToString();
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

    // 发送好友请求
    private static async Task HandleFriendRequest(string content, ClientInfo clientInfo, TcpServer server)
    {
        Console.WriteLine($"用户 {clientInfo.Username} 发送好友请求给: {content}");

        if (!int.TryParse(content, out int toUserId))
        {
            await SendResponseAsync(clientInfo.Stream, 5, "fail|参数错误");
            return;
        }

        // 检查是否已经是好友
        string checkFriendSql = $@"
            SELECT COUNT(*) FROM [Friend] 
            WHERE (UserID = {clientInfo.UserId} AND FriendUserID = {toUserId})
               OR (UserID = {toUserId} AND FriendUserID = {clientInfo.UserId})";
        var friendResult = Dao.getScalar(checkFriendSql);
        if (friendResult != null && Convert.ToInt32(friendResult) > 0)
        {
            await SendResponseAsync(clientInfo.Stream, 5, "already_friends");
            return;
        }

        // 检查是否已经发送过请求（待处理）
        string checkRequestSql = $@"
            SELECT COUNT(*) FROM [FriendRequest] 
            WHERE FromUserID = {clientInfo.UserId} AND ToUserID = {toUserId} AND Status = 0";
        var requestResult = Dao.getScalar(checkRequestSql);
        if (requestResult != null && Convert.ToInt32(requestResult) > 0)
        {
            await SendResponseAsync(clientInfo.Stream, 5, "already_sent");
            return;
        }

        // 插入好友请求
        string insertSql = $@"
            INSERT INTO [FriendRequest] (FromUserID, ToUserID, Status, RequestTime) 
            VALUES ({clientInfo.UserId}, {toUserId}, 0, GETDATE())";
        Dao.CUD(insertSql);

        // 通知对方（如果在线）
        string notifyContent = $"{clientInfo.UserId}:{clientInfo.Username}";
        await server.SendToUserAsync(toUserId, 8, notifyContent);

        await SendResponseAsync(clientInfo.Stream, 5, "success");
    }

    // 获取好友申请列表
    private static async Task HandleGetFriendRequests(ClientInfo clientInfo, TcpServer server)
    {
        Console.WriteLine($"用户 {clientInfo.Username} 请求好友申请列表");

        string sql = $@"
            SELECT fr.RequestID, fr.FromUserID, u.Username 
            FROM [FriendRequest] fr
            INNER JOIN [User] u ON fr.FromUserID = u.UserID
            WHERE fr.ToUserID = {clientInfo.UserId} AND fr.Status = 0
            ORDER BY fr.RequestTime DESC";

        var result = Dao.getData(sql);
        var requests = new List<string>();

        if (result != null)
        {
            foreach (System.Data.DataRow row in result.Rows)
            {
                int requestId = Convert.ToInt32(row["RequestID"]);
                int fromUserId = Convert.ToInt32(row["FromUserID"]);
                string fromUsername = row["Username"].ToString();
                requests.Add($"{requestId}:{fromUserId}:{fromUsername}");
            }
        }

        string requestList = string.Join(",", requests);
        await SendResponseAsync(clientInfo.Stream, 6, requestList);
    }

    // 处理好友申请（同意/拒绝）
    private static async Task  HandleProcessFriendRequest(string content, ClientInfo clientInfo, TcpServer server)
    {
        Console.WriteLine($"用户 {clientInfo.Username} 处理好友申请: {content}");

        // 格式：requestId:accept/reject
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

        // 获取请求信息
        string getRequestSql = $"SELECT FromUserID, ToUserID FROM [FriendRequest] WHERE RequestID = {requestId} AND Status = 0";
        var requestResult = Dao.getData(getRequestSql);

        if (requestResult == null || requestResult.Rows.Count == 0)
        {
            await SendResponseAsync(clientInfo.Stream, 7, "fail|请求不存在或已处理");
            return;
        }

        int fromUserId = Convert.ToInt32(requestResult.Rows[0]["FromUserID"]);
        int toUserId = Convert.ToInt32(requestResult.Rows[0]["ToUserID"]);

        // 确保是发给当前用户的
        if (toUserId != clientInfo.UserId)
        {
            await SendResponseAsync(clientInfo.Stream, 7, "fail|无权操作");
            return;
        }

        if (action == "accept")
        {
            // 同意：添加到好友表
            string insertFriendSql = $@"
                INSERT INTO [Friend] (UserID, FriendUserID, AddTime) 
                VALUES ({fromUserId}, {toUserId}, GETDATE())";
            Dao.CUD(insertFriendSql);

            // 更新请求状态
            string updateSql = $"UPDATE [FriendRequest] SET Status = 1 WHERE RequestID = {requestId}";
            Dao.CUD(updateSql);

            Console.WriteLine($"用户 {clientInfo.Username} 同意了 {fromUserId} 的好友请求");

            // 通知对方：已同意，并请求对方刷新好友列表
            await server.SendToUserAsync(fromUserId, 8, $"accepted:{clientInfo.UserId}:{clientInfo.Username}|refresh");

            // 同时通知同意方（张三）刷新自己的好友列表
            await server.SendToUserAsync(toUserId, 8, $"friend_added:{fromUserId}|refresh");
        }
        else if (action == "reject")
        {
            // 拒绝：更新请求状态
            string updateSql = $"UPDATE [FriendRequest] SET Status = 2 WHERE RequestID = {requestId}";
            Dao.CUD(updateSql);

            Console.WriteLine($"用户 {clientInfo.Username} 拒绝了 {fromUserId} 的好友请求");
        }

        await SendResponseAsync(clientInfo.Stream, 7, "success");
    }

    // 发送响应消息
    private static async Task SendResponseAsync(NetworkStream stream, int msgType, string content)
    {
        var response = new NetMessage { MsgType = msgType, Content = content };
        string json = JsonSerializer.Serialize(response);
        byte[] data = Encoding.UTF8.GetBytes(json + "\n");
        await stream.WriteAsync(data, 0, data.Length);
    }
}
