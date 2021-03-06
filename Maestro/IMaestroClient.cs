using System;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro
{
    public interface IMaestroClient
    {
        IObservable<bool> Status { get; }
        IObservable<byte> Fade { get; }
        IObservable<Exception> Error { get; }
        bool IsConnected { get; }

        Task ConnectAsync(CancellationToken cancellationToken = default);
        Task ExtendAsync(CancellationToken cancellationToken = default);
        Task FullyExtendAsync(CancellationToken cancellationToken = default);
        Task GetFadeAsync(CancellationToken cancellationToken = default);
        Task GetStatusAsync(CancellationToken cancellationToken = default);
        Task ResetGripAsync(CancellationToken cancellationToken = default);
        Task RetractAsync(CancellationToken cancellationToken = default);
        Task SetFadeAsync(byte level, CancellationToken cancellationToken = default);
        Task SetGripAsync(byte gripAmount, CancellationToken cancellationToken = default);
        Task SleepAsync(CancellationToken cancellationToken = default);
        Task TapAsync(CancellationToken cancellationToken = default);
        Task WakeAsync(CancellationToken cancellationToken = default);
    }
}
