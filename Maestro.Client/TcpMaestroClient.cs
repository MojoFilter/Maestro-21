using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro.Client
{
    public class TcpMaestroClient : IMaestroClient
    {
        public TcpMaestroClient(int port, IPAddress host)
        {
            _port = port;
            _host = host;
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(_host, _port).ConfigureAwait(false);
            this.Read();
        }


        public IObservable<bool> Status => _status.AsObservable();

        public IObservable<byte> Fade => _fade.AsObservable();

        public IObservable<Exception> Error => _error.AsObservable();

        public Task GetStatusAsync(CancellationToken cancellationToken = default)
            => this.ExecuteAsync(Commands.GetStatus, 0x0, cancellationToken);

        public Task GetFadeAsync(CancellationToken cancellationToken = default)
            => this.ExecuteAsync(Commands.GetFade, 0x0, cancellationToken);

        public Task SetFadeAsync(byte level, CancellationToken cancellationToken = default)
            => this.ExecuteAsync(Commands.SetFade, level, cancellationToken);

        public Task WakeAsync(CancellationToken cancellationToken = default)
            => this.ExecuteAsync(Commands.Wake, 0x0, cancellationToken);

        public Task SleepAsync(CancellationToken cancellationToken = default)
            => this.ExecuteAsync(Commands.Sleep, 0x0, cancellationToken);

        public Task TapAsync(CancellationToken cancellationToken = default)
            => this.ExecuteAsync(Commands.Tap, 0x0, cancellationToken);

        private async Task ExecuteAsync(Commands command, byte arg, CancellationToken cancellationToken)
        {
            var stream = _client.GetStream();
            var message = new byte[] { (byte)command, arg };
            await stream.WriteAsync(message, 0, 2, cancellationToken).ConfigureAwait(false);
        }

        private async void Read()
        {
            try
            {
                var stream = _client.GetStream();
                var buffer = new byte[2];
                while (!_readCanceller.IsCancellationRequested)
                {
                    var readCount = await stream.ReadAsync(buffer, 0, 2).ConfigureAwait(false);
                    switch ((Commands)buffer[0])
                    {
                        case Commands.UpdateFade:
                            _fade.OnNext(buffer[1]);
                            break;
                        case Commands.UpdateState:
                            _status.OnNext(buffer[1] > 0);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _error.OnNext(ex);
            }
        }

        private readonly ISubject<Exception> _error = new Subject<Exception>();
        private readonly ISubject<bool> _status = new BehaviorSubject<bool>(false);
        private readonly ISubject<byte> _fade = new BehaviorSubject<byte>(0);
        private readonly int _port;
        private readonly IPAddress _host;
        private TcpClient _client;
        private CancellationTokenSource _readCanceller = new CancellationTokenSource();

    }
}
