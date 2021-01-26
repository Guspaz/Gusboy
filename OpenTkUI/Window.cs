namespace OpenTkUI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Gusboy;
    using NAudio.Wave;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.Desktop;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    internal class Window : GameWindow
    {
        private const int SAMPLE_RATE = 47663; // Let NAudio resample, this aligns our 1MHz APU clock to get pretty close to the 59.7275Hz the real hardware needs

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

        private readonly int[] framebuffer = new int[160 * 144];
        private readonly int[] screenbuffer = new int[160 * 144];

        private Gameboy gb;

        private long lastFrameTick;
        private long frameCount;
        private GusboyWaveProvider audioSource;
        private WaveOutEvent outputDevice;

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
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
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

        protected override void OnFileDrop(FileDropEventArgs e)
        {
            if (e.FileNames.Length == 1)
            {
                this.InitGameboy(e.FileNames[0]);
            }
        }

        private void InitGameboy(string filePath)
        {
            if (File.Exists(filePath))
            {
                if (this.outputDevice != null)
                {
                    this.outputDevice.Stop();
                    this.outputDevice.Dispose();
                }

                this.gb = new Gameboy(this.MessageCallback, this.DrawFramebuffer, this.framebuffer, SAMPLE_RATE, filePath);

                this.audioSource = new GusboyWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, 2), this.gb);
                this.outputDevice = new WaveOutEvent() { DesiredLatency = 100, NumberOfBuffers = 50 }; // 2ms buffers
                this.outputDevice.Init(this.audioSource);
                this.outputDevice.Play();
            }
        }

        private bool DrawFramebuffer()
        {
            // Double buffer for smoothness, but we're not going to v-sync unless there is VRR on the monitor
            Array.Copy(this.framebuffer, this.screenbuffer, this.framebuffer.Length);

            this.frameCount++;

            if (this.frameCount == 60)
            {
                // This code can go here (measures the emulator output framerate) or in OnRenderFrame (measures the present framerate).
                // Console.WriteLine($"Framerate: {this.frameCount / ((double)(Stopwatch.GetTimestamp() - this.lastFrameTick) / Stopwatch.Frequency)}");
                this.lastFrameTick = Stopwatch.GetTimestamp();
                this.frameCount = 0;
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
