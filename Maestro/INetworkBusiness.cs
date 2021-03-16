using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Maestro
{
    public interface INetworkBusiness
    {
        IPAddress GetServiceAddress();
        int GetServicePort();
    }

    internal class NetworkBusiness : INetworkBusiness
    {
        public IPAddress GetServiceAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList
                       .Where(add => add.AddressFamily is AddressFamily.InterNetwork)
                       .Where(ip => !IPAddress.IsLoopback(ip))
                       .FirstOrDefault();
        }

        public int GetServicePort() => 4321;
    }
}
