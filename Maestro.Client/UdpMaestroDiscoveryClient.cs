using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro.Client
{
    public class UdpMaestroDiscoveryClient
    {
        public IObservable<MaestroDeviceInfo> DiscoverAsync()
        {
            return Observable.Create<MaestroDeviceInfo>(obs =>
            {
                var cancelSource = new CancellationTokenSource();
                var client = new UdpClient();
                client.EnableBroadcast = true;
                var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, 5678);
                Task.Run(async () =>
                {
                    while (!cancelSource.Token.IsCancellationRequested)
                    {
                        var response = await client.ReceiveAsync().ConfigureAwait(false);
                        var name = Encoding.UTF8.GetString(response.Buffer);
                        var ip = response.RemoteEndPoint.Address.ToString();
                        obs.OnNext(new MaestroDeviceInfo()
                        {
                            Address = ip,
                            Name = name
                        });
                    }
                });
                client.SendAsync(new[] { (byte)1 }, 1, broadcastEndpoint);
                return () => cancelSource.Cancel();
            });
        }
    }

    public struct MaestroDeviceInfo
    {
        public string Address { get; set; }
        public string Name { get; set; }
    }
}
