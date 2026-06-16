using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinQQServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TcpServer tcpServer = new TcpServer();
            tcpServer.StartAsync().Wait();
        }
    }
}
