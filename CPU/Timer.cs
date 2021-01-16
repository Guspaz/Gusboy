namespace GusBoy
{
    public partial class CPU
    {
        // Implemented from reference doc at https://hacktix.github.io/GBEDG/timers/

        public Gshort rDIV = 0;
        public byte rTIMA = 0;
        public byte rTMA = 0;
        public byte rTAC = 0;

        public bool reloadingTima;

        private long oldCpuTicks;
        private bool oldAndResult;

        private long reloadTima;

        public void TimerTick()
        {
            long newCpuTicks = this.ticks - this.oldCpuTicks;
            this.oldCpuTicks = this.ticks;

            for ( int i = 0; i < newCpuTicks; i++ )
            {
                this.rDIV++;

                if ( this.reloadingTima && --this.reloadTima == 0 )
                {
                    // Reload TIMA
                    this.rTIMA = this.rTMA;

                    // Fire the timer interrupt
                    this.TriggerInterrupt(INT_TIMER);

                    // Reset the state
                    this.reloadingTima = false;
                    this.reloadTima = -1;
                }

                bool divBit = false;

                switch ( this.rTAC & 0b011 )
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

                if ( this.oldAndResult && !andResult )
                {
                    if ( this.rTIMA == 255 )
                    {
                        // Overflow is about to happen
                        this.rTIMA = 0;
                        this.reloadTima = 4;
                        this.reloadingTima = true;
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