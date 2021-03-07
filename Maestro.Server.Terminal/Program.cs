using System;
using System.Net;
using System.Threading.Tasks;

namespace Maestro.Server.Terminal
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new TcpMaestroServer();
            await server.Start(IPAddress.Any, 4321, default);
        }
    }
}
