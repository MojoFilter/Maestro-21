using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Maestro.Server.Gpio
{
    public interface IMaestroGpioFactory
    {
        IMaestroController NewController();
    }

    public static class GpioConfiguration
    {
        public static IServiceCollection UseGpio(this IServiceCollection s)
        {
            s.AddTransient<IMaestroGpioFactory, GpioFactory>();
            s.AddTransient(p => p.GetRequiredService<IMaestroGpioFactory>().NewController());
            return s;
        }
    }

    internal class GpioFactory : IMaestroGpioFactory
    {
        public IMaestroController NewController() => new GpioMaestroController();
    }
}
