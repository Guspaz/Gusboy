[assembly: System.Resources.NeutralResourcesLanguageAttribute("en")]

namespace GusBoy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using NAudio.Wave;

    /// <summary>
    /// Main form for the application.
    /// </summary>
    public partial class Gusboy : Form
    {
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

        private readonly BufferedWaveProvider audioBuffer = new BufferedWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(48000, 2)) { DiscardOnBufferOverflow = true };
        private readonly WaveOutEvent outputDevice = new WaveOutEvent() { DesiredLatency = 50, NumberOfBuffers = 10 };

        private readonly Gameboy gb;
        private long frames = -1;
        private long cpuTicks = 0;
        private long clockTicks = 0;
        private readonly DirectBitmap framebuffer = new DirectBitmap(160, 144);

        public Gusboy()
        {
            this.InitializeComponent();

            this.outputDevice.Init(this.audioBuffer);
            this.outputDevice.Play();

            this.gb = new Gameboy(this.AddMessage, this.DrawFramebuffer, this.framebuffer.Bits, 48000);
        }

        public bool AddMessage(string message)
        {
            if (!this.txt_messages.IsDisposed)
            {
                this.txt_messages.AppendText(message + "\r\n");
            }

            return true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) => false;

        private bool DrawFramebuffer()
        {
            this.frames++;

            this.Invalidate(new Rectangle(0, 0, 160 * 2, 144 * 2), false);

            if (this.frames % 120 == 0)
            {
                double timeElapsed = (double)(System.Diagnostics.Stopwatch.GetTimestamp() - this.clockTicks) / System.Diagnostics.Stopwatch.Frequency;
                double framerate = 120 / timeElapsed;
                double clockspeed = (this.gb.Cpu.Ticks - this.cpuTicks) / 1000000.0 / timeElapsed;

                this.statusStrip.Items[0].Text = $"Clockspeed: {clockspeed,5:N} MHz  Framerate: {framerate,2:N} Hz";

                this.cpuTicks = this.gb.Cpu.Ticks;
                this.clockTicks = System.Diagnostics.Stopwatch.GetTimestamp();
            }

            return true;
        }

        private void EmuWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!this.IsDisposed)
            {
                this.gb.Tick();

                if (this.gb.Apu.Buffer.Count >= 100)
                {
                    float[] inBuffer = this.gb.Apu.Buffer.ToArray();
                    byte[] outBuffer = new byte[inBuffer.Length * 4];
                    Buffer.BlockCopy(inBuffer, 0, outBuffer, 0, outBuffer.Length);

                    this.audioBuffer.AddSamples(outBuffer, 0, outBuffer.Length);
                    this.gb.Apu.Buffer.Clear();

                    // Drain the buffer
                    while (this.audioBuffer.BufferedBytes > 4096)
                    {
                    }
                }
            }
        }

        private void Gusboy_Shown(object sender, EventArgs e)
        {
            this.ClientSize = new Size(160 * 2, this.ClientSize.Height);

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            BackgroundWorker emuWorker = new BackgroundWorker();

            emuWorker.DoWork += this.EmuWorker_DoWork;

            emuWorker.RunWorkerAsync();
        }

        private void GusBoy_Paint(object sender, PaintEventArgs e)
        {
            if (this.framebuffer != null)
            {
                e.Graphics.CompositingMode = CompositingMode.SourceCopy;
                e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                e.Graphics.DrawImage(this.framebuffer.Bitmap, 0, 0, 160 * 2, 144 * 2);
            }
        }

        private void GusBoy_KeyDown(object sender, KeyEventArgs e)
        {
            // This event-based input is laggy. Should probably switch to polling using the WinInput class.
            if (this.keymap.ContainsKey(e.KeyCode))
            {
                this.gb.Input.KeyDown(this.keymap[e.KeyCode]);
            }

            e.Handled = true;
        }

        private void GusBoy_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.keymap.ContainsKey(e.KeyCode))
            {
                this.gb.Input.KeyUp(this.keymap[e.KeyCode]);
            }

            e.Handled = true;
        }

        private void GusBoy_FormClosed(object sender, FormClosedEventArgs e) => this.gb.Rom.SaveSRAM();

        public class DirectBitmap : IDisposable
        {
            public DirectBitmap(int width, int height)
            {
                this.Bits = new int[width * height];
                this.BitsHandle = GCHandle.Alloc(this.Bits, GCHandleType.Pinned);
                this.Bitmap = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, this.BitsHandle.AddrOfPinnedObject());
            }

            public Bitmap Bitmap { get; private set; }

            public int[] Bits { get; private set; }

            public bool Disposed { get; private set; }

            protected GCHandle BitsHandle { get; private set; }

            public void Dispose()
            {
                if (this.Disposed)
                {
                    return;
                }

                this.Disposed = true;
                this.Bitmap.Dispose();
                this.BitsHandle.Free();

                GC.SuppressFinalize(this);
            }
        }
    }
}
