using Maestro;
using Microsoft.Extensions.DependencyInjection;

namespace MaestroCommander
{
    public interface IMaestroCommanderFactory 
    {
        ICommanderViewModel NewCommanderViewModel();
    }

    public static class CommanderConfiguration
    {
        public static IServiceCollection UseMaestroCommander(this IServiceCollection s)
        {
            s.AddTransient<IDeviceViewModelFactory, DeviceViewModelFactory>();
            s.AddTransient<IMaestroCommanderFactory, CommanderFactory>();
            s.AddTransient(p => p.GetRequiredService<IMaestroCommanderFactory>().NewCommanderViewModel());
            return s;
        }
    }

    internal class CommanderFactory : IMaestroCommanderFactory
    {

        public CommanderFactory(
            IMaestroDiscoveryClient maestroDiscoveryClient,
            IDeviceViewModelFactory deviceViewModelFactory)
        {
            (_discoveryClient, _deviceViewModelFactory) 
                = (maestroDiscoveryClient, deviceViewModelFactory);
        }

        public ICommanderViewModel NewCommanderViewModel() =>
            new CommanderViewModel(_discoveryClient, _deviceViewModelFactory);

        private readonly IMaestroDiscoveryClient _discoveryClient;
        private readonly IDeviceViewModelFactory _deviceViewModelFactory;
    }
}
