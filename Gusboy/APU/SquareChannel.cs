namespace Gusboy
{
    public class SquareChannel : Channel
    {
        private readonly byte[,] squareDuty = new byte[,]
        {
            { 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 1, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 1, 0 },
        };

        private int volume;

        private int volumeTimer;
        private int sweepTimer;
        private int sweepFrequency;
        private bool sweepEnabled;

        public int InitialVolume { get; set; }

        public int InitialVolumeTimer { get; set; }

        public bool EnvelopeAddMode { get; set; }

        public int Frequency { get; set; }

        public int Duty { get; set; }

        public int DutyStep { get; set; }

        public int InitialSweepTimer { get; set; }

        public bool SweepNegate { get; set; }

        public int SweepShift { get; set; }

        public bool NegateDirty { get; set; } = false;

        protected override int DigitalOutput => this.squareDuty[this.Duty, this.DutyStep] * this.volume;

        protected override int LengthWidth => 64;

        public void Trigger()
        {
            this.ChannelEnable = this.DacEnable;

            if (this.lengthTimer == 0)
            {
                this.lengthTimer = this.LengthWidth;
            }

            // When triggering a square channel, the low two bits of the frequency timer are NOT modified.
            // TODO: Moving to m-cycles may have broken this last-two-bits behaviour? Removed it for now.
            // this.frequencyTimer = (this.frequencyTimer & 0b11) | (((2048 - this.Frequency) * 4) & ~0b11);
            this.frequencyTimer = 2048 - this.Frequency;

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
                this.ChannelEnable = false;
            }

            return newFrequency;
        }

        protected override int FrequencyTimerFire()
        {
            if (++this.DutyStep > 7)
            {
                this.DutyStep = 0;
            }

            return 2048 - this.Frequency; // Removing the * 4 for m-cycle conversion
        }
    }
}
