namespace SdlUI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime;
    using System.Runtime.InteropServices;
    using Gusboy;

    using SDL2;

    internal class Window
    {
        // How many samples per second (The Gameboy CPU clock is evenly divisible by 32768)
        private const int SAMPLE_RATE = 32768;

        // How many samples should we generate in one chunk?
        private const int DESIRED_BUFFER_LENGTH = (SAMPLE_RATE * 2) / (1024 * 8); // 1/8th of a milliscond

        // How many milliseconds of audio buffers should we try to maintain? Assumes the internal SDL buffers are always full.
        private const int TARGET_BUFFER = 60;

        // Used in the formula that adjusts the time between audio chunks to maintain the buffer size.
        // Lower numbers are more aggressive but have less even frame pacing.
        private const int AUDIO_ADJUSTMENT_FACTOR = 1000;

        private readonly Dictionary<SDL.SDL_Scancode, Input.Keys> keymap = new Dictionary<SDL.SDL_Scancode, Input.Keys>
        {
            { SDL.SDL_Scancode.SDL_SCANCODE_A, Input.Keys.A },
            { SDL.SDL_Scancode.SDL_SCANCODE_B, Input.Keys.B },
            { SDL.SDL_Scancode.SDL_SCANCODE_SPACE, Input.Keys.Select },
            { SDL.SDL_Scancode.SDL_SCANCODE_RETURN, Input.Keys.Start },
            { SDL.SDL_Scancode.SDL_SCANCODE_UP, Input.Keys.Up },
            { SDL.SDL_Scancode.SDL_SCANCODE_DOWN, Input.Keys.Down },
            { SDL.SDL_Scancode.SDL_SCANCODE_LEFT, Input.Keys.Left },
            { SDL.SDL_Scancode.SDL_SCANCODE_RIGHT, Input.Keys.Right },
        };

        // The buffer the gameboy renders directly into
        private readonly int[] framebuffer = new int[160 * 144];

        private readonly GCHandle framebufferHandle;
        private readonly IntPtr framebufferSurface;

        private readonly List<float> frameTimes = new List<float>();

        private readonly Queue<int> audioBufferStatusQueue = new Queue<int>();

        private readonly IntPtr window;
        private readonly IntPtr renderer;

        private readonly double baseTimeBetweenSamples = Stopwatch.Frequency / (SAMPLE_RATE * 2 / DESIRED_BUFFER_LENGTH);
        private double timeBetweenSamples = Stopwatch.Frequency / (SAMPLE_RATE * 2 / DESIRED_BUFFER_LENGTH);
        private long nextAudioSampleTime;

        private Gameboy gb;
        private GusboyAudio audio;

        private long frameCount;

        private int audioBufferStatusAccumulator;

        private long lastRenderTick;
        private long lastFrameTime;

        public Window(IntPtr windowPointer, IntPtr rendererPointer)
        {
            this.window = windowPointer;
            this.renderer = rendererPointer;
            this.framebufferHandle = GCHandle.Alloc(this.framebuffer, GCHandleType.Pinned);
            this.framebufferSurface = SDL.SDL_CreateRGBSurfaceWithFormatFrom(this.framebufferHandle.AddrOfPinnedObject(), 160, 144, 32, 4 * 160, SDL.SDL_PIXELFORMAT_ARGB8888);

            SDL.SDL_RenderSetLogicalSize(this.renderer, 160, 144);
            SDL.SDL_RenderSetIntegerScale(this.renderer, SDL.SDL_bool.SDL_TRUE);

            // TODO: Get this from the emulator and not hardcoded
            // Clear the initial screen
            SDL.SDL_SetRenderDrawColor(this.renderer, 0xC6, 0xCB, 0xA5, 0xFF); // MGB screen off colour
            SDL.SDL_RenderClear(this.renderer);
            SDL.SDL_RenderPresent(this.renderer);
        }

        ~Window()
        {
            SDL.SDL_FreeSurface(this.framebufferSurface);
            this.framebufferHandle.Free();
        }

        public void Run()
        {
            var args = Environment.GetCommandLineArgs();

            if (args?.Length == 2)
            {
                this.InitGameboy(args[1]);
            }

            this.audio = new GusboyAudio(SAMPLE_RATE);

            // Wait for something to initialize the gameboy
            while (this.gb == null)
            {
                this.HandleEvent();
            }

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            this.nextAudioSampleTime = Stopwatch.GetTimestamp() + (int)this.timeBetweenSamples;

            while (true)
            {
                this.HandleEvent();

                // Unroll this to have an 8:1 ratio between event handling and emulator execution.
                this.OnUpdateFrame();
                this.OnUpdateFrame();
                this.OnUpdateFrame();
                this.OnUpdateFrame();
                this.OnUpdateFrame();
                this.OnUpdateFrame();
                this.OnUpdateFrame();
                this.OnUpdateFrame();
            }
        }

        protected void HandleEvent()
        {
            SDL.SDL_PollEvent(out SDL.SDL_Event sdlevent);

            switch (sdlevent.type)
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    Environment.Exit(0);
                    break;

                case SDL.SDL_EventType.SDL_DROPFILE:
                    this.OnFileDrop(sdlevent.drop);
                    break;

                case SDL.SDL_EventType.SDL_KEYDOWN:
                    this.OnKeyDown(sdlevent.key.keysym.scancode);
                    break;

                case SDL.SDL_EventType.SDL_KEYUP:
                    this.OnKeyUp(sdlevent.key.keysym.scancode);
                    break;

                case SDL.SDL_EventType.SDL_WINDOWEVENT:
                    // Force window size to match renderer scale, unless we're maximized
                    if (sdlevent.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED
                        && (SDL.SDL_GetWindowFlags(this.window) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_MAXIMIZED) == 0)
                    {
                        SDL.SDL_RenderGetScale(this.renderer, out float scaleX, out float scaleY);
                        SDL.SDL_SetWindowSize(this.window, (int)(scaleX * 160), (int)(scaleY * 144));
                    }

                    break;

                // case SDL.SDL_EventType.SDL_WINDOWEVENT:
                //     Console.WriteLine($"DEBUG: Unhandled SDL event {sdlevent.window.windowEvent}");
                //     break;

                // default:
                //     Console.WriteLine($"DEBUG: Unhandled SDL event {sdlevent.type}");
                //     break;
            }
        }

        protected void OnUpdateFrame()
        {
            long currentTime = Stopwatch.GetTimestamp();

            if (currentTime >= this.nextAudioSampleTime)
            {
                // If we fall more than 50ms of samples behind, rebase the clock.
                // TODO: Remove magic number
                if (((currentTime - this.nextAudioSampleTime) / this.baseTimeBetweenSamples) > 410)
                {
                    Console.WriteLine("WARNING: audio clock is too far behind, rebasing sample clock.");
                    this.nextAudioSampleTime = currentTime + (int)this.baseTimeBetweenSamples;
                }

                float audioBufferStatusAverage = this.UpdateAudioBufferAverage(this.audio.BufferSize());

                this.audio.AddSamples(this.gb.TickForAudio(DESIRED_BUFFER_LENGTH).ToArray());

                // TODO: Better algorithm here.
                this.timeBetweenSamples = this.baseTimeBetweenSamples * (1 + ((audioBufferStatusAverage - TARGET_BUFFER) / AUDIO_ADJUSTMENT_FACTOR));

                this.nextAudioSampleTime += (int)this.timeBetweenSamples;
            }
        }

        protected void OnKeyDown(SDL.SDL_Scancode key)
        {
            if ((SDL.SDL_GetWindowFlags(this.window) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS) != 0)
            {
                if (this.keymap.ContainsKey(key))
                {
                    this.gb.KeyDown(this.keymap[key]);
                }
            }
        }

        protected void OnKeyUp(SDL.SDL_Scancode key)
        {
            if ((SDL.SDL_GetWindowFlags(this.window) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS) != 0)
            {
                if (this.keymap.ContainsKey(key))
                {
                    this.gb.KeyUp(this.keymap[key]);
                }
            }
        }

        protected void OnFileDrop(SDL.SDL_DropEvent dropEvent)
        {
            // TODO: Check if file exists
            string file = Marshal.PtrToStringAnsi(dropEvent.file);
            this.InitGameboy(file);
        }

        protected void OnRenderFrame()
        {
            // Measure the framerate every 60 frames
            if (++this.frameCount == 60)
            {
                SDL.SDL_SetWindowTitle(this.window, $"Gusboy - FPS: {(double)this.frameCount / ((Stopwatch.GetTimestamp() - this.lastFrameTime) / (double)Stopwatch.Frequency):0.000} Buffer: {this.audio.BufferSize()}ms");
                this.lastFrameTime = Stopwatch.GetTimestamp();
                this.frameCount = 0;
            }

            long currentTime = Stopwatch.GetTimestamp();

            var frameTime = ((currentTime - this.lastRenderTick) / (float)Stopwatch.Frequency) * 1000;

            if (frameTime > 30)
            {
                Console.WriteLine($"Spike: {frameTime}");
            }

            this.frameTimes.Add(frameTime);
            this.lastRenderTick = currentTime;

            if (this.frameTimes.Count == 4000)
            {
                File.WriteAllLines("frametimes.txt", this.frameTimes.Select(f => f.ToString()));
                this.frameTimes.Clear();
            }

            // Copy the framebuffer from the array to the GPU VRAM
            var texture = SDL.SDL_CreateTextureFromSurface(this.renderer, this.framebufferSurface);

            // Draw the texture to the screen (default source/destination will just fill the window/screen)
            SDL.SDL_RenderCopy(this.renderer, texture, IntPtr.Zero, IntPtr.Zero);

            // Swap buffers (tell the GPU to display this frame)
            SDL.SDL_RenderPresent(this.renderer);

            // We're done with the texture so ditch it
            SDL.SDL_DestroyTexture(texture);
        }

        private float UpdateAudioBufferAverage(int newValue)
        {
            this.audioBufferStatusAccumulator += newValue;
            this.audioBufferStatusQueue.Enqueue(newValue);

            if (this.audioBufferStatusQueue.Count > 128)
            {
                this.audioBufferStatusAccumulator -= this.audioBufferStatusQueue.Dequeue();
            }

            return this.audioBufferStatusAccumulator / (float)this.audioBufferStatusQueue.Count;
        }

        private void InitGameboy(string filePath)
        {
            if (File.Exists(filePath))
            {
                this.gb = new Gameboy(this.MessageCallback, this.DrawFramebuffer, this.framebuffer, SAMPLE_RATE, filePath);
                this.lastFrameTime = Stopwatch.GetTimestamp();
                this.lastRenderTick = Stopwatch.GetTimestamp();
            }
        }

        private bool DrawFramebuffer()
        {
            this.OnRenderFrame();

            return true;
        }

        private bool MessageCallback(string message, bool deletePrevious)
        {
            Console.WriteLine(message);

            return true;
        }
    }
}
