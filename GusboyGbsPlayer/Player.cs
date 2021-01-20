namespace GusboyGbsPlayer
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using Gusboy;
    using NAudio.Wave;

    /// <summary>
    /// Windows Forms stuff.
    /// </summary>
    public partial class Player : Form
    {
        private const int SAMPLE_RATE = 48000;
        private Gameboy gb;
        private GusboyWaveProvider audioSource;

        private string title;
        private string author;
        private string copyright;
        private int numSongs;
        private int currentSong;

        // We want much larger buffers for an audio player than an emulator
        private WaveOutEvent outputDevice;

        public Player()
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
            if (!this.TitleLbl.IsDisposed)
            {
                var action = new Action<string, bool>((message, deletePrevious) =>
                {
                    string field = message.Substring(0, message.IndexOf(':'));
                    string content = message[(message.IndexOf(':') + 1)..].Trim();

                    switch (field)
                    {
                        case "Title":
                            this.title = content;
                            break;
                        case "Author":
                            this.author = content;
                            break;
                        case "©":
                            this.copyright = content;
                            break;
                        case "Songs":
                            this.numSongs = int.Parse(content);
                            break;
                        case "Playing":
                            this.currentSong = int.Parse(content);
                            break;
                    }

                    this.TitleLbl.Text = $"{this.title}\nBy {this.author}\n©{this.copyright}";
                    this.SongNumbersLbl.Text = $"{this.currentSong}/{this.numSongs}";
                });

                if (this.TitleLbl.InvokeRequired)
                {
                    this.TitleLbl.Invoke(action, message, deletePrevious);
                }
                else
                {
                    action(message, deletePrevious);
                }
            }

            return true;
        }

        // GBS files disable the GPU, but we want to at least not crash if the user starts a real ROM
        private bool DrawFramebuffer() => true;

        private void NextTrackBtn_Click(object sender, EventArgs e)
        {
            if (this.outputDevice.PlaybackState == PlaybackState.Playing)
            {
                this.gb.KeyDown(Input.Keys.Right);
                this.gb.KeyUp(Input.Keys.Right);
            }
        }

        private void PrevTrackBtn_Click(object sender, EventArgs e)
        {
            if (this.outputDevice.PlaybackState == PlaybackState.Playing)
            {
                this.gb.KeyDown(Input.Keys.Left);
                this.gb.KeyUp(Input.Keys.Left);
            }
        }

        private void Player_DragDrop(object sender, DragEventArgs e)
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

                this.gb = new Gameboy(this.AddMessage, this.DrawFramebuffer, null, SAMPLE_RATE, filePath);

                this.audioSource = new GusboyWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, 2), this.gb, this.waveformPainter1, 200);
                this.outputDevice = new WaveOutEvent() { DesiredLatency = 200, NumberOfBuffers = 10 };
                this.outputDevice.Init(this.audioSource);
                this.outputDevice.Play();
            }
        }

        private void Player_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
    }
}
