namespace Gusboy
{
    public abstract class Channel
    {
        public int LengthLoad
        {
            set => this.LengthTimer = this.LengthWidth - value;
        }

        public bool DacEnable { get; set; }

        public bool ChannelEnable { get; set; }

        public bool LengthEnable { get; set; }

        public bool LeftEnable { get; set; }

        public bool RightEnable { get; set; }

        public float OutputLeft => (this.ChannelEnable && this.LeftEnable) ? this.DigitalOutput / 100f : 0;

        public float OutputRight => (this.ChannelEnable && this.RightEnable) ? this.DigitalOutput / 100f : 0;

        protected abstract int DigitalOutput { get; }

        protected int LengthTimer { get; set; }

        protected int FrequencyTimer { get; set; }

        protected abstract int LengthWidth { get; }

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
                    this.ChannelEnable = false;
                }
            }
        }

        public void ClockTick()
        {
            if (this.FrequencyTimer > 0)
            {
                this.FrequencyTimer--;
            }

            if (this.FrequencyTimer == 0)
            {
                this.FrequencyTimer = this.FrequencyTimerFire();
            }
        }

        protected abstract int FrequencyTimerFire();
    }
}
