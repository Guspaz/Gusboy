namespace SdlUI
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using SDL2;

    public class GusboyAudio
    {
        private readonly SDL.SDL_AudioSpec obtainedSpec;
        private readonly uint device;
        private readonly int obtainedSampleSize;

        public GusboyAudio(int sampleRate)
        {
            SDL.SDL_AudioSpec desiredSpec = new SDL.SDL_AudioSpec
            {
                freq = sampleRate,
                format = SDL.AUDIO_F32,
                channels = 2,
                samples = 1024,
                userdata = IntPtr.Zero,
            };

            if ((this.device = SDL.SDL_OpenAudioDevice(null, 0, ref desiredSpec, out this.obtainedSpec, 0)) < 0)
            {
                Console.WriteLine($"ERROR: Couldn't open audio: {SDL.SDL_GetError()}");
                return;
            }

            // Console.WriteLine("Audio information:");
            // Console.WriteLine($"   Freq: {this.obtainedSpec.freq}");
            // Console.WriteLine($"   Channels: {this.obtainedSpec.channels}");
            // Console.WriteLine($"   Samples: {this.obtainedSpec.samples}");
            // Console.WriteLine($"   Size: {this.obtainedSpec.size}");
            // Console.WriteLine($"   Silence: {this.obtainedSpec.silence}");
            // Console.WriteLine($"   Format: {this.obtainedSpec.format}");
            this.obtainedSampleSize = (int)(this.obtainedSpec.size / this.obtainedSpec.samples / this.obtainedSpec.channels);

            SDL.SDL_PauseAudioDevice(this.device, 0);
        }

        ~GusboyAudio()
        {
            SDL.SDL_CloseAudioDevice(this.device);
        }

        public int BufferSize()
        {
            int bufferBytes = (int)SDL.SDL_GetQueuedAudioSize(this.device);

            // We multiply by 500 instead of 1000 because we assume stereo.
            return ((bufferBytes / this.obtainedSampleSize) * 500) / this.obtainedSpec.freq;
        }

        public void AddSamples(float[] samples)
        {
            var handle = GCHandle.Alloc(samples, GCHandleType.Pinned);
            int result = SDL.SDL_QueueAudio(this.device, handle.AddrOfPinnedObject(), (uint)(samples.Length * sizeof(float)));
            handle.Free();

            if (result < 0)
            {
                Console.WriteLine($"ERROR: Failed to send audio samples to device: {SDL.SDL_GetError()}");
            }
        }
    }
}
