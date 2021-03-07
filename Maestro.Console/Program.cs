using Maestro.Client;
using System;
using System.Threading.Tasks;
using Terminal = System.Console;

namespace Maestro.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new TcpMaestroClient();
            await client.ConnectAsync("192.168.86.63", 4321).ConfigureAwait(false);
            
            client.Error
                  .Subscribe(ex =>
                  {
                      Terminal.ForegroundColor = ConsoleColor.Red;
                      Terminal.WriteLine($"** {ex.Message}");
                      Terminal.ResetColor();
                  });

            client.Status
                  .Subscribe(isAwake =>
                  {
                      Terminal.ForegroundColor = ConsoleColor.Green;
                      var stat = isAwake ? "Awake" : "Asleep";
                      Terminal.WriteLine($"> {stat}");
                      Terminal.ResetColor();
                  });
            System.Console.WriteLine("Connected AF");
            bool gettin = true;
            do
            {
                System.Console.Write("?");
                var cmd = System.Console.ReadKey();
                System.Console.WriteLine();
                switch (cmd.KeyChar)
                {
                    case 's':
                        await client.GetStatusAsync().ConfigureAwait(false);
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
            } while (gettin);
        }
    }
}
