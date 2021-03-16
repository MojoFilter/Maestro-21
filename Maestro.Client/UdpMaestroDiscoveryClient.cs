using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;

namespace Maestro.Client
{
    public class UdpMaestroDiscoveryClient : IMaestroDiscoveryClient
    {
        public IObservable<MaestroDeviceInfo> DiscoverAsync()
        {
            return Observable.Create<MaestroDeviceInfo>(obs =>
            {
                var cancelSource = new CancellationTokenSource();
                this.ListenForAnnouncements(obs, cancelSource.Token);
                this.SayWhoDere(obs);
                return () => cancelSource.Cancel();
            });
        }

        private async void SayWhoDere(IObserver<MaestroDeviceInfo> obs)
        {
            try
            {
                var client = new UdpClient();
                client.EnableBroadcast = true;
                var ep = new IPEndPoint(IPAddress.Broadcast, 5678);
                Console.WriteLine($"Sending holla");
                await client.SendAsync(new[] { (byte)1 }, 1, ep).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                obs.OnError(ex);
            }
        }

        private async void ListenForAnnouncements(IObserver<MaestroDeviceInfo> obs, CancellationToken cancellationToken)
        {
            try
            {
                var client = new UdpClient(5679);
                client.EnableBroadcast = true;
                while (!cancellationToken.IsCancellationRequested) {
                    var announcement = await client.ReceiveAsync().ConfigureAwait(false);
                    Console.WriteLine($"Got a holla back");
                    var name = Encoding.UTF8.GetString(announcement.Buffer);
                    Console.WriteLine($"From {name}");
                    obs.OnNext(new MaestroDeviceInfo()
                    {
                        Address = announcement.RemoteEndPoint.Address.ToString(),
                        Name = name
                    });
                }
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    obs.OnError(ex);
                }
            }
        }
    }


}
