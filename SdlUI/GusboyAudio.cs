namespace SdlUI
{
    using System;
    using System.Runtime.InteropServices;
    using SDL2;

    public class GusboyAudio
    {
        private readonly SDL.SDL_AudioSpec obtainedSpec;
        private readonly IntPtr stream;
        private readonly int obtainedSampleSize;

        public GusboyAudio(int sampleRate)
        {
            SDL.SDL_AudioSpec desiredSpec = new SDL.SDL_AudioSpec
            {
                freq = sampleRate,
                format = SDL.AUDIO_F32,
                channels = 2,
                samples = 256,
                callback = this.Callback,
                userdata = IntPtr.Zero,
            };

            if (SDL.SDL_OpenAudio(ref desiredSpec, out this.obtainedSpec) < 0)
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
            this.stream = SDL.SDL_NewAudioStream(
                desiredSpec.format,
                desiredSpec.channels,
                desiredSpec.freq,
                this.obtainedSpec.format,
                this.obtainedSpec.channels,
                this.obtainedSpec.freq);

            this.obtainedSampleSize = (int)GetSampleSize(this.obtainedSpec);

            if (this.stream == IntPtr.Zero)
            {
                Console.WriteLine($"ERROR: Couldn't create audio stream: {SDL.SDL_GetError()}");
                return;
            }

            SDL.SDL_PauseAudio(0);
        }

        ~GusboyAudio()
        {
            SDL.SDL_FreeAudioStream(this.stream);
        }

        public int BufferSize()
        {
            // Assume the internal buffer is always full, since we can't get its size
            int bufferBytes = (int)this.obtainedSpec.size + SDL.SDL_AudioStreamAvailable(this.stream);

            // TODO: Handle where internal buffer size is bigger than the desired buffer size (we'll starve the stream)
            // We multiply by 500 instead of 1000 because we assume stereo.
            return ((bufferBytes / this.obtainedSampleSize) * 500) / this.obtainedSpec.freq;
        }

        public void AddSamples(float[] samples)
        {
            var handle = GCHandle.Alloc(samples, GCHandleType.Pinned);
            int result = SDL.SDL_AudioStreamPut(this.stream, handle.AddrOfPinnedObject(), samples.Length * sizeof(float));
            handle.Free();

            if (result < 0)
            {
                Console.WriteLine($"ERROR: Failed to send samples to stream: {SDL.SDL_GetError()}");
            }
        }

        private static uint GetSampleSize(SDL.SDL_AudioSpec obtainedSpec)
        {
            return obtainedSpec.size / obtainedSpec.samples / obtainedSpec.channels;
        }

        private void Callback(IntPtr userdata, IntPtr buffer, int len)
        {
            int available = SDL.SDL_AudioStreamAvailable(this.stream);

            if (available < len)
            {
                // Buffer underflow, return empty
                Marshal.Copy(new byte[len], 0, buffer, len);
                SDL.SDL_AudioStreamClear(this.stream);

                // Console.WriteLine("WARNING: Audio buffer empty.");
            }
            else
            {
                int obtained = SDL.SDL_AudioStreamGet(this.stream, buffer, len);

                if (obtained == -1)
                {
                    Console.WriteLine($"ERROR: Failed to get converted audio data: {SDL.SDL_GetError()}");
                }
                else if (obtained != len)
                {
                    // Unexpected returned number? Clear the rest of the buffer
                    Console.WriteLine("ERROR: Unexpected audio converted sample count.");

                    // TODO: Validate this is right
                    Marshal.Copy(new byte[len - obtained], 0, buffer + obtained, len - obtained);
                }
            }
        }
    }
}
