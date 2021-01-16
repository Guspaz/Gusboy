namespace GusBoy
{
    public class SquareChannel
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

        public int Frequency;
        public int FrequencyTimer;

        public int Duty;
        public int DutyStep;

        public float OutputLeft => (this.LengthStatus && this.LeftEnable) ? (this.SquareDuty[this.Duty, this.DutyStep] * this.Volume) / 100f : 0;
        public float OutputRight => (this.LengthStatus && this.RightEnable) ? (this.SquareDuty[this.Duty, this.DutyStep] * this.Volume) / 100f : 0;

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

            if ( this.LengthTimer == 0 )
            {
                this.LengthTimer = 64;
            }

            // When triggering a square channel, the low two bits of the frequency timer are NOT modified.
            this.FrequencyTimer = (this.FrequencyTimer & 0b11) | (((2048 - this.Frequency) * 4) & ~0b11);

            // Volume/sweep timer treat a period of 0 as 8
            this.VolumeTimer = this.InitialVolumeTimer == 0 ? 8 : this.InitialVolumeTimer;
            this.SweepTimer = this.InitialSweepTimer == 0 ? 8 : this.InitialSweepTimer;

            this.Volume = this.InitialVolume;

            this.SweepFrequency = this.Frequency;

            this.SweepEnabled = this.InitialSweepTimer != 0 || this.SweepShift != 0;

            this.NegateDirty = false;

            if ( this.SweepShift != 0 )
            {
                this.SweepCalculation();
            }
        }

        public void ClockTick()
        {
            if ( this.FrequencyTimer > 0 )
            {
                this.FrequencyTimer--;
            }

            if ( this.FrequencyTimer == 0 )
            {
                this.FrequencyTimer = (2048 - this.Frequency) * 4;

                if ( ++this.DutyStep > 7 )
                {
                    this.DutyStep = 0;
                }
            }
        }

        public void LengthTimerTick()
        {
            if ( this.LengthEnable )
            {
                if ( this.LengthTimer > 0 )
                {
                    this.LengthTimer--;
                }

                if ( this.LengthTimer == 0 )
                {
                    this.LengthStatus = false;
                }
            }
        }

        public void VolumeEnvelopeTick()
        {
            if ( this.InitialVolumeTimer > 0 )
            {
                if ( this.VolumeTimer > 0 )
                {
                    this.VolumeTimer--;
                }

                if ( this.VolumeTimer == 0 )
                {
                    if ( this.Volume < 15 && this.EnvelopeAddMode )
                    {
                        this.Volume++;
                    }
                    else if ( this.Volume > 0 && !this.EnvelopeAddMode )
                    {
                        this.Volume--;
                    }

                    // Volume/sweep timer treat a period of 0 as 8
                    this.VolumeTimer = this.InitialVolumeTimer == 0 ? 8 : this.InitialVolumeTimer;
                }
            }
        }

        public void SweepTick()
        {
            if ( this.SweepTimer > 0 )
            {
                this.SweepTimer--;
            }

            if ( this.SweepTimer == 0 )
            {
                if ( this.SweepEnabled && this.InitialSweepTimer != 0 )
                {
                    int newFrequency = this.SweepCalculation();

                    if ( newFrequency <= 2047 && this.SweepShift != 0 )
                    {
                        this.SweepFrequency = newFrequency;
                        this.Frequency = newFrequency;

                        this.SweepCalculation();
                    }
                }

                // Volume/sweep timer treat a period of 0 as 8
                this.SweepTimer = this.InitialSweepTimer == 0 ? 8 : this.InitialSweepTimer;
            }
        }

        public int SweepCalculation()
        {
            int newFrequency = this.SweepFrequency + (this.SweepFrequency >> this.SweepShift) * (this.SweepNegate ? -1 : 1);

            if ( this.SweepNegate )
            {
                this.NegateDirty = true;
            }

            if ( newFrequency > 2047 || newFrequency < 0 )
            {
                this.LengthStatus = false;
            }

            return newFrequency;
        }
    }
}
