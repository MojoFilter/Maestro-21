using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Maestro.Client
{
    public interface IMaestroClientFactory
    {
        IMaestroDiscoveryClient NewDiscoveryClient();
        IMaestroClient NewTcpMaestroClient(IPAddress host);
    }

    public static class ClientConfiguration
    {
        public static IServiceCollection UseMaestroClient(this IServiceCollection s)
        {
            s.AddTransient<IMaestroClientFactory, MaestroClientFactory>();
            //s.AddTransient(p => p.GetRequiredService<IMaestroClientFactory>().NewTcpMaestroClient());
            s.AddTransient(p => p.GetRequiredService<IMaestroClientFactory>().NewDiscoveryClient());
            return s;
        }
    }

    public class MaestroClientFactory : IMaestroClientFactory
    {
        public MaestroClientFactory(INetworkBusiness networkBusiness)
        {
            _net = networkBusiness;
        }

        public IMaestroClient NewTcpMaestroClient(IPAddress host) =>
            new TcpMaestroClient(_net.GetServicePort(), host);

        public IMaestroDiscoveryClient NewDiscoveryClient() =>
            new UdpMaestroDiscoveryClient();

        private readonly INetworkBusiness _net;
    }
}
