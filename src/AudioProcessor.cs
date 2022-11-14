using System;
using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework.Plugin;

namespace SimpleSynthVst
{
    internal sealed class AudioProcessor : VstPluginAudioProcessor
    {
        private readonly SynthManager synthManager;

        public AudioProcessor(SynthManager synthManager) : base(0, 2, 0, true)
        {
            if (synthManager == null)
            {
                throw new ArgumentNullException(nameof(synthManager));
            }

            this.synthManager = synthManager;
        }

        public override void Process(VstAudioBuffer[] inChannels, VstAudioBuffer[] outChannels)
        {
            synthManager.Render(outChannels[0].AsSpan(), outChannels[1].AsSpan());
        }

        public override float SampleRate
        {
            get => synthManager.SampleRate;
            set => synthManager.SampleRate = (int)value;
        }
    }
}
