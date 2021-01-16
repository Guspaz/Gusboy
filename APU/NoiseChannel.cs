namespace Gusboy
{
    public class NoiseChannel
    {
        private readonly int[] divisor = { 8, 16, 32, 48, 64, 80, 96, 112 };

        private int volumeTimer;
        private int lengthTimer;
        private int volume;
        private int rLFSR;
        private int frequencyTimer;

        public int LengthLoad
        {
            set => this.lengthTimer = 64 - value;
        }

        public bool DacEnable { get; set; }

        public bool ChannelEnable { get; set; }

        public bool LengthEnable { get; set; }

        public int InitialVolume { get; set; }

        public int InitialVolumeTimer { get; set; }

        public bool EnvelopeAddMode { get; set; }

        public float OutputLeft => (this.ChannelEnable && this.LeftEnable) ? ((~this.rLFSR & 0b1) * this.volume) / 100f : 0;

        public float OutputRight => (this.ChannelEnable && this.RightEnable) ? ((~this.rLFSR & 0b1) * this.volume) / 100f : 0;

        public bool LeftEnable { get; set; }

        public bool RightEnable { get; set; }

        public bool WidthMode { get; set; }

        public int DivisorCode { get; set; }

        public int ClockShift { get; set; }

        public void Trigger()
        {
            this.ChannelEnable = this.DacEnable;

            if (this.lengthTimer == 0)
            {
                this.lengthTimer = 64;
            }

            this.frequencyTimer = this.divisor[this.DivisorCode] << this.ClockShift;

            // Volume/sweep timer treat a period of 0 as 8
            this.volumeTimer = this.InitialVolumeTimer == 0 ? 8 : this.InitialVolumeTimer;

            this.volume = this.InitialVolume;

            this.rLFSR = 0b0111_1111_1111_1111;
        }

        public void ClockTick()
        {
            if (this.frequencyTimer > 0)
            {
                this.frequencyTimer--;
            }

            // Using a noise channel clock shift of 14 or 15 results in the LFSR receiving no clocks.
            if (this.frequencyTimer == 0 && this.ClockShift <= 15)
            {
                this.frequencyTimer = this.divisor[this.DivisorCode] << this.ClockShift;

                int newHighBit = (this.rLFSR & 0b01) ^ ((this.rLFSR >> 1) & 0b01);

                this.rLFSR = (this.rLFSR >> 1) | (newHighBit << 14);

                if (this.WidthMode)
                {
                    // Should I discard everything above bit 6?
                    this.rLFSR = (this.rLFSR & 0b0111_1111_1011_1111) | newHighBit << 6;
                }
            }
        }

        public void LengthTimerTick()
        {
            if (this.LengthEnable)
            {
                if (this.lengthTimer > 0)
                {
                    this.lengthTimer--;
                }

                if (this.lengthTimer == 0)
                {
                    this.ChannelEnable = false;
                }
            }
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
    }
}
