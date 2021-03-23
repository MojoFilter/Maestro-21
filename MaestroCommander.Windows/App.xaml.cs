using Maestro;
using Maestro.Client;
using MaestroCommander.Windows.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace MaestroCommander.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var serviceProvider = this.ConfigureServices();
            var win = serviceProvider.GetRequiredService<MainWindow>();
            this.MainWindow = win;
            win.Show();
        }

        private IServiceProvider ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.UseMaestro()
                    .UseMaestroClient()
                    .UseMaestroCommander()
                    .AddTransient<MidiDirectorViewModel>()
                    .AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }
    }
}
