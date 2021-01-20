﻿[assembly: System.Resources.NeutralResourcesLanguageAttribute("en")]

namespace Gusboy
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using NAudio.Wave;

    /// <summary>
    /// Main form for the application.
    /// </summary>
    public partial class Gusboy : Form
    {
        private const int SAMPLE_RATE = 48000;

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

        private readonly DirectBitmap framebuffer = new DirectBitmap(160, 144);

        private GusboyWaveProvider audioSource;
        private WaveOutEvent outputDevice;

        private Gameboy gb;

        private long frames = -1;
        private long cpuTicks = 0;
        private long clockTicks = 0;

        public Gusboy()
        {
            this.InitializeComponent();

            var args = Environment.GetCommandLineArgs();

            if (args?.Length == 2)
            {
                this.InitGameboy(args[1]);
            }
        }

        public bool AddMessage(string message, bool deletePrevious = false)
        {
            if (!this.txt_messages.IsDisposed)
            {
                var action = new Action<string, bool>((message, deletePrevious) =>
                {
                    if (deletePrevious)
                    {
                        this.txt_messages.Text = this.txt_messages.Text.Remove(this.txt_messages.Text.LastIndexOf("\r\n"));
                        this.txt_messages.Text = this.txt_messages.Text.Remove(this.txt_messages.Text.LastIndexOf("\r\n"));
                        this.txt_messages.Text += "\r\n";
                    }

                    this.txt_messages.AppendText(message + "\r\n");
                });

                if (this.txt_messages.InvokeRequired)
                {
                    this.txt_messages.Invoke(action, message, deletePrevious);
                }
                else
                {
                    action(message, deletePrevious);
                }
            }

            return true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) => false;

        private bool DrawFramebuffer()
        {
            this.frames++;

            this.Invalidate(new Rectangle(0, 0, 160 * 4, 144 * 4), false);

            Application.DoEvents();

            if (this.frames % 120 == 0)
            {
                double timeElapsed = (double)(System.Diagnostics.Stopwatch.GetTimestamp() - this.clockTicks) / System.Diagnostics.Stopwatch.Frequency;
                double framerate = 120 / timeElapsed;
                double clockspeed = (this.gb.CpuTicks - this.cpuTicks) / 1000000.0 / timeElapsed;

                // Because NAudio might be a different thread, use invoke to touch the control
                if (!this.statusStrip.IsDisposed)
                {
                    this.statusStrip.Invoke(new Action<string>(text => { this.statusStrip.Items[0].Text = text; }), $"Clockspeed: {clockspeed,5:N} MHz  Framerate: {framerate,2:N} Hz");
                }

                this.cpuTicks = this.gb.CpuTicks;
                this.clockTicks = System.Diagnostics.Stopwatch.GetTimestamp();
            }

            return true;
        }

        private void Gusboy_Shown(object sender, EventArgs e)
        {
            this.ClientSize = new Size(160 * 3, (144 * 3) + 200);

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        private void Gusboy_Paint(object sender, PaintEventArgs e)
        {
            if (this.framebuffer != null)
            {
                e.Graphics.CompositingMode = CompositingMode.SourceCopy;
                e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                e.Graphics.DrawImage(this.framebuffer.Bitmap, 0, 0, 160 * 3, 144 * 3);
            }
        }

        private void Gusboy_KeyDown(object sender, KeyEventArgs e)
        {
            // This event-based input is laggy. Should probably switch to polling using the WinInput class.
            if (this.keymap.ContainsKey(e.KeyCode))
            {
                this.gb.KeyDown(this.keymap[e.KeyCode]);
            }

            e.Handled = true;
        }

        private void Gusboy_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.keymap.ContainsKey(e.KeyCode))
            {
                this.gb.KeyUp(this.keymap[e.KeyCode]);
            }

            e.Handled = true;
        }

        private void Gusboy_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Gusboy_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] data && data.Length == 1)
            {
                this.InitGameboy(data[0]);
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

                this.gb = new Gameboy(this.AddMessage, this.DrawFramebuffer, this.framebuffer.Bits, SAMPLE_RATE, filePath);

                this.audioSource = new GusboyWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, 2), this.gb);
                this.outputDevice = new WaveOutEvent() { DesiredLatency = 200, NumberOfBuffers = 10 };
                this.outputDevice.Init(this.audioSource);
                this.outputDevice.Play();
            }
        }

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
