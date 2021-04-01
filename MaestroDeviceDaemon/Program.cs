using Maestro;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Maestro.Server;
using System.Diagnostics;
using Maestro.Server.Gpio;

namespace MaestroDeviceDaemon
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var host = ConfigureServices().Build();
            await host.StartAsync();

            var cancellationSource = host.Services.GetRequiredService<CancellationTokenSource>();
            var server = host.Services.GetRequiredService<IMaestroServer>();
            var discoveryServer = host.Services.GetRequiredService<IMaestroDiscoveryServer>();

            server.Status.Subscribe(s => Console.WriteLine(s));
            StartServer(server, cancellationSource.Token);
            
            discoveryServer.Start(cancellationSource.Token);

            await host.WaitForShutdownAsync(cancellationSource.Token).ConfigureAwait(false);
        }

        private static IHostBuilder ConfigureServices() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                    services.AddTransient<CancellationTokenSource>()
                            .AddTransient<IDebug, ConsoleDebug>()
                            .UseMaestro()
                            .UseMaestroServer()
                            .UseGpio());

        private static async void StartServer(IMaestroServer server, CancellationToken cancellationToken)
        {
            try
            {
                await server.StartAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
