using Jacobi.Vst.Plugin.Framework;
using Jacobi.Vst.Plugin.Framework.Plugin;
using Maestro.Plugin;

namespace MaestroConductor
{
    public class PluginCommandStub : StdPluginCommandStub
    {
        protected override IVstPlugin CreatePluginInstance()
        {
            return new Plugin();
        }
    }
}
