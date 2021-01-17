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
            while (gb.Apu.Buffer.Count < count >> 2)
            {
                gb.Tick();
            }

            Buffer.BlockCopy(gb.Apu.Buffer.ToArray(), 0, buffer, offset, count);
            gb.Apu.Buffer.Clear();

            return count;
        }
    }
}