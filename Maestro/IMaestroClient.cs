using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro
{
    public interface IMaestroClient
    {
        IObservable<bool> Status { get; }
        IObservable<byte> Fade { get; }
        IObservable<Exception> Error { get; }

        Task GetFadeAsync(CancellationToken cancellationToken = default);
        Task GetStatusAsync(CancellationToken cancellationToken = default);
        Task SetFadeAsync(byte level, CancellationToken cancellationToken = default);
        Task SleepAsync(CancellationToken cancellationToken = default);
        Task WakeAsync(CancellationToken cancellationToken = default);
    }
}
