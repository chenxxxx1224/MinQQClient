using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinQQClient
{
    public class Client
    {
        private TcpClient _client;
        private NetworkStream _stream;

        // 当前登录用户信息
        public int UserId { get; set; }
        public string Username { get; set; }

        // 消息回调事件（解决多个接收者竞争消息的问题）
        public event Action<NetMessage> OnMessageReceived;

        public async Task ConnectAsync(string ip = "127.0.0.1", int port = 8888)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);
            _stream = _client.GetStream();
        }

        // 发送消息的通用方法
        public async Task SendMessageAsync(int msgType, string content)
        {
            var msg = new NetMessage { MsgType = msgType, Content = content };
            string json = JsonSerializer.Serialize(msg);
            byte[] data = Encoding.UTF8.GetBytes(json + "\n");
            await _stream.WriteAsync(data, 0, data.Length);
        }

        // 触发消息事件（供 Main.cs 调用）
        public void RaiseMessageReceived(NetMessage msg)
        {
            OnMessageReceived?.Invoke(msg);
        }

        // 接收服务端返回的消息（保留但不使用，由 Main.cs 统一接收）
        public async Task<NetMessage> ReceiveMessageAsync()
        {
            byte[] buffer = new byte[4096];
            string remainingData = "";

            while (true)
            {
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) return null;

                remainingData += Encoding.UTF8.GetString(buffer, 0, bytesRead);

                int newlineIndex;
                while ((newlineIndex = remainingData.IndexOf('\n')) != -1)
                {
                    string completeJson = remainingData.Substring(0, newlineIndex).Trim();
                    remainingData = remainingData.Substring(newlineIndex + 1);

                    if (!string.IsNullOrEmpty(completeJson))
                    {
                        return JsonSerializer.Deserialize<NetMessage>(completeJson);
                    }
                }
            }
        }

        // 获取网络流，用于后台接收消息
        public NetworkStream GetStream() => _stream;

        public void Close() => _client?.Close();
    }
}
