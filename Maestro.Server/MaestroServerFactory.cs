using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maestro.Server
{
    public interface IMaestroServerFactory
    {
        IMaestroDiscoveryServer NewDiscoveryServer();
        IMaestroServer NewMaestroServer();
    }

    public static class ServerFactoryConfiguration
    {
        public static IServiceCollection UseMaestroServer(this IServiceCollection s)
        {
            s.AddTransient<IMaestroServerFactory, MaestroServerFactory>();
            s.AddTransient(p => p.GetRequiredService<IMaestroServerFactory>().NewMaestroServer());
            s.AddTransient(p => p.GetRequiredService<IMaestroServerFactory>().NewDiscoveryServer());
            return s;
        }
    }

    public class MaestroServerFactory : IMaestroServerFactory
    {
        public MaestroServerFactory(INetworkBusiness net, IMaestroController maestroController)
        {
            _net = net;
            _maestroController = maestroController;
        }

        public IMaestroServer NewMaestroServer() =>
            new TcpMaestroServer(_net.GetServiceAddress(), _net.GetServicePort(), _maestroController);

        public IMaestroDiscoveryServer NewDiscoveryServer() =>
            new UdpMaestroDiscoveryServer();

        private readonly INetworkBusiness _net;
        private readonly IMaestroController _maestroController;

    }
}
