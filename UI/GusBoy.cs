using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GusBoy
{
    public partial class Gusboy : Form
    {
        public Gusboy()
        {
            InitializeComponent();
        }

        private Gameboy gb;

        public bool AddMessage(string message)
        {
            if (!txt_messages.IsDisposed)
            {
                txt_messages.AppendText(message + "\r\n");
            }

            return true;
        }

        long frames = -1;
        long cpuTicks = 0;
        long clockTicks = 0;

        private bool DrawFramebuffer()
        {
            frames++;
            
            this.Invalidate(new Rectangle(0, 0, 160 * 2, 144 * 2), false);

            Application.DoEvents();

            if (frames % 120 == 0)
            {
                double timeElapsed = (double)(System.Diagnostics.Stopwatch.GetTimestamp() - clockTicks) / System.Diagnostics.Stopwatch.Frequency;
                double framerate = 120 / timeElapsed;
                double clockspeed = (gb.cpu.ticks - cpuTicks) / 1000000.0 / timeElapsed;

                statusStrip.Items[0].Text = $"Clockspeed: {clockspeed,5:N} MHz  Framerate: {framerate,2:N} Hz";

                cpuTicks = gb.cpu.ticks;
                clockTicks = System.Diagnostics.Stopwatch.GetTimestamp();
            }

            return true;
        }

        public class DirectBitmap : IDisposable
        {
            public Bitmap Bitmap { get; private set; }
            public Int32[] Bits { get; private set; }
            public bool Disposed { get; private set; }
            public int Height { get; private set; }
            public int Width { get; private set; }

            protected GCHandle BitsHandle { get; private set; }

            public DirectBitmap(int width, int height)
            {
                Width = width;
                Height = height;
                Bits = new Int32[width * height];
                BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
                Bitmap = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
            }

            public void Dispose()
            {
                if (Disposed) return;
                Disposed = true;
                Bitmap.Dispose();
                BitsHandle.Free();
            }
        }

        private DirectBitmap framebuffer;

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.ClientSize = new Size(160 * 2, this.ClientSize.Height);

            framebuffer = new DirectBitmap(160, 144);

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            gb = new Gameboy(AddMessage, DrawFramebuffer, framebuffer.Bits, 48000);

            var audioBuffer = new BufferedWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(48000, 2)) { DiscardOnBufferOverflow = true };
            var outputDevice = new WaveOutEvent() { DesiredLatency = 50, NumberOfBuffers = 10 };

            outputDevice.Init(audioBuffer);
            outputDevice.Play();

            try
            {

                while (!this.IsDisposed)
                {
                    gb.Tick();

                    if (gb.apu.buffer.Count >= 100)
                    {
                        float[] inBuffer = gb.apu.buffer.ToArray();
                        byte[] outBuffer = new byte[inBuffer.Length * 4];
                        Buffer.BlockCopy(inBuffer, 0, outBuffer, 0, outBuffer.Length);

                        audioBuffer.AddSamples(outBuffer, 0, outBuffer.Length);
                        gb.apu.buffer.Clear();

                        // Drain the buffer
                        while (audioBuffer.BufferedBytes > 4096) { }
                    }
                }
            }
            catch (gbException ex)
            {
                AddMessage(ex.Message);
            }
        }

        private void GusBoy_Paint(object sender, PaintEventArgs e)
        {
            if (framebuffer != null)
            {
                e.Graphics.CompositingMode = CompositingMode.SourceCopy;
                e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                e.Graphics.DrawImage(framebuffer.Bitmap, 0, 0, 160 * 2, 144 * 2);
            }
        }

        Dictionary<Keys, Input.Keys> keymap = new Dictionary<Keys, Input.Keys>
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

        private void GusBoy_KeyDown(object sender, KeyEventArgs e)
        {
            // This event-based input is laggy. Should probably switch to polling using the WinInput class.
            if (keymap.ContainsKey(e.KeyCode))
            {
                gb.input.KeyDown(keymap[e.KeyCode]);
            }

            e.Handled = true;
        }

        private void GusBoy_KeyUp(object sender, KeyEventArgs e)
        {
            if (keymap.ContainsKey(e.KeyCode))
            {
                gb.input.KeyUp(keymap[e.KeyCode]);
            }

            e.Handled = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return false;
        }

        private void GusBoy_FormClosed(object sender, FormClosedEventArgs e)
        {
            gb.rom.SaveSRAM();
        }
    }
}
