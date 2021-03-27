using Maestro.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaestroConductor.Test
{
    [TestClass]
    public class PluginTest
    {
        [TestMethod]
        public void CanCreatePlugin()
        {
            var services = new ServiceCollection();
           // Plugin.ConfigurePluginServices(services);

        }
    }
}
