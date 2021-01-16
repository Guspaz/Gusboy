namespace GusBoy
{
    public partial class CPU
    {
        // Implemented from reference doc at https://hacktix.github.io/GBEDG/timers/

        public gshort rDIV = 0;
        public byte rTIMA = 0;
        public byte rTMA = 0;
        public byte rTAC = 0;

        public bool reloadingTima;

        private long oldCpuTicks;
        private bool oldAndResult;

        private long reloadTima;

        public void TimerTick()
        {
            long newCpuTicks = this.ticks - oldCpuTicks;
            oldCpuTicks = this.ticks;

            for (int i = 0; i < newCpuTicks; i++)
            {
                rDIV++;

                if (reloadingTima && --reloadTima == 0)
                {
                    // Reload TIMA
                    rTIMA = rTMA;

                    // Fire the timer interrupt
                    TriggerInterrupt(INT_TIMER);

                    // Reset the state
                    reloadingTima = false;
                    reloadTima = -1;
                }

                bool divBit = false;

                switch (rTAC & 0b011)
                {
                    case 0b00:
                        divBit = (rDIV & 0b10_0000_0000) != 0;
                        break;
                    case 0b01:
                        divBit = (rDIV & 0b1000) != 0;
                        break;
                    case 0b10:
                        divBit = (rDIV & 0b10_0000) != 0;
                        break;
                    case 0b11:
                        divBit = (rDIV & 0b1000_0000) != 0;
                        break;
                }

                bool timerEnableBit = (rTAC & 0b100) != 0;

                bool andResult = divBit && timerEnableBit;

                if (oldAndResult && !andResult)
                {
                    if (rTIMA == 255)
                    {
                        // Overflow is about to happen
                        rTIMA = 0;
                        reloadTima = 4;
                        reloadingTima = true;
                    }
                    else
                    {
                        // No overflow
                        rTIMA++;
                    }
                }

                oldAndResult = andResult;
            }
        }
    }
}