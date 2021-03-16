using System;

namespace Maestro
{
    public interface IMaestroDiscoveryClient
    {
        IObservable<MaestroDeviceInfo> DiscoverAsync();
    }

    public struct MaestroDeviceInfo
    {
        public string Address { get; set; }
        public string Name { get; set; }
    }
}
