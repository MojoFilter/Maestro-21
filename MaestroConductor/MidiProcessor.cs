using Jacobi.Vst.Plugin.Framework;

namespace MaestroConductor
{
    internal sealed class MidiProcessor : IVstMidiProcessor, IVstPluginMidiSource
    {
        public VstEventCollection Events { get; } = new VstEventCollection();

        public int ChannelCount => 16;

        public void Process(VstEventCollection events)
        {
            this.Events.AddRange(events);
        }
    }
}
