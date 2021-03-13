using Maestro.Server.Gpio;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro.Server.Terminal
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Creating Server...");
            var server = new TcpMaestroServer(new GpioMaestroController());
            var cancellationSource = new CancellationTokenSource();
            Console.WriteLine("Starting Server...");
            try 
            {
                await StartServer(server, cancellationSource).ConfigureAwait(false);
                Console.WriteLine("Awaiting Connections...");
            } 
            catch (Exception ex) 
            {
                Console.WriteLine($"Some shit happened: {ex.Message}");
            }
            Console.ReadKey();        
        }

        private static async Task StartServer(TcpMaestroServer server, CancellationTokenSource cancellationSource)
        {
            await Task.Yield();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList.FirstOrDefault(add => add.AddressFamily is AddressFamily.InterNetwork);
            Console.WriteLine($"Binding to IP {ip}");
            await server.Start(ip, 4321, cancellationSource.Token).ConfigureAwait(false);
        }

    }
}
