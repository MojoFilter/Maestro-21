using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Maestro
{
    public static class Util
    {
        public static IDisposable SubscribeAsync<T>(this IObservable<T> src, Func<CancellationToken, Task> onNext)
        {
            return SubscribeAsync(src, (T _) => onNext(default));
        }

        public static IDisposable SubscribeAsync<T>(this IObservable<T> src, Func<T, Task> onNext)
        {
            return src.SelectMany(async x =>
            {
                await onNext(x).ConfigureAwait(false);
                return Unit.Default;
            }).Subscribe();
        }
    }
}
