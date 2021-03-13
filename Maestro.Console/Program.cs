using Maestro.Client;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Terminal = System.Console;

namespace Maestro.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var discoveryClient = new UdpMaestroDiscoveryClient();
            var client = new TcpMaestroClient();
            
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

            var device = await discoveryClient.DiscoverAsync().Take(1);
            Terminal.WriteLine($"Discovered {device.Name} @ {device.Address}");
            await client.ConnectAsync(device.Address, 4321).ConfigureAwait(false);
            System.Console.WriteLine("Connected AF");
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
        }
    }
}
