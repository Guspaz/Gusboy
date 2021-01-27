namespace OpenTkUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OpenTK.Audio.OpenAL;
    using OpenTK.Mathematics;

    public class GusboyAudio
    {
        private readonly int sampleRate;
        private readonly int alSource;
        private readonly int maxBuffers = 100;
        private readonly ALDevice alDevice;
        private readonly ALContext alContext;

        public GusboyAudio(int sampleRate)
        {
            this.sampleRate = sampleRate;

            this.alDevice = ALC.OpenDevice(null);
            var alContextAttributes = new ALContextAttributes()
            {
                Sync = true,
            };
            this.alContext = ALC.CreateContext(this.alDevice, alContextAttributes);
            ALC.MakeContextCurrent(this.alContext);

            this.alSource = AL.GenSource();
        }

        ~GusboyAudio()
        {
            ALC.DestroyContext(this.alContext);
            ALC.CloseDevice(this.alDevice);
        }

        public bool HasEmptyBuffers()
        {
            AL.GetSource(this.alSource, ALGetSourcei.BuffersProcessed, out int buffersProcessed);
            AL.GetSource(this.alSource, ALGetSourcei.BuffersQueued, out int buffersQueued);

            return buffersProcessed > 0 || buffersQueued < this.maxBuffers;
        }

        public void AddSamples(float[] samples)
        {
            int buffer;

            // Safety net: the caller should have checked HasEmptyBuffers before trying to send us data
            if (!this.HasEmptyBuffers())
            {
                return;
            }

            if (this.NeedToCreateBuffers())
            {
                // There are unused buffers (we're at the star of playback) so queue them
                buffer = AL.GenBuffer();
            }
            else
            {
                // There are no unused buffers, unqueue one
                buffer = AL.SourceUnqueueBuffer(this.alSource);
            }

            // OpenAL doesn't accept float samples, so we must convert. Linq would be a one-liner but this is probably faster.
            short[] shortBuffer = new short[samples.Length];

            for (int i = 0; i < samples.Length; i++)
            {
                shortBuffer[i] = (short)Math.Clamp(samples[i] * 32768, -32768, 32767);
            }

            // Queue up the actual buffer
            AL.BufferData<short>(buffer, ALFormat.Stereo16, shortBuffer, this.sampleRate);
            AL.SourceQueueBuffer(this.alSource, buffer);

            // If we're not already playing (or have previously stopped), then start/resume.
            if (AL.GetSourceState(this.alSource) != ALSourceState.Playing)
            {
                AL.SourcePlay(this.alSource);
            }
        }

        private bool NeedToCreateBuffers()
        {
            AL.GetSource(this.alSource, ALGetSourcei.BuffersProcessed, out int buffersProcessed);
            AL.GetSource(this.alSource, ALGetSourcei.BuffersQueued, out int buffersQueued);

            return buffersQueued + buffersProcessed < this.maxBuffers;
        }
    }
}
