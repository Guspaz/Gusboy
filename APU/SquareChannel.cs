namespace GusBoy
{
    public class SquareChannel
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

        public int Frequency;
        public int FrequencyTimer;

        public int Duty;
        public int DutyStep;

        public float OutputLeft => (LengthStatus && LeftEnable) ? (SquareDuty[Duty, DutyStep] * Volume) / 100f : 0;
        public float OutputRight => (LengthStatus && RightEnable) ? (SquareDuty[Duty, DutyStep] * Volume) / 100f : 0;

        public bool LeftEnable;
        public bool RightEnable;

        public int InitialSweepTimer;
        public int SweepTimer;
        public int SweepFrequency;
        public bool SweepEnabled;
        public bool SweepNegate;
        public int SweepShift;
        public bool NegateDirty = false;

        private readonly byte[,] SquareDuty = new byte[,]{
            { 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 1, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 1, 0 }
        };

        public void Trigger()
        {
            this.LengthStatus = this.DacEnable;
            
            if (this.LengthTimer == 0)
            {
                LengthTimer = 64;
            }

            // When triggering a square channel, the low two bits of the frequency timer are NOT modified.
            this.FrequencyTimer = (FrequencyTimer & 0b11) | (((2048 - this.Frequency) * 4) & ~0b11 );

            // Volume/sweep timer treat a period of 0 as 8
            this.VolumeTimer = this.InitialVolumeTimer == 0 ? 8 : this.InitialVolumeTimer;
            this.SweepTimer = this.InitialSweepTimer == 0 ? 8 : this.InitialSweepTimer;

            this.Volume = this.InitialVolume;

            this.SweepFrequency = this.Frequency;

            this.SweepEnabled = this.InitialSweepTimer != 0 || this.SweepShift != 0;

            this.NegateDirty = false;

            if (this.SweepShift != 0)
            {
                SweepCalculation();
            }
        }

        public void ClockTick()
        {
            if (FrequencyTimer > 0)
            {
                FrequencyTimer--;
            }

            if (FrequencyTimer == 0)
            {
                FrequencyTimer = (2048 - this.Frequency) * 4;

                if (++DutyStep > 7)
                {
                    DutyStep = 0;
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

        public void SweepTick()
        {
            if (SweepTimer > 0)
            {
                SweepTimer--;
            }

            if (SweepTimer == 0)
            {
                if (SweepEnabled && InitialSweepTimer != 0)
                {
                    int newFrequency = SweepCalculation();

                    if (newFrequency <= 2047 && SweepShift != 0)
                    {
                        this.SweepFrequency = newFrequency;
                        this.Frequency = newFrequency;

                        SweepCalculation();
                    }
                }

                // Volume/sweep timer treat a period of 0 as 8
                SweepTimer = this.InitialSweepTimer == 0 ? 8 : this.InitialSweepTimer;
            }
        }

        public int SweepCalculation()
        {
            int newFrequency = this.SweepFrequency + (this.SweepFrequency >> this.SweepShift) * (this.SweepNegate ? -1 : 1);

            if (this.SweepNegate)
            {
                this.NegateDirty = true;
            }

            if (newFrequency > 2047 || newFrequency < 0)
            {
                LengthStatus = false;
            }

            return newFrequency;
        }
    }
}
