using System;
using MinQQServer.Models;
using MinQQServer.Network;
using MinQQServer.Services;
using System.Threading.Tasks;

namespace MinQQServer.Handlers
{
    public class RegisterHandler
    {
        private readonly UserService _userService = new UserService();

        public async Task Handle(string content, ClientInfo clientInfo, TcpServer server)
        {
            string[] parts = content.Split('|');
            if (parts.Length >= 2)
            {
                string account = parts[0].Trim();
                string password = parts[1].Trim();

                var (success, message, _) = _userService.Register(account, password);
                await SendResponseAsync(clientInfo.Stream, 10, message);
            }
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
