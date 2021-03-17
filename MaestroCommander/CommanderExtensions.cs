using System;
using System.Diagnostics;

namespace MaestroCommander
{
    public static class CommanderExtensions
    {
        public static IDisposable Fail(this IObservable<Exception> src)
        {
            return src.Subscribe(ex => Debug.Fail(ex.Message));
        }
    }
}
