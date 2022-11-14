using Jacobi.Vst.Plugin.Framework;
using Jacobi.Vst.Plugin.Framework.Plugin;

namespace SimpleSynthVst
{
    public sealed class PluginCommandStub : StdPluginCommandStub
    {
        protected override IVstPlugin CreatePluginInstance()
        {
            return new Plugin();
        }
    }
}
