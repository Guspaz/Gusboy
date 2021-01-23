namespace Gusboy
{
    using System.Runtime.CompilerServices;

    public abstract class Channel
    {
#pragma warning disable SA1401 // There is a large performance impact in debug mode if these are properties
        protected int frequencyTimer;
        protected int lengthTimer;
#pragma warning restore SA1401

        public int LengthLoad
        {
            set => this.lengthTimer = this.LengthWidth - value;
        }

        public bool DacEnable { get; set; }

        public bool ChannelEnable { get; set; }

        public bool LengthEnable { get; set; }

        public bool LeftEnable { get; set; }

        public bool RightEnable { get; set; }

        public double OutputLeft
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (this.ChannelEnable && this.LeftEnable) ? this.DigitalOutput / 100.0 : 0;
        }

        public double OutputRight
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (this.ChannelEnable && this.RightEnable) ? this.DigitalOutput / 100.0 : 0;
        }

        protected abstract int DigitalOutput { get; }

        protected abstract int LengthWidth { get; }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClockTick()
        {
            if (this.frequencyTimer > 0)
            {
                this.frequencyTimer--;
            }

            if (this.frequencyTimer == 0)
            {
                this.frequencyTimer = this.FrequencyTimerFire();
            }
        }

        protected abstract int FrequencyTimerFire();
    }
}
