using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Jacobi.Vst.Plugin.Framework.Plugin;
using Maestro.Client;
using MaestroConductor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace Maestro.Plugin
{
    public sealed class Plugin : VstPluginWithServices
    {
        public Plugin()
            : base("Maestro Conductor",
                  GetPluginId(),
                  new VstProductInfo("Maestro Conductor", "MojoFilter", 1),
                  VstPluginCategory.Synth)
        {
        }

        protected override void ConfigureServices(IServiceCollection services) => 
            PluginConfiguration.ConfigurePluginServices(services);

        public static int GetPluginId()
        {
            var id = "MAES";
            var bytes = Encoding.UTF8.GetBytes(id);
            return BitConverter.ToInt32(bytes, 0);
        }
    }

    public static class PluginConfiguration
    {
        public static void ConfigurePluginServices(IServiceCollection services)
        {
            services.AddSingletonAll<PluginEditor>()
                    .AddSingletonAll<MidiProcessor>()
                    .AddSingletonAll<AudioProcessor>()
                    .AddSingletonAll<MaestroMap>()
                    .AddSingleton<MaestroConductorWindow>()
                    .UseMaestro()
                    .UseMaestroClient();
        }
    }
}
