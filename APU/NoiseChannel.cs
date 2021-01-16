namespace GusBoy
{
    public class NoiseChannel
    {
        public int LengthLoad
        {
            set
            {
                LengthTimer = 64 - value;
            }
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

        public float OutputLeft => (LengthStatus && LeftEnable) ? ((~LFSR & 0b1) * Volume) / 100f : 0;
        public float OutputRight => (LengthStatus && RightEnable) ? ((~LFSR & 0b1) * Volume) / 100f : 0;

        public bool LeftEnable;
        public bool RightEnable;

        private int LFSR;
        public bool WidthMode;

        public int DivisorCode;
        private int[] divisor = { 8, 16, 32, 48, 64, 80, 96, 112 };
        public int ClockShift;
        public int FrequencyTimer;

        public void Trigger()
        {
            this.LengthStatus = this.DacEnable;

            if (this.LengthTimer == 0)
            {
                this.LengthTimer = 64;
            }

            FrequencyTimer = divisor[DivisorCode] << ClockShift;

            // Volume/sweep timer treat a period of 0 as 8
            this.VolumeTimer = this.InitialVolumeTimer == 0 ? 8 : this.InitialVolumeTimer;

            this.Volume = InitialVolume;

            this.LFSR = 0b0111_1111_1111_1111;
        }

        public void ClockTick()
        {
            if (FrequencyTimer > 0)
            {
                FrequencyTimer--;
            }

            // Using a noise channel clock shift of 14 or 15 results in the LFSR receiving no clocks.
            if (FrequencyTimer == 0 && ClockShift <= 15)
            {
                FrequencyTimer = divisor[DivisorCode] << (ClockShift);

                int newHighBit = (LFSR & 0b01) ^ ((LFSR >> 1) & 0b01);

                LFSR = LFSR >> 1;
                LFSR = LFSR | (newHighBit << 14);

                if (WidthMode)
                {
                    // Should I discard everything above bit 6?
                    LFSR = (LFSR & 0b0111_1111_1011_1111) | newHighBit << 6;
                }
            }
        }

        public void LengthTimerTick()
        {
            if (LengthEnable)
            {
                if (LengthTimer > 0)
                {
                    LengthTimer--;
                }

                if (LengthTimer == 0)
                {
                    LengthStatus = false;
                }
            }
        }

        public void VolumeEnvelopeTick()
        {
            if (InitialVolumeTimer > 0)
            {
                if (VolumeTimer > 0)
                {
                    VolumeTimer--;
                }

                if (VolumeTimer == 0)
                {
                    if (Volume < 15 && EnvelopeAddMode)
                    {
                        Volume++;
                    }
                    else if (Volume > 0 && !EnvelopeAddMode)
                    {
                        Volume--;
                    }

                    // Volume/sweep timer treat a period of 0 as 8
                    this.VolumeTimer = this.InitialVolumeTimer == 0 ? 8 : this.InitialVolumeTimer;
                }
            }
        }
    }
}
