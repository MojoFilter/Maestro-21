using Maestro;
using System;

namespace MaestroDeviceDaemon
{
    internal class ConsoleDebug : IDebug
    {
        public void WriteLine(string v) => Console.WriteLine(v);
    }
}
