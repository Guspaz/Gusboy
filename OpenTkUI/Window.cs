namespace OpenTkUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime;
    using Gusboy;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.Desktop;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    internal class Window : GameWindow
    {
        private const int SAMPLE_RATE = 32768; // This rate is divisible by the gameboy CPU clock and will result in the correct clockspeed.
        private const int DESIRED_BUFFER_LENGTH = (SAMPLE_RATE * 2) / (1024 * 8); // ~1ms per buffer / 8
        private const double DYNAMIC_REFRESH_FACTOR = 1.001;
        private const int TARGET_BUFFER = 100;
        private const int TARGET_BUFFER_SAMPLES = (SAMPLE_RATE * TARGET_BUFFER) / 1000;

        private readonly Dictionary<Keys, Input.Keys> keymap = new Dictionary<Keys, Input.Keys>
        {
            { Keys.A, Input.Keys.A },
            { Keys.B, Input.Keys.B },
            { Keys.Space, Input.Keys.Select },
            { Keys.Enter, Input.Keys.Start },
            { Keys.Up, Input.Keys.Up },
            { Keys.Down, Input.Keys.Down },
            { Keys.Left, Input.Keys.Left },
            { Keys.Right, Input.Keys.Right },
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

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            this.MakeCurrent();
        }

        public override void Run()
        {
            this.OnLoad();
            this.OnResize(new ResizeEventArgs(this.Size));

            // Do some things to try to avoid spikes
            // Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            // Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x4; // Only run on core 3
            // System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;

            // Wait for something to initialize the gameboy
            while (this.gb == null)
            {
                this.ProcessEvents();

                if (!this.Exists || this.IsExiting)
                {
                    this.DestroyWindow();
                    return;
                }
            }

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            // Initialize
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.Texture2D);
            GL.ClearColor(1, 1, 1, 1);

            this.nextAudioSampleTime = Stopwatch.GetTimestamp() + (int)this.timeBetweenSamples;

            while (true)
            {
                // TODO: Call this less often, closer to once or twice per frame.
                this.ProcessEvents();

                this.OnUpdateFrame(default);

                if (!this.Exists || this.IsExiting)
                {
                    this.DestroyWindow();
                    return;
                }
            }
        }

        protected override void OnLoad()
        {
            var args = Environment.GetCommandLineArgs();

            if (args?.Length == 2)
            {
                this.InitGameboy(args[1]);
            }

            this.audio = new GusboyAudio(SAMPLE_RATE);

            base.OnLoad();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
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

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (this.IsFocused)
            {
                if (this.keymap.ContainsKey(e.Key))
                {
                    this.gb.KeyDown(this.keymap[e.Key]);
                }
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            if (this.IsFocused)
            {
                if (this.keymap.ContainsKey(e.Key))
                {
                    this.gb.KeyUp(this.keymap[e.Key]);
                }
            }

            base.OnKeyUp(e);
        }

        protected override void OnFileDrop(FileDropEventArgs e)
        {
            if (e.FileNames.Length == 1)
            {
                this.InitGameboy(e.FileNames[0]);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (this.Exists & !this.IsExiting)
            {
                if (++this.frameCount == 60)
                {
                    // This code can go here (measures the emulator output framerate) or in OnRenderFrame (measures the present framerate).
                    Console.WriteLine($"Framerate: {(double)this.frameCount / ((Stopwatch.GetTimestamp() - this.lastFrameTime) / (double)Stopwatch.Frequency):0.000}");
                    this.lastFrameTime = Stopwatch.GetTimestamp();
                    this.frameCount = 0;
                }

                // long currentTime = Stopwatch.GetTimestamp();

                // this.frameTimes.Add(((currentTime - this.lastRenderTick) / (float)Stopwatch.Frequency) * 1000);
                // this.lastRenderTick = currentTime;

                // if (this.frameTimes.Count == 4000)
                // {
                //     File.WriteAllLines("frametimes.txt", this.frameTimes.Select(f => f.ToString()));
                //     this.frameTimes.Clear();
                // }
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                int id = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, id);

                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgb,
                    160,
                    144,
                    0,
                    PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    this.screenbuffer);

                GL.TexParameter(
                    TextureTarget.Texture2D,
                    TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Linear);

                GL.TexParameter(
                    TextureTarget.Texture2D,
                    TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Nearest);

                // This is black magic to me. What does it mean, ProjectPSX?
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 1);
                GL.Vertex2(-1, -1);
                GL.TexCoord2(1, 1);
                GL.Vertex2(1, -1);
                GL.TexCoord2(1, 0);
                GL.Vertex2(1, 1);
                GL.TexCoord2(0, 0);
                GL.Vertex2(-1, 1);
                GL.End();

                GL.DeleteTexture(id);
                this.SwapBuffers();
            }
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

            this.OnRenderFrame(default);

            return true;
        }

        private bool MessageCallback(string message, bool deletePrevious)
        {
            Console.WriteLine(message);

            return true;
        }
    }
}
