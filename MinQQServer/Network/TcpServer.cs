using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MinQQServer.Models;
using MinQQServer.Handlers;

namespace MinQQServer.Network
{
    public class TcpServer
    {
        private readonly int _port;
        private List<ClientInfo> _clients = new List<ClientInfo>();
        private readonly object _lock = new object();

        public TcpServer(int port = 8888) => _port = port;

        public async Task StartAsync()
        {
            var listener = new TcpListener(IPAddress.Any, _port);
            listener.Start();
            Console.WriteLine($"服务端已启动，等待客户端连接 (端口: {_port})...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("客户端已连接！");

                var clientInfo = new ClientInfo
                {
                    Client = client,
                    Stream = client.GetStream()
                };

                lock (_lock)
                {
                    _clients.Add(clientInfo);
                }

                _ = Task.Run(() => HandleClient(clientInfo));
            }
        }

        private async Task HandleClient(ClientInfo clientInfo)
        {
            var stream = clientInfo.Stream;
            byte[] buffer = new byte[4096];
            string remainingData = "";

            try
            {
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    remainingData += Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    int newlineIndex;
                    while ((newlineIndex = remainingData.IndexOf('\n')) != -1)
                    {
                        string completeJson = remainingData.Substring(0, newlineIndex).Trim();
                        remainingData = remainingData.Substring(newlineIndex + 1);

                        if (!string.IsNullOrEmpty(completeJson))
                        {
                            await MessageDispatcher.Process(completeJson, clientInfo, this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"客户端断开连接: {ex.Message}");
            }
            finally
            {
                lock (_lock)
                {
                    _clients.Remove(clientInfo);
                    Console.WriteLine($"用户 {clientInfo.Username} 已下线");
                    BroadcastOnlineUsersAsync();
                }
                clientInfo.Client.Close();
            }
        }

        public async Task SendToUserAsync(int userId, int msgType, string content)
        {
            var targetClient = _clients.FirstOrDefault(c => c.UserId == userId);
            if (targetClient != null)
            {
                await SendMessageAsync(targetClient.Stream, msgType, content);
            }
        }

        public async Task BroadcastAsync(int msgType, string content)
        {
            List<ClientInfo> clientsCopy;
            lock (_lock)
            {
                clientsCopy = _clients.ToList();
            }

            foreach (var client in clientsCopy)
            {
                try
                {
                    await SendMessageAsync(client.Stream, msgType, content);
                }
                catch { }
            }
        }

        private async Task SendMessageAsync(NetworkStream stream, int msgType, string content)
        {
            var response = new NetMessage { MsgType = msgType, Content = content };
            string json = JsonSerializer.Serialize(response);
            byte[] data = Encoding.UTF8.GetBytes(json + "\n");
            await stream.WriteAsync(data, 0, data.Length);
        }

        public string GetOnlineUsersString()
        {
            lock (_lock)
            {
                return string.Join(",", _clients.Select(c => $"{c.UserId}:{c.Username}"));
            }
        }

        public ClientInfo GetClientByUserId(int userId)
        {
            lock (_lock)
            {
                return _clients.FirstOrDefault(c => c.UserId == userId);
            }
        }

        public async Task BroadcastOnlineUsersAsync()
        {
            string users = GetOnlineUsersString();
            await BroadcastAsync(3, users);
        }
    }
}
