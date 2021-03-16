using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro.Server
{
    public class UdpMaestroDiscoveryServer : IMaestroDiscoveryServer
    {
        public async void Start(CancellationToken cancellationToken = default)
        {
            var server = new UdpClient(5678);
            try
            {
                await this.SendAnnouncementAsync().ConfigureAwait(false);
                while (!cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Waiting for a holla");
                    var req = await server.ReceiveAsync().ConfigureAwait(false);
                    Console.WriteLine("Heard a holla");
                    await this.SendAnnouncementAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task SendAnnouncementAsync()
        {
            var maestroId = Encoding.UTF8.GetBytes(Environment.MachineName);
            var client = new UdpClient();
            client.EnableBroadcast = true;
            var ep = new IPEndPoint(IPAddress.Broadcast, 5679);
            Console.WriteLine("Sending holla back");
            await client.SendAsync(maestroId, maestroId.Length, ep).ConfigureAwait(false);
        }
    }
}
