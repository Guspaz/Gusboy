namespace GusBoy
{
    public class NoiseChannel
    {
        public int LengthLoad
        {
            set => this.LengthTimer = 64 - value;
        }

        public bool DacEnable;

        public int LengthTimer;
        public bool LengthStatus;
        public bool LengthEnable;

        public int Volume;
        public int InitialVolume;
        public int InitialVolumeTimer;
        public int VolumeTimer;
        public bool EnvelopeAddMode;

        public float OutputLeft => (this.LengthStatus && this.LeftEnable) ? ((~this.LFSR & 0b1) * this.Volume) / 100f : 0;

        public float OutputRight => (this.LengthStatus && this.RightEnable) ? ((~this.LFSR & 0b1) * this.Volume) / 100f : 0;

        public bool LeftEnable;
        public bool RightEnable;

        private int LFSR;
        public bool WidthMode;

        public int DivisorCode;
        private readonly int[] divisor = { 8, 16, 32, 48, 64, 80, 96, 112 };
        public int ClockShift;
        public int FrequencyTimer;

        public void Trigger()
        {
            this.LengthStatus = this.DacEnable;

            if (this.LengthTimer == 0)
            {
                this.LengthTimer = 64;
            }

            this.FrequencyTimer = this.divisor[this.DivisorCode] << this.ClockShift;

            // Volume/sweep timer treat a period of 0 as 8
            this.VolumeTimer = this.InitialVolumeTimer == 0 ? 8 : this.InitialVolumeTimer;

            this.Volume = this.InitialVolume;

            this.LFSR = 0b0111_1111_1111_1111;
        }

        public void ClockTick()
        {
            if (this.FrequencyTimer > 0)
            {
                this.FrequencyTimer--;
            }

            // Using a noise channel clock shift of 14 or 15 results in the LFSR receiving no clocks.
            if (this.FrequencyTimer == 0 && this.ClockShift <= 15)
            {
                this.FrequencyTimer = this.divisor[this.DivisorCode] << this.ClockShift;

                int newHighBit = (this.LFSR & 0b01) ^ ((this.LFSR >> 1) & 0b01);

                this.LFSR = (this.LFSR >> 1) | (newHighBit << 14);

                if (this.WidthMode)
                {
                    // Should I discard everything above bit 6?
                    this.LFSR = (this.LFSR & 0b0111_1111_1011_1111) | newHighBit << 6;
                }
            }
        }

        public void LengthTimerTick()
        {
            if (this.LengthEnable)
            {
                if (this.LengthTimer > 0)
                {
                    this.LengthTimer--;
                }

                if (this.LengthTimer == 0)
                {
                    this.LengthStatus = false;
                }
            }
        }

        public void VolumeEnvelopeTick()
        {
            if (this.InitialVolumeTimer > 0)
            {
                if (this.VolumeTimer > 0)
                {
                    this.VolumeTimer--;
                }

                if (this.VolumeTimer == 0)
                {
                    if (this.Volume < 15 && this.EnvelopeAddMode)
                    {
                        this.Volume++;
                    }
                    else if (this.Volume > 0 && !this.EnvelopeAddMode)
                    {
                        this.Volume--;
                    }

                    // Volume/sweep timer treat a period of 0 as 8
                    this.VolumeTimer = this.InitialVolumeTimer == 0 ? 8 : this.InitialVolumeTimer;
                }
            }
        }
    }
}
