namespace Gusboy
{
    /// <summary>
    /// Implemented from reference doc at https://hacktix.github.io/GBEDG/timers/
    /// Implements the CPU timer.
    /// </summary>
    public partial class CPU
    {
        private long oldCpuTicks;
        private bool oldAndResult;
        private bool reloadingTima;
        private uint rtcMultiplier;
        private bool rtcSupported;

#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable IDE1006 // Naming Styles
        public int rDIV { get; set; }

        public byte rTIMA { get; set; }

        public byte rTMA { get; set; }

        public byte rTAC { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1300 // Element should begin with upper-case letter

        public bool ReloadingTima
        {
            get => this.reloadingTima;
            set => this.reloadingTima = value;
        }

        public bool RtcTimerActive
        {
            get => this.rtcMultiplier > 0;
            set => this.rtcMultiplier = value ? this.fSpeedTimerMultiplier : 0;
        }

        public bool RtcSupported { set => this.rtcSupported = value; }

        public RtcCounter RtcCounter { get; } = new RtcCounter();

        public void TimerTick()
        {
            uint newCpuTicks = (uint)(this.Ticks - this.oldCpuTicks) >> 2;
            this.oldCpuTicks = this.Ticks;

            if (this.rtcSupported)
            {
                // Increment the RTC clock at 2 MHz
                this.RtcCounter.AddTicks(newCpuTicks * this.rtcMultiplier);
            }

            // It takes one full M-Cycle to reload TIMA so we've divided by four
            for (int i = 0; i < newCpuTicks; i++)
            {
                this.rDIV += 4;

                if (this.reloadingTima)
                {
                    // Reload TIMA
                    this.rTIMA = this.rTMA;

                    // Fire the timer interrupt
                    this.TriggerInterrupt(INT_TIMER);

                    // Reset the state
                    this.reloadingTima = false;
                }

                bool divBit = false;

                switch (this.rTAC & 0b011)
                {
                    case 0b00:
                        divBit = (this.rDIV & 0b10_0000_0000) != 0;
                        break;
                    case 0b01:
                        divBit = (this.rDIV & 0b1000) != 0;
                        break;
                    case 0b10:
                        divBit = (this.rDIV & 0b10_0000) != 0;
                        break;
                    case 0b11:
                        divBit = (this.rDIV & 0b1000_0000) != 0;
                        break;
                }

                bool timerEnableBit = (this.rTAC & 0b100) != 0;

                bool andResult = divBit && timerEnableBit;

                if (this.oldAndResult && !andResult)
                {
                    if (this.rTIMA == 255)
                    {
                        // Overflow is about to happen
                        this.rTIMA = 0;
                        this.ReloadingTima = true;
                    }
                    else
                    {
                        // No overflow
                        this.rTIMA++;
                    }
                }

                this.oldAndResult = andResult;
            }
        }
    }
}