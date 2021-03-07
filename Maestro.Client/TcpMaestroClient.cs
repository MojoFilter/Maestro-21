using System.IO;
using System.Net.Sockets;
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
        }

        public Task<string> GetStatusAsync(CancellationToken cancellationToken = default)
            => this.ExecuteAsync(Commands.GetStatus, 0x0, cancellationToken);

        public Task<string> WakeAsync(CancellationToken cancellationToken = default)
            => this.ExecuteAsync(Commands.Wake, 0x0, cancellationToken);

        public Task<string> SleepAsync(CancellationToken cancellationToken = default)
            => this.ExecuteAsync(Commands.Sleep, 0x0, cancellationToken);

        private async Task<string> ExecuteAsync(Commands command, byte arg, CancellationToken cancellationToken)
        {
            var stream = _client.GetStream();
            var message = new byte[] { (byte)command, arg };
            await stream.WriteAsync(message, 0, 2, cancellationToken).ConfigureAwait(false);
            var reader = new StreamReader(stream, Encoding.UTF8, true, 8, true);
            return await reader.ReadLineAsync().ConfigureAwait(false);
        }

        private TcpClient _client;
    }
}
