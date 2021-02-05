namespace OpenTkUI
{
    using System;
    using System.Runtime.InteropServices;
    using SDL2;

    public class GusboyAudio
    {
        private readonly SDL.SDL_AudioSpec obtainedSpec;
        private readonly IntPtr stream;

        public GusboyAudio(int sampleRate)
        {
            SDL.SDL_AudioSpec desiredSpec = new SDL.SDL_AudioSpec
            {
                freq = sampleRate,
                format = SDL.AUDIO_F32,
                channels = 2,
                samples = 1024,
                callback = this.Callback,
                userdata = IntPtr.Zero, // TODO: Is this required?
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
            // Console.WriteLine($"   Format: {this.obtainedSpec.format}");
            this.stream = SDL.SDL_NewAudioStream(
                desiredSpec.format,
                desiredSpec.channels,
                desiredSpec.freq,
                this.obtainedSpec.format,
                this.obtainedSpec.channels,
                this.obtainedSpec.freq);

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
            return SDL.SDL_AudioStreamAvailable(this.stream) / sizeof(float) / 2;
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

        private void Callback(IntPtr userdata, IntPtr buffer, int len)
        {
            int available = SDL.SDL_AudioStreamAvailable(this.stream);

            if (available < len)
            {
                // Buffer underflow, return empty
                Marshal.Copy(new byte[len], 0, buffer, len);
                SDL.SDL_AudioStreamClear(this.stream);
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
                    Marshal.Copy(new byte[len - obtained], obtained, buffer, len - obtained);
                }
            }
        }
    }
}
