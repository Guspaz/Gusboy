namespace Gusboy
{
    using System;
    using NAudio.Wave;

    /// <summary>
    /// Uses NAudio to directly drive emulation speed.
    /// </summary>
    public class GusboyWaveProvider : IWaveProvider
    {
        private readonly WaveFormat waveFormat;
        private readonly Gameboy gb;

        public GusboyWaveProvider(WaveFormat waveFormat, Gameboy gb)
        {
            this.waveFormat = waveFormat;
            this.gb = gb;
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

            Buffer.BlockCopy(this.gb.AudioBuffer.ToArray(), 0, buffer, offset, count);
            this.gb.AudioBuffer.Clear();

            return count;
        }
    }
}