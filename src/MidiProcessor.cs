using System;
using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;

namespace SimpleSynthVst
{
    internal sealed class MidiProcessor : IVstMidiProcessor
    {
        private readonly SynthManager synthManager;

        public MidiProcessor(SynthManager synthManager)
        {
            if (synthManager == null)
            {
                throw new ArgumentNullException(nameof(synthManager));
            }

            this.synthManager = synthManager;
        }

        public void Process(VstEventCollection events)
        {
            synthManager.ClearMessages();

            foreach (VstEvent evnt in events)
            {
                if (evnt.EventType == VstEventTypes.MidiEvent)
                {
                    var midiEvent = (VstMidiEvent)evnt;

                    var channel = midiEvent.Data[0] & 0x0F;
                    var command = midiEvent.Data[0] & 0xF0;
                    var data1 = midiEvent.Data[1];
                    var data2 = midiEvent.Data[2];

                    synthManager.PushMessage(midiEvent.DeltaFrames, channel, command, data1, data2);
                }
            }
        }

        public int ChannelCount => 16;
    }
}
