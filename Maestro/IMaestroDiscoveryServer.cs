using System.Threading;

namespace Maestro
{
    public interface IMaestroDiscoveryServer
    {
        void Start(CancellationToken cancellationToken = default);
    }
}
