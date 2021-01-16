namespace GusBoy
{
    public partial class CPU
    {
        public const byte INT_VBLANK = (1 << 0);
        public const byte INT_LCDSTAT = (1 << 1);
        public const byte INT_TIMER = (1 << 2);
        public const byte INT_SERIAL = (1 << 3);
        public const byte INT_JOYPAD = (1 << 4);
        private const int ADDR_INT_VBLANK = 0x40;
        private const int ADDR_INT_LCDSTAT = 0x48;
        private const int ADDR_INT_TIMER = 0x50;
        private const int ADDR_INT_SERIAL = 0x58;
        private const int ADDR_INT_JOYPAD = 0x60;

        public bool fInterruptMasterEnable = false;
        public byte rInterruptEnable = 0;
        public byte rInterruptFlags = 0;

        public void InterruptTick()
        {
            // TODO: Verify if we need to trigger the JOYPAD interrupt if the game wrote to the JOYPD register with different values than the current input

            if ( (this.fHalt || this.fStop) && (this.rInterruptEnable & this.rInterruptFlags & 0x1f) != 0 )
            {
                this.fHalt = false;
                this.fStop = false;
            }

            if ( this.fInterruptMasterEnable && this.rInterruptEnable > 0 && this.rInterruptFlags > 0 )
            {
                // Get the set of interrupts that have fired and are enabled
                int fired = this.rInterruptFlags & this.rInterruptEnable;

                if ( (fired & INT_VBLANK) != 0 )
                {
                    this.FireInterrupt(INT_VBLANK, ADDR_INT_VBLANK);
                    return;
                }

                if ( (fired & INT_LCDSTAT) != 0 )
                {
                    this.FireInterrupt(INT_LCDSTAT, ADDR_INT_LCDSTAT);
                    return;
                }

                if ( (fired & INT_TIMER) != 0 )
                {
                    this.FireInterrupt(INT_TIMER, ADDR_INT_TIMER);
                    return;
                }

                if ( (fired & INT_SERIAL) != 0 )
                {
                    this.FireInterrupt(INT_SERIAL, ADDR_INT_SERIAL);
                    return;
                }

                if ( (fired & INT_JOYPAD) != 0 )
                {
                    this.FireInterrupt(INT_JOYPAD, ADDR_INT_JOYPAD);
                    return;
                }
            }
        }

        public void TriggerInterrupt(byte interrupt) => this.rInterruptFlags |= interrupt;

        private void FireInterrupt(int interrupt, int address)
        {
            // Clear the interrupt flag
            this.rInterruptFlags = (byte)(this.rInterruptFlags & ~interrupt);

            // Disable interrupts
            this.fInterruptMasterEnable = false;

            this.ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = address;
            this.rSP -= 2;

            this.ticks += 16; // Nintendo says RST instructions are 16
        }
    }
}