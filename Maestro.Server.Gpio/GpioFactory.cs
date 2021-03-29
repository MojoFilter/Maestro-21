using Maestro.Devices.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;

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
        public IMaestroController NewController()
        {
            var tapper = new MultiTapper(CreateTappers().ToArray());
            return new GpioMaestroController(tapper);
        }

        private static IEnumerable<ITapper> CreateTappers()
        {
            var gpio = new GpioController();
            return TapperConfigurations.Select(c =>
                new TapperDriver(TapperExtent, gpio, c.enPwmChannel, c.in1, c.in2))
                .ToArray();
        }

        private static readonly TimeSpan TapperExtent = TimeSpan.FromMilliseconds(150);
        private static readonly (int enPwmChannel, int in1, int in2)[] TapperConfigurations = new[]
        {
            (0, 5, 6),
            (0, 23, 24)
        };
    }
}
