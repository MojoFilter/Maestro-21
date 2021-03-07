using Maestro.Client;
using System;
using System.Threading.Tasks;

namespace Maestro.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new TcpMaestroClient();
            await client.ConnectAsync("192.168.86.63", 4321).ConfigureAwait(false);
            System.Console.WriteLine("Connected AF");
            bool gettin = true;
            do
            {
                System.Console.Write("?");
                var cmd = System.Console.ReadKey();
                System.Console.WriteLine();
                var response = string.Empty;
                switch (cmd.KeyChar)
                {
                    case 's':
                        response = await client.GetStatusAsync().ConfigureAwait(false);
                        break;
                    case '+':
                        response = await client.WakeAsync().ConfigureAwait(false);
                        break;
                    case '-':
                        response = await client.SleepAsync().ConfigureAwait(false);
                        break;
                    case 'q':
                        gettin = false;
                        break;
                }
                System.Console.WriteLine($"> {response}");
            } while (gettin);
        }
    }
}
