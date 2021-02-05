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
        private const int SAMPLE_RATE = 32768; // This rate is divisible by the gameboy CPU clock and will result in the correct clockspeed.
        private const int DESIRED_BUFFER_LENGTH = (SAMPLE_RATE * 2) / (1024 * 8); // ~1ms per buffer / 8
        private const double DYNAMIC_REFRESH_FACTOR = 1.001;

        // TODO: Figure this out, it's way too long, and was like 3 seconds before.
        private const int TARGET_BUFFER = 1;
        private const int TARGET_BUFFER_SAMPLES = (SAMPLE_RATE * TARGET_BUFFER) / 1000;

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
        private readonly int[] screenbuffer = new int[160 * 144];

        private readonly List<float> frameTimes = new List<float>();

        private readonly Queue<int> audioBufferStatusQueue = new Queue<int>();

        private readonly double baseTimeBetweenSamples = Stopwatch.Frequency / (1024 * 8);
        private double timeBetweenSamples = Stopwatch.Frequency / (1024 * 8);
        private long nextAudioSampleTime;

        private Gameboy gb;
        private GusboyAudio audio;

        private long frameCount;

        private int audioBufferStatusAccumulator;

        private long lastRenderTick;
        private long lastFrameTime;

        private IntPtr window;
        private IntPtr renderer;

        public Window(IntPtr windowPointer, IntPtr rendererPointer)
        {
            this.window = windowPointer;
            this.renderer = rendererPointer;
        }

        public void Run()
        {
            var args = Environment.GetCommandLineArgs();

            if (args?.Length == 2)
            {
                this.InitGameboy(args[1]);
            }

            this.audio = new GusboyAudio(SAMPLE_RATE);

            // Do some things to try to avoid spikes
            // Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            // Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x4; // Only run on core 3
            // System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;

            // Wait for something to initialize the gameboy
            while (this.gb == null)
            {
                this.HandleEvent();
            }

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            this.nextAudioSampleTime = Stopwatch.GetTimestamp() + (int)this.timeBetweenSamples;

            while (true)
            {
                // TODO: Call this less often, closer to once or twice per frame?
                this.HandleEvent();

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
                    // Console.WriteLine($"DEBUG: Unhandled SDL event {sdlevent.window.windowEvent}");
                    break;

                default:
                    // Console.WriteLine($"DEBUG: Unhandled SDL event {sdlevent.type}");
                    break;
            }
        }

        protected void OnUpdateFrame()
        {
            if (Stopwatch.GetTimestamp() >= this.nextAudioSampleTime)
            {
                int audioBufferCount = this.audio.BufferSize();

                this.audioBufferStatusAccumulator += audioBufferCount;
                this.audioBufferStatusQueue.Enqueue(audioBufferCount);

                if (this.audioBufferStatusQueue.Count > 128)
                {
                    this.audioBufferStatusAccumulator -= this.audioBufferStatusQueue.Dequeue();
                }

                float audioBufferStatusAverage = this.audioBufferStatusAccumulator / (float)this.audioBufferStatusQueue.Count;

                this.audio.AddSamples(this.gb.TickForAudio(DESIRED_BUFFER_LENGTH).ToArray());

                // Console.WriteLine($"{audioBufferStatusAverage} - {this.TIME_BETWEEN_SAMPLES}");
                if (audioBufferStatusAverage > TARGET_BUFFER_SAMPLES * 1.1)
                {
                    // Too many samples, increase the time between samples
                    this.timeBetweenSamples = this.baseTimeBetweenSamples * DYNAMIC_REFRESH_FACTOR;
                }
                else if (audioBufferStatusAverage < TARGET_BUFFER_SAMPLES * 0.9)
                {
                    // Too few samples, decrease the time between samples
                    this.timeBetweenSamples = this.baseTimeBetweenSamples / DYNAMIC_REFRESH_FACTOR;
                }
                else
                {
                    this.timeBetweenSamples = this.baseTimeBetweenSamples;
                }

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
            if (++this.frameCount == 60)
            {
                // This code can go here (measures the emulator output framerate) or in OnRenderFrame (measures the present framerate).
                Console.WriteLine($"Framerate: {(double)this.frameCount / ((Stopwatch.GetTimestamp() - this.lastFrameTime) / (double)Stopwatch.Frequency):0.000}");
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

            SDL.SDL_RenderClear(this.renderer);

            var handle = GCHandle.Alloc(this.screenbuffer, GCHandleType.Pinned);
            var surface = SDL.SDL_CreateRGBSurfaceWithFormatFrom(handle.AddrOfPinnedObject(), 160, 144, 32, 4 * 160, SDL.SDL_PIXELFORMAT_ARGB8888);
            var texture = SDL.SDL_CreateTextureFromSurface(this.renderer, surface);

            SDL.SDL_RenderCopy(this.renderer, texture, IntPtr.Zero, IntPtr.Zero);

            SDL.SDL_RenderPresent(this.renderer);

            SDL.SDL_DestroyTexture(texture);
            SDL.SDL_FreeSurface(surface);
            handle.Free();
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
            Array.Copy(this.framebuffer, this.screenbuffer, this.framebuffer.Length);

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
