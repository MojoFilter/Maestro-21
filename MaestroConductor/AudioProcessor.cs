using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Jacobi.Vst.Plugin.Framework.Plugin;
using System;

namespace MaestroConductor
{
    internal sealed class AudioProcessor : VstPluginAudioProcessor
    {
        public AudioProcessor(IVstPluginEvents pluginEvents) : base(0, 0, 0, true)
        {
            pluginEvents.Opened += Plugin_Opened;
        }

        public override void Process(VstAudioBuffer[] inChannels, VstAudioBuffer[] outChannels)
        {
            if (_hostProcessor != null &&
                _midiProcessor != null &&
                _midiProcessor.Events.Count > 0)
            {
                _hostProcessor.Process(_midiProcessor.Events);
                _midiProcessor.Events.Clear();
            }

            base.Process(inChannels, outChannels);
        }

        private void Plugin_Opened(object? sender, EventArgs e)
        {
            var plugin = (VstPlugin?)sender;
            _midiProcessor = plugin?.GetInstance<MidiProcessor>();
            _hostProcessor = plugin?.Host?.GetInstance<IVstMidiProcessor>();
        }

        private MidiProcessor? _midiProcessor;
        private IVstMidiProcessor? _hostProcessor;
    }
}
