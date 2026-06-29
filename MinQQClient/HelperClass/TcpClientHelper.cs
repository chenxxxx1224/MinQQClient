using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MinQQClient.Model;

namespace MinQQClient.HelperClass
{
    public class TcpClientHelper
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public int UserId { get; set; }
        public string Username { get; set; }

        public event Action<NetMessage> OnMessageReceived;

        public async Task ConnectAsync(string ip = "127.0.0.1", int port = 8888)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);
            _stream = _client.GetStream();
        }

        public async Task SendMessageAsync(int msgType, string content)
        {
            var msg = new NetMessage { MsgType = msgType, Content = content };
            string json = JsonSerializer.Serialize(msg);
            byte[] data = Encoding.UTF8.GetBytes(json + "\n");
            await _stream.WriteAsync(data, 0, data.Length);
        }

        public void RaiseMessageReceived(NetMessage msg)
        {
            OnMessageReceived?.Invoke(msg);
        }

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

        public NetworkStream GetStream() => _stream;

        public void Close() => _client?.Close();
    }
}
