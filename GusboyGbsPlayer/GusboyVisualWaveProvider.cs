namespace Gusboy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NAudio.Gui;
    using NAudio.Wave;

    /// <summary>
    /// Uses NAudio to directly drive emulation speed.
    /// </summary>
    public class GusboyVisualWaveProvider : IWaveProvider
    {
        private readonly WaveFormat waveFormat;
        private readonly Gameboy gb;
        private readonly WaveformPainter painter;
        private readonly Queue<float> sampleQueue = new Queue<float>();

        private readonly int bufferSkip;
        private readonly int queueLength;

        public GusboyVisualWaveProvider(WaveFormat waveFormat, Gameboy gb, WaveformPainter painter, int bufferLength)
        {
            this.waveFormat = waveFormat;
            this.gb = gb;
            this.painter = painter;

            // We can assume stereo (since the emulator doesn't support mono)
            this.bufferSkip = waveFormat.SampleRate * 2 / 60;
            this.queueLength = bufferLength / 500 * waveFormat.SampleRate / this.bufferSkip;
        }

        public WaveFormat WaveFormat
        {
            get { return this.waveFormat; }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            while (this.gb.AudioBuffer.Count < count >> 2)
            {
                this.gb.Tick();
            }

            foreach (float sample in this.gb.AudioBuffer.Where((x, i) => i % this.bufferSkip == 0))
            {
                // Use a queue to delay the sample display to line up with the audio lag
                this.sampleQueue.Enqueue(sample);

                if (this.sampleQueue.Count > this.queueLength)
                {
                    this.painter.AddMax(this.sampleQueue.Dequeue() * 3);
                }
            }

            Buffer.BlockCopy(this.gb.AudioBuffer.ToArray(), 0, buffer, offset, count);
            this.gb.AudioBuffer.Clear();

            return count;
        }
    }
}