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

                // 广播在线用户列表给所有用户
                await server.BroadcastOnlineUsersAsync();
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

            // 保存消息到数据库
            string sql = $"INSERT INTO [Message] (SenderID, ReceiverID, Content, SendTime, IsRead) VALUES ({clientInfo.UserId}, {receiverId}, '{message}', GETDATE(), 0)";
            Dao.CUD(sql);

            // 转发消息给目标用户（格式：senderId:message）
            string forwardContent = $"{clientInfo.UserId}:{message}";
            await server.SendToUserAsync(receiverId, 2, forwardContent);

            Console.WriteLine($"消息已转发给用户 {receiverId}");
        }
    }

    // 处理在线用户列表请求
    private static async Task HandleOnlineUsersRequest(ClientInfo clientInfo, TcpServer server)
    {
        Console.WriteLine($"用户 {clientInfo.Username} 请求在线用户列表");

        string onlineUsers = server.GetOnlineUsersString();
        await SendResponseAsync(clientInfo.Stream, 3, onlineUsers);
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
