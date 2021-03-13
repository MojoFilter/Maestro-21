using Maestro.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Maestro.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App CurrentMaestro => App.Current as App;

        public IMaestroClient Client { get; private set; }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var client = new TcpMaestroClient();
            this.Client = client;
            await client.ConnectAsync("192.168.86.69", 4321).ConfigureAwait(false);
        }
    }
}
