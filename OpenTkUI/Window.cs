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
        private const int DESIRED_BUFFER_LENGTH = (SAMPLE_RATE * 2) / 1024; // ~1ms per buffer
        private const int MAX_FRAMEQUEUE_LENGTH = 2;
        private const double DYNAMIC_REFRESH_FACTOR = 1.0001;
        private const int MIN_BUFFER = 50;

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

        // The buffer queues
        private readonly Queue<int[]> screenbuffers = new Queue<int[]>();
        private readonly Queue<int[]> unusedbuffers = new Queue<int[]>();

        // The most recently rendered frame
        private readonly int[] lastFrame = new int[160 * 144];

        private Gameboy gb;
        private GusboyAudio audio;

        private int ticksPerFrame = (int)((1 / 59.7275) * Stopwatch.Frequency); // Doesn't need to be exact because we're going to vary it to keep the buffer full.

        private long frameCount;

        private List<float> frameTimes = new List<float>();
        private long nextRenderTick;

        private BackgroundWorker renderThread = new BackgroundWorker();

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        public override void Run()
        {
            this.OnLoad();
            this.OnResize(new ResizeEventArgs(this.Size));

            // Do some things to try to avoid spikes
            // Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            // Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x4; // Only run on core 3
            // System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;

            this.renderThread.DoWork += this.RenderThread_DoWork;

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

            this.Context.MakeNoneCurrent();

            this.renderThread.RunWorkerAsync();

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
            int audioBufferCount = this.audio.CurrentBufferLatency();

            while (audioBufferCount < MIN_BUFFER)
            {
                this.audio.AddSamples(this.gb.TickForAudio(DESIRED_BUFFER_LENGTH).ToArray());

                audioBufferCount = this.audio.CurrentBufferLatency();
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

        private long lastRenderTick;

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (this.Exists & !this.IsExiting)
            {
                // Default to the most recent frame if there is nothing new
                int[] dequeuedFrame = null;

                if (this.screenbuffers.Count > 0)
                {
                    dequeuedFrame = this.screenbuffers.Dequeue();

                    //if (++this.frameCount == 60)
                    //{
                    //    // This code can go here (measures the emulator output framerate) or in OnRenderFrame (measures the present framerate).
                    //    Console.WriteLine($"Framerate: {Stopwatch.Frequency / (double)this.ticksPerFrame:0.000}");
                    //    this.frameCount = 0;
                    //}

                    long currentTime = Stopwatch.GetTimestamp();

                    this.frameTimes.Add(((currentTime - this.lastRenderTick) / (float)Stopwatch.Frequency) * 1000);
                    this.lastRenderTick = currentTime;

                    if (this.frameTimes.Count == 4000)
                    {
                        File.WriteAllLines("frametimes.txt", this.frameTimes.Select(f => f.ToString()));
                        this.frameTimes.Clear();
                    }
                }
                else
                {
                    // Slow down refresh to increase the buffer
                    this.AdjustDynamicRate(false);
                }

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
                    dequeuedFrame ?? this.lastFrame);

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

                if (dequeuedFrame != null)
                {
                    this.unusedbuffers.Enqueue(dequeuedFrame);
                }
            }
        }

        private void RenderThread_DoWork(object sender, DoWorkEventArgs e)
        {
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            this.MakeCurrent();

            // Initialize
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.Texture2D);
            GL.ClearColor(1, 1, 1, 1);

            this.nextRenderTick = Stopwatch.GetTimestamp();
            this.lastRenderTick = Stopwatch.GetTimestamp();

            while (this.Exists)
            {
                if (Stopwatch.GetTimestamp() >= this.nextRenderTick)
                {
                    this.OnRenderFrame(default);
                    this.nextRenderTick += this.ticksPerFrame;
                }
            }
        }

        private void AdjustDynamicRate(bool increase)
        {
            if (this.gb == null)
            {
                return;
            }

            if (increase)
            {
                // Decrease the time between frames as we're accumulating them to ofast
                this.ticksPerFrame = (int)(this.ticksPerFrame / DYNAMIC_REFRESH_FACTOR);
                Console.WriteLine($"Buffer full, new render target: {this.ticksPerFrame}");
            }
            else if (!increase)
            {
                // Increase the time between frames, as we're consuming frames too fast.
                this.ticksPerFrame = (int)(this.ticksPerFrame * DYNAMIC_REFRESH_FACTOR);
                Console.WriteLine($"Buffer empty, new render target: {this.ticksPerFrame}");
            }
        }

        private void InitGameboy(string filePath)
        {
            if (File.Exists(filePath))
            {
                this.gb = new Gameboy(this.MessageCallback, this.DrawFramebuffer, this.framebuffer, SAMPLE_RATE, filePath);
                //this.lastFrameTick = Stopwatch.GetTimestamp();
                //this.lastRenderTick = Stopwatch.GetTimestamp();
            }
        }

        private bool DrawFramebuffer()
        {
            // If the buffer chain is full, drop the frame
            if (this.screenbuffers.Count < MAX_FRAMEQUEUE_LENGTH)
            {
                int[] copybuffer;

                if (this.unusedbuffers.Count > 0)
                {
                    copybuffer = this.unusedbuffers.Dequeue();
                }
                else
                {
                    copybuffer = new int[this.framebuffer.Length];
                }

                Array.Copy(this.framebuffer, copybuffer, this.framebuffer.Length);
                Array.Copy(this.framebuffer, this.lastFrame, this.framebuffer.Length);
                this.screenbuffers.Enqueue(copybuffer);
            }
            else
            {
                // Speed up to decrease the buffer
                this.AdjustDynamicRate(true);
            }

            return true;
        }

        private bool MessageCallback(string message, bool deletePrevious)
        {
            Console.WriteLine(message);

            return true;
        }
    }
}
