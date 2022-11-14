using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SimpleSynthVst
{
    internal sealed class SynthManager
    {
        private List<MidiMessage> messages = new List<MidiMessage>();

        private int sampleRate;

        private bool noteOn;
        private float phase;
        private float deltaPhase;
        private float envelope;

        internal void ClearMessages()
        {
            messages.Clear();
        }

        internal void PushMessage(int delta, int channel, int command, int data1, int data2)
        {
            messages.Add(new MidiMessage(delta, channel, command, data1, data2));
        }

        internal void Render(Span<float> left, Span<float> right)
        {
            var start = 0;

            foreach (var msg in messages)
            {
                var count = msg.Delta - start;
                RenderCore(left.Slice(start, count), right.Slice(start, count));
                ProcessMidiMessage(msg.Channel, msg.Command, msg.Data1, msg.Data2);
                start = msg.Delta;
            }

            RenderCore(left.Slice(start), right.Slice(start));
        }

        private void RenderCore(Span<float> left, Span<float> right)
        {
            for (var t = 0; t < left.Length; t++)
            {
                if (noteOn)
                {
                    envelope += 0.005F;
                }
                else
                {
                    envelope -= 0.001F;
                }
                envelope = Math.Clamp(envelope, 0F, 0.5F);

                var value = envelope * MathF.Sin(phase);
                left[t] = value;
                right[t] = value;
                phase = (phase + deltaPhase) % (2 * MathF.PI);
            }
        }

        private void ProcessMidiMessage(int channel, int command, int data1, int data2)
        {
            switch (command)
            {
                case 0x80: // Note Off
                    noteOn = false;
                    break;

                case 0x90: // Note On
                    noteOn = true;
                    var frequency = 440 * MathF.Pow(2F, (data1 - 69) / 12F);
                    deltaPhase = 2 * MathF.PI * frequency / sampleRate;
                    break;
            }
        }

        internal int SampleRate
        {
            get => sampleRate;
            set => sampleRate = value;
        }



        private struct MidiMessage
        {
            private int delta;
            private byte channel;
            private byte command;
            private byte data1;
            private byte data2;

            internal MidiMessage(int delta, int channel, int command, int data1, int data2)
            {
                this.delta = delta;
                this.channel = (byte)channel;
                this.command = (byte)command;
                this.data1 = (byte)data1;
                this.data2 = (byte)data2;
            }

            internal int Delta => delta;
            internal int Channel => channel;
            internal int Command => command;
            internal int Data1 => data1;
            internal int Data2 => data2;
        }
    }
}
