using System;
using System.IO;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro.Client
{
    public class TcpMaestroClient
    {
        public async Task ConnectAsync(string host, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(host, port).ConfigureAwait(false);
            this.Read();
        }

        public IObservable<bool> Status => _status.AsObservable();

        public IObservable<Exception> Error => _error.AsObservable();

        public Task GetStatusAsync(CancellationToken cancellationToken = default)
            => this.ExecuteAsync(Commands.GetStatus, 0x0, cancellationToken);

        public Task WakeAsync(CancellationToken cancellationToken = default)
            => this.ExecuteAsync(Commands.Wake, 0x0, cancellationToken);

        public Task SleepAsync(CancellationToken cancellationToken = default)
            => this.ExecuteAsync(Commands.Sleep, 0x0, cancellationToken);

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
                var reader = new StreamReader(stream);
                while (!_readCanceller.IsCancellationRequested)
                {
                    var message = await reader.ReadLineAsync().ConfigureAwait(false);
                    _status.OnNext(message.StartsWith("I"));
                }
            }
            catch (Exception ex)
            {
                _error.OnNext(ex);
            }
        }

        private readonly ISubject<Exception> _error = new Subject<Exception>();
        private readonly ISubject<bool> _status = new BehaviorSubject<bool>(false);
        private TcpClient _client;
        private CancellationTokenSource _readCanceller = new CancellationTokenSource();
    }
}
