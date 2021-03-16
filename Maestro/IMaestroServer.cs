using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro
{
    public interface IMaestroServer
    {
        IObservable<string> Status { get; }

        Task StartAsync(CancellationToken cancellationToken);
    }
}
