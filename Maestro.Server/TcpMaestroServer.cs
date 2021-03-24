using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro.Server
{

    public class TcpMaestroServer : IMaestroServer
    {
        public TcpMaestroServer(IPAddress localAddress, int port, IMaestroController controller)
        {
            _controller = controller;
            _localAddress = localAddress;
            _port = port;
        }

        public IObservable<string> Status => _status.AsObservable();

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _controller.InitAsync(cancellationToken).ConfigureAwait(false);
            var server = new TcpListener(_localAddress, _port);
            using (cancellationToken.Register(() => server.Stop()))
            {
                try
                {
                    server.Start();
                    this.Log($"Listening at {_localAddress}:{_port}");
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            this.ReadClient(await server.AcceptTcpClientAsync(), cancellationToken);
                        }
                        catch (IOException) { }
                    }
                }
                catch
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    throw;
                }
            }
        }


        private async void ReadClient(TcpClient client, CancellationToken cancellationToken)
        {
            try
            {
                var stream = client.GetStream();
                var buffer = new byte[2];
                while (client.Connected  && !cancellationToken.IsCancellationRequested)
                {
                    var readCount = await stream.ReadAsync(buffer, 0, 2);
                    switch ((Commands)buffer[0])
                    {
                        case Commands.Wake:
                            _controller.Wake();
                            await this.SendStatusAsync(stream).ConfigureAwait(false);
                            break;
                        case Commands.Sleep:
                            _controller.Sleep();
                            await this.SendStatusAsync(stream).ConfigureAwait(false);
                            break;
                        case Commands.GetStatus:
                            await this.SendStatusAsync(stream).ConfigureAwait(false);
                            break;
                        case Commands.GetFade:
                            await this.SendFadeAsync(stream).ConfigureAwait(false);
                            break;
                        case Commands.SetFade:
                            _controller.SetFade(buffer[1] / 255.0);
                            await this.SendFadeAsync(stream).ConfigureAwait(false);
                            break;
                        case Commands.Tap:
                            _controller.Tap();
                            break;
                    }
                }
            }
            catch (IOException) { }
        }

        private async Task SendFadeAsync(Stream stream)
        {
            var fade = (byte)(_controller.GetFade() * 255.0);
            await this.SendMessageAsync(stream, Commands.UpdateFade, fade).ConfigureAwait(false);
        }

        private async Task SendStatusAsync(Stream stream)
        {
            await this.SendMessageAsync(stream, Commands.UpdateState, (byte)(_controller.IsAwake() ? 0 : 1)).ConfigureAwait(false);
        }


        private async Task SendMessageAsync(Stream stream, Commands cmd, byte argument)
        {
            await stream.WriteAsync(new[] { (byte)cmd, argument }, 0, 2).ConfigureAwait(false);
        }

        private void Log(string message)
        {
            _status.OnNext(message);
        }

        private readonly ReplaySubject<string> _status = new ReplaySubject<string>(50);
        private IMaestroController _controller;
        private readonly IPAddress _localAddress;
        private readonly int _port;
        private const int CommandMessageLength = 2;
    }
}
