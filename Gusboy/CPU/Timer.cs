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
        private int rtcClock;

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

        public bool EnableRTC { get; set; }

        public int RtcSubseconds { get; set; }

        public int RtcSeconds { get; set; }

        public int RtcMinutes { get; set; }

        public int RtcHours { get; set; }

        public int RtcDays { get; set; }

        public bool RtcCarry { get; set; }

        public void TimerTick()
        {
            int newCpuTicks = (int)(this.Ticks - this.oldCpuTicks) >> 2;
            this.oldCpuTicks = this.Ticks;

            if (this.EnableRTC)
            {
                // Increment the RTC clock at 2 MHz
                this.rtcClock += this.fSpeed ? newCpuTicks : newCpuTicks << 1;

                // Tick the rtc counter at 32,768 (2097152 / 32768 = 64)
                if (this.rtcClock >= 64)
                {
                    this.rtcClock -= 64;
                    this.RtcSubseconds++;

                    if (this.RtcSubseconds == 32768)
                    {
                        this.RtcSubseconds = 0;
                        this.RtcSeconds++;

                        if (this.RtcSeconds == 64)
                        {
                            this.RtcSeconds = 0;
                        }
                        else if (this.RtcSeconds == 60)
                        {
                            this.RtcSeconds = 0;
                            this.RtcMinutes++;

                            if (this.RtcMinutes == 64)
                            {
                                this.RtcMinutes = 0;
                            }
                            else if (this.RtcMinutes == 60)
                            {
                                this.RtcMinutes = 0;
                                this.RtcHours++;

                                if (this.RtcHours == 32)
                                {
                                    this.RtcHours = 0;
                                }
                                else if (this.RtcHours == 24 )
                                {
                                    this.RtcHours = 0;
                                    this.RtcDays++;

                                    if (this.RtcDays == 512)
                                    {
                                        this.RtcDays = 0;
                                        this.RtcCarry = true;
                                    }
                                }
                            }
                        }
                    }
                }
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