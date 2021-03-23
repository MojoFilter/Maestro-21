using Microsoft.Extensions.DependencyInjection;

namespace Maestro
{
    public interface IMaestroFactory 
    {
        IMaestroController NewDebugController();
        INetworkBusiness NewNetworkBusiness();
    }
    
    public static class MaestroFactoryConfiguration
    {
        public static IServiceCollection UseMaestro(this IServiceCollection s)
        {
            s.AddTransient<IMaestroFactory, MaestroFactory>();
            s.AddTransient(p => p.GetRequiredService<IMaestroFactory>().NewNetworkBusiness());
            //s.AddTransient(p => p.GetRequiredService<IMaestroFactory>().NewDebugController());
            return s;
        }
    }

    public class MaestroFactory : IMaestroFactory
    {
        public INetworkBusiness NewNetworkBusiness() => new NetworkBusiness();
        public IMaestroController NewDebugController() => new DebugController();
    }
}
