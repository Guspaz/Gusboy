namespace Gusboy
{
    public class SquareChannel
    {
        private readonly byte[,] squareDuty = new byte[,]
        {
            { 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 1, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 1, 0 },
        };

        private int lengthTimer;
        private int volume;
        private int volumeTimer;
        private int frequencyTimer;
        private int sweepTimer;
        private int sweepFrequency;
        private bool sweepEnabled;

        public int LengthLoad
        {
            set => this.lengthTimer = 64 - value;
        }

        public bool DacEnable { get; set; }

        public bool LengthStatus { get; set; }

        public bool LengthEnable { get; set; }

        public int InitialVolume { get; set; }

        public int InitialVolumeTimer { get; set; }

        public bool EnvelopeAddMode { get; set; }

        public int Frequency { get; set; }

        public int Duty { get; set; }

        public int DutyStep { get; set; }

        public float OutputLeft => (this.LengthStatus && this.LeftEnable) ? (this.squareDuty[this.Duty, this.DutyStep] * this.volume) / 100f : 0;

        public float OutputRight => (this.LengthStatus && this.RightEnable) ? (this.squareDuty[this.Duty, this.DutyStep] * this.volume) / 100f : 0;

        public bool LeftEnable { get; set; }

        public bool RightEnable { get; set; }

        public int InitialSweepTimer { get; set; }

        public bool SweepNegate { get; set; }

        public int SweepShift { get; set; }

        public bool NegateDirty { get; set; } = false;

        public void Trigger()
        {
            this.LengthStatus = this.DacEnable;

            if (this.lengthTimer == 0)
            {
                this.lengthTimer = 64;
            }

            // When triggering a square channel, the low two bits of the frequency timer are NOT modified.
            this.frequencyTimer = (this.frequencyTimer & 0b11) | (((2048 - this.Frequency) * 4) & ~0b11);

            // Volume/sweep timer treat a period of 0 as 8
            this.volumeTimer = this.InitialVolumeTimer == 0 ? 8 : this.InitialVolumeTimer;
            this.sweepTimer = this.InitialSweepTimer == 0 ? 8 : this.InitialSweepTimer;

            this.volume = this.InitialVolume;

            this.sweepFrequency = this.Frequency;

            this.sweepEnabled = this.InitialSweepTimer != 0 || this.SweepShift != 0;

            this.NegateDirty = false;

            if (this.SweepShift != 0)
            {
                this.SweepCalculation();
            }
        }

        public void ClockTick()
        {
            if (this.frequencyTimer > 0)
            {
                this.frequencyTimer--;
            }

            if (this.frequencyTimer == 0)
            {
                this.frequencyTimer = (2048 - this.Frequency) * 4;

                if (++this.DutyStep > 7)
                {
                    this.DutyStep = 0;
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
                    this.LengthStatus = false;
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

        public void SweepTick()
        {
            if (this.sweepTimer > 0)
            {
                this.sweepTimer--;
            }

            if (this.sweepTimer == 0)
            {
                if (this.sweepEnabled && this.InitialSweepTimer != 0)
                {
                    int newFrequency = this.SweepCalculation();

                    if (newFrequency <= 2047 && this.SweepShift != 0)
                    {
                        this.sweepFrequency = newFrequency;
                        this.Frequency = newFrequency;

                        this.SweepCalculation();
                    }
                }

                // Volume/sweep timer treat a period of 0 as 8
                this.sweepTimer = this.InitialSweepTimer == 0 ? 8 : this.InitialSweepTimer;
            }
        }

        public int SweepCalculation()
        {
            int newFrequency = this.sweepFrequency + ((this.sweepFrequency >> this.SweepShift) * (this.SweepNegate ? -1 : 1));

            if (this.SweepNegate)
            {
                this.NegateDirty = true;
            }

            if (newFrequency > 2047 || newFrequency < 0)
            {
                this.LengthStatus = false;
            }

            return newFrequency;
        }
    }
}
