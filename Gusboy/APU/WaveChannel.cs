﻿namespace Gusboy
{
    using System.Runtime.CompilerServices;

    public class WaveChannel : Channel
    {
        private readonly int[] volumeFactor = { 4, 0, 1, 2 };

        private int wavePosition;

        public int Volume { get; set; }

        public byte[] WaveTable { get; set; } = new byte[16];

        public int SampleBuffer { get; set; }

        public int Frequency { get; set; }

        protected override int DigitalOutput
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.SampleBuffer >> this.volumeFactor[this.Volume];
        }

        protected override int LengthWidth => 256;

        public void Trigger()
        {
            this.ChannelEnable = this.DacEnable;

            if (this.lengthTimer == 0)
            {
                this.lengthTimer = this.LengthWidth;
            }

            // Per Binji: "The trick is to add a 6 cycle delay to the wave period whenever it is triggered." Dunno if this is actually needed.
            this.frequencyTimer = 2048 - this.Frequency; // * 2; // Removed to convert to m-cycles (dividing by two because we double-tick wave)

            this.wavePosition = 0;
        }

        public void FakeBootstrap(int frequencyTimer, int wavePosition)
        {
            this.frequencyTimer = frequencyTimer;
            this.wavePosition = wavePosition;
        }

        protected override int FrequencyTimerFire()
        {
            if (++this.wavePosition > 31)
            {
                this.wavePosition = 0;
            }

            if ((this.wavePosition & 0b0000_0001) == 0)
            {
                this.SampleBuffer = (this.WaveTable[this.wavePosition >> 1] >> 4) & 0b0000_1111;
            }
            else
            {
                this.SampleBuffer = this.WaveTable[this.wavePosition >> 1] & 0b0000_1111;
            }

            return 2048 - this.Frequency; // * 2; // Removed to convert to m-cycles (dividing by two because we double-tick wave)
        }
    }
}
