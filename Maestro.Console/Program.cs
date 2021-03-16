using Maestro.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Terminal = System.Console;

namespace Maestro.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var host = ConfigureServices().Build();
            await host.StartAsync().ConfigureAwait(false);

            var discoveryClient = host.Services.GetRequiredService<IMaestroDiscoveryClient>();
            var clientFactory = host.Services.GetRequiredService<IMaestroClientFactory>();
            //var client = host.Services.GetRequiredService<IMaestroClient>();

            Terminal.WriteLine("Listening for servers");
            var device = await discoveryClient.DiscoverAsync().Take(1);
            Terminal.WriteLine($"Discovered {device.Name} @ {device.Address}");
            var client = clientFactory.NewTcpMaestroClient(IPAddress.Parse(device.Address));
            await client.ConnectAsync().ConfigureAwait(false);
            System.Console.WriteLine("Connected AF");

            client.Error
                  .Subscribe(ex =>
                  {
                      Terminal.ForegroundColor = ConsoleColor.Red;
                      Terminal.WriteLine();
                      Terminal.WriteLine($"** {ex.Message}");
                      Terminal.ResetColor();
                  });

            client.Status
                  .Subscribe(isAwake =>
                  {
                      Terminal.ForegroundColor = ConsoleColor.Green;
                      var stat = isAwake ? "Awake" : "Asleep";
                      Terminal.WriteLine();
                      Terminal.WriteLine($"> {stat}");
                      Terminal.ResetColor();
                  });

            client.Fade
                .Subscribe(fade =>
                {
                    Terminal.ForegroundColor = ConsoleColor.Cyan;
                    var pct = Math.Round((fade / 255.0) * 100);
                    Terminal.WriteLine();
                    Terminal.WriteLine($"Fade: {pct}%");
                    Terminal.ResetColor();
                });
            
            await client.GetStatusAsync();
            await client.GetFadeAsync();
            bool gettin = true;
            do
            {
                System.Console.Write("?");
                var cmd = System.Console.ReadKey();
                System.Console.WriteLine();
                if (char.IsDigit(cmd.KeyChar))
                {
                    var level = (byte)(((cmd.KeyChar - '0') / 10.0) * 255.0);
                    await client.SetFadeAsync(level).ConfigureAwait(false);
                }
                else
                {
                    switch (cmd.KeyChar)
                    {
                        case 's':
                            await client.GetStatusAsync().ConfigureAwait(false);
                            break;
                        case 'f':
                            await client.GetFadeAsync().ConfigureAwait(false);
                            break;
                        case '+':
                            await client.WakeAsync().ConfigureAwait(false);
                            break;
                        case '-':
                            await client.SleepAsync().ConfigureAwait(false);
                            break;
                        case 'q':
                            gettin = false;
                            break;
                    }
                }
            } while (gettin);

            await host.WaitForShutdownAsync().ConfigureAwait(false);
        }

        private static IHostBuilder ConfigureServices() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                    services.UseMaestro()
                            .UseMaestroClient());
    }
}
