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

    public class TcpMaestroServer
    {
        public TcpMaestroServer(IMaestroController controller)
        {
            _controller = controller;
        }

        public IObservable<string> Status => _status.AsObservable();

        public async Task Start(IPAddress localAddress, int port, CancellationToken cancellationToken)
        {
            await _controller.InitAsync(cancellationToken).ConfigureAwait(false);
            var server = new TcpListener(localAddress, port);
            using (cancellationToken.Register(() => server.Stop()))
            {
                try
                {
                    server.Start();
                    this.Log($"Listening at {localAddress}:{port}");
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            await this.HandleClient(await server.AcceptTcpClientAsync(), cancellationToken);
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

        private async Task HandleClient(TcpClient client, CancellationToken cancellationToken)
        {
            await Task.Yield();
            this.Log($"Client connected from {client.Client.RemoteEndPoint}");
            try
            {
                using (client)
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream))
                using (var writer = new StreamWriter(stream) { AutoFlush = true })
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var buffer = new byte[CommandMessageLength];
                        var readCount = await stream.ReadAsync(buffer, 0, CommandMessageLength, cancellationToken).ConfigureAwait(false);
                        if (readCount == CommandMessageLength)
                        {
                            string response = "nope";
                            switch ((Commands)buffer[0])
                            {
                                case Commands.Wake:
                                    _controller.Wake();
                                    goto case Commands.GetStatus;
                                case Commands.Sleep:
                                    _controller.Sleep();
                                    goto case Commands.GetStatus;
                                case Commands.GetStatus:
                                    response = this.GetStatus();
                                    break;
                            }
                            await writer.WriteLineAsync(response).ConfigureAwait(false);
                        }
                    }
                }
            }
            finally
            {
                this.Log("Client disconnected");
            }
        }

        private string GetStatus() => _controller.IsAwake() ? "I'm awake!" : "sleep af";

        private void Log(string message)
        {
            _status.OnNext(message);
        }

        private readonly ReplaySubject<string> _status = new ReplaySubject<string>(50);
        private IMaestroController _controller;
        private const int CommandMessageLength = 2;
    }
}
