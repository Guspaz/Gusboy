namespace Gusboy
{
    using System.Runtime.CompilerServices;

    public class NoiseChannel : Channel
    {
        // Converted to m-cycles
        // private readonly int[] divisor = { 8, 16, 32, 48, 64, 80, 96, 112 };
        private readonly int[] divisor = { 2, 4, 8, 12, 16, 20, 24, 28 };

        private int volumeTimer;
        private int volume;
        private int rLFSR;

        public int InitialVolume { get; set; }

        public int InitialVolumeTimer { get; set; }

        public bool EnvelopeAddMode { get; set; }

        public bool WidthMode { get; set; }

        public int DivisorCode { get; set; }

        public int ClockShift { get; set; }

        protected override int DigitalOutput
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (~this.rLFSR & 0b1) * this.volume;
        }

        protected override int LengthWidth => 64;

        public void Trigger()
        {
            this.ChannelEnable = this.DacEnable;

            if (this.lengthTimer == 0)
            {
                this.lengthTimer = this.LengthWidth;
            }

            this.frequencyTimer = this.divisor[this.DivisorCode] << this.ClockShift;

            // Volume/sweep timer treat a period of 0 as 8
            this.volumeTimer = this.InitialVolumeTimer == 0 ? 8 : this.InitialVolumeTimer;

            this.volume = this.InitialVolume;

            this.rLFSR = 0b0111_1111_1111_1111;
        }

        public void VolumeEnvelopeTick()
        {
            if (this.InitialVolumeTimer > 0)
            {
                if (this.volumeTimer > 0)
                {
                    this.volumeTimer--;
                }

                if (this.volumeTimer == 0)
                {
                    if (this.volume < 15 && this.EnvelopeAddMode)
                    {
                        this.volume++;
                    }
                    else if (this.volume > 0 && !this.EnvelopeAddMode)
                    {
                        this.volume--;
                    }

                    // Volume/sweep timer treat a period of 0 as 8
                    this.volumeTimer = this.InitialVolumeTimer == 0 ? 8 : this.InitialVolumeTimer;
                }
            }
        }

        public void FakeBootstrap(int frequencyTimer) => this.frequencyTimer = frequencyTimer;

        protected override int FrequencyTimerFire()
        {
            // Using a noise channel clock shift of 14 or 15 results in the LFSR receiving no clocks.
            if (this.ClockShift <= 15)
            {
                int newHighBit = (this.rLFSR & 0b01) ^ ((this.rLFSR >> 1) & 0b01);

                this.rLFSR = (this.rLFSR >> 1) | (newHighBit << 14);

                if (this.WidthMode)
                {
                    // Should I discard everything above bit 6?
                    this.rLFSR = (this.rLFSR & 0b0111_1111_1011_1111) | newHighBit << 6;
                }
            }

            return this.divisor[this.DivisorCode] << this.ClockShift;
        }
    }
}
