namespace OpenTkUI
{
    using System;
    using OpenTK.Audio.OpenAL;

    public class GusboyAudio
    {
        private readonly int sampleRate;
        private readonly int alSource;
        private readonly int maxBuffers = 200;
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

        public int CurrentBufferLatency()
        {
            AL.GetSource(this.alSource, ALGetSourcei.BuffersQueued, out int buffersQueued);
            AL.GetSource(this.alSource, ALGetSourcei.BuffersProcessed, out int buffersProcessed);

            return buffersQueued - buffersProcessed;
        }

        public void AddSamples(float[] samples)
        {
            int buffer;

            AL.GetSource(this.alSource, ALGetSourcei.BuffersProcessed, out int buffersProcessed);
            AL.GetSource(this.alSource, ALGetSourcei.BuffersQueued, out int buffersQueued);

            if (buffersProcessed > 0)
            {
                // There are buffers waiting to be processed
                buffer = AL.SourceUnqueueBuffer(this.alSource);
            }
            else if (buffersQueued < this.maxBuffers)
            {
                // No buffers waiting to be processed, but we haven't hit the max yet.
                buffer = AL.GenBuffer();
            }
            else
            {
                // ERROR: Buffer is full
                return;
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
    }
}
