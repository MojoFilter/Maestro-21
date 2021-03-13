using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro.Server
{
    public class UdpMaestroDiscoveryServer
    {
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            var maestroId = Encoding.UTF8.GetBytes(Environment.MachineName);
            var server = new UdpClient(5678);
            while (!cancellationToken.IsCancellationRequested)
            {
                var req = await server.ReceiveAsync().ConfigureAwait(false);
                await server.SendAsync(maestroId, maestroId.Length, req.RemoteEndPoint);
            }
        }
    }
}
