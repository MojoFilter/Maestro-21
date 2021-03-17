using Maestro;
using Maestro.Client;
using System.Net;

namespace MaestroCommander
{

    public interface IDeviceViewModelFactory
    {
        IDeviceViewModel NewViewModel(MaestroDeviceInfo deviceInfo);
    }

    internal class DeviceViewModelFactory : IDeviceViewModelFactory
    {
        public DeviceViewModelFactory(IMaestroClientFactory maestroClientFactory)
        {
            _maestroClientFactory = maestroClientFactory;
        }

        public IDeviceViewModel NewViewModel(MaestroDeviceInfo deviceInfo)
        {
            var ip = IPAddress.Parse(deviceInfo.Address);
            var client = _maestroClientFactory.NewTcpMaestroClient(ip);
            return new SandboxDeviceViewModel(deviceInfo.Name, client);
        }

        private readonly IMaestroClientFactory _maestroClientFactory;
    }
}
