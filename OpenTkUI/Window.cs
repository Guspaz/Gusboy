namespace OpenTkUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using Gusboy;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.Desktop;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    internal class Window : GameWindow
    {
        private const int SAMPLE_RATE = 32768; // This rate is divisible by the gameboy CPU clock and will result in the correct clockspeed.
        private const int DESIRED_BUFFER_LENGTH = (SAMPLE_RATE * 2) / 512; // ~2ms per buffer
        private const int MAX_FRAMEQUEUE_LENGTH = 2;
        private const double MIN_FRAMERATE = 59.25;
        private const double MAX_FRAMERATE = 60.25;
        private const double DYNAMIC_REFRESH_FACTOR = 1.00001;

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

        private readonly Queue<int[]> screenbuffers = new Queue<int[]>();
        private readonly Queue<int[]> unusedbuffers = new Queue<int[]>();

        private readonly BackgroundWorker fillAudioBuffer = new BackgroundWorker();

        // The most recently rendered frame
        private readonly int[] lastFrame = new int[160 * 144];

        private Gameboy gb;
        private GusboyAudio audio;

        private long lastFrameTick;
        private long frameCount;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            this.MakeCurrent();
        }

        protected override void OnLoad()
        {
            var args = Environment.GetCommandLineArgs();

            if (args?.Length == 2)
            {
                this.InitGameboy(args[1]);
            }

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.Texture2D);
            GL.ClearColor(1, 1, 1, 1);

            this.audio = new GusboyAudio(SAMPLE_RATE);

            this.fillAudioBuffer.DoWork += this.FillAudioBuffer_DoWork;

            base.OnLoad();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            // I don't understand why, but OpenAL's ProcessedBuffers doesn't seem to be updated often enough to do this synchronously.
            if (!this.fillAudioBuffer.IsBusy)
            {
                this.fillAudioBuffer.RunWorkerAsync();
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

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            this.frameCount++;

            if (this.frameCount == 600)
            {
                // This code can go here (measures the emulator output framerate) or in OnRenderFrame (measures the present framerate).
                Console.WriteLine($"Framerate: {this.frameCount / ((double)(Stopwatch.GetTimestamp() - this.lastFrameTick) / Stopwatch.Frequency):0.000}");
                this.lastFrameTick = Stopwatch.GetTimestamp();
                this.frameCount = 0;
            }

            // Default to the most recent frame if there is nothing new
            int[] dequeuedFrame = null;

            if (this.screenbuffers.Count > 0)
            {
                dequeuedFrame = this.screenbuffers.Dequeue();
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

        protected override void OnFileDrop(FileDropEventArgs e)
        {
            if (e.FileNames.Length == 1)
            {
                this.InitGameboy(e.FileNames[0]);
            }
        }

        private void AdjustDynamicRate(bool increase)
        {
            if (this.gb == null)
            {
                return;
            }

            if (increase && this.RenderFrequency < MAX_FRAMERATE)
            {
                // Slightly increase the update rate here, as the frames are accumulating too fast
                this.RenderFrequency *= DYNAMIC_REFRESH_FACTOR;
                this.UpdateFrequency = this.RenderFrequency * 2;
                // Console.WriteLine($"Buffer full, new render target: {this.RenderFrequency:0.000}");
            }
            else if (!increase && this.RenderFrequency > MIN_FRAMERATE)
            {
                // Slightly decrease the update rate here, as we're consuming frames too fast.
                this.RenderFrequency /= DYNAMIC_REFRESH_FACTOR;
                this.UpdateFrequency = this.RenderFrequency * 2;
                // Console.WriteLine($"Buffer empty, new render target: {this.RenderFrequency:0.000}");
            }
        }

        private void FillAudioBuffer_DoWork(object sender, DoWorkEventArgs e)
        {
            while (this.audio.HasEmptyBuffers() && this.gb != null)
            {
                while (this.gb.AudioBuffer.Count < DESIRED_BUFFER_LENGTH)
                {
                    this.gb.Tick();
                }

                this.audio.AddSamples(this.gb.AudioBuffer.ToArray());
                this.gb.AudioBuffer.Clear();
            }
        }

        private void InitGameboy(string filePath)
        {
            if (File.Exists(filePath))
            {
                this.gb = new Gameboy(this.MessageCallback, this.DrawFramebuffer, this.framebuffer, SAMPLE_RATE, filePath);
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
