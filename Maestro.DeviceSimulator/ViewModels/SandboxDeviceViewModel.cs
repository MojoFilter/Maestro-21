using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Maestro;

namespace Maestro.DeviceSimulator.ViewModels
{
    public class SandboxDeviceViewModel
    {
        private async Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            await _server.StartAsync(_cancellationTokenSource.Token).ConfigureAwait(false);
        }

        private readonly IMaestroServer _server;
        private CancellationTokenSource _cancellationTokenSource;
    }
}
