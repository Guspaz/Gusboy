namespace GusBoy
{
    public partial class CPU
    {
        public const byte INT_VBLANK = (1 << 0);
        public const byte INT_LCDSTAT = (1 << 1);
        public const byte INT_TIMER = (1 << 2);
        public const byte INT_SERIAL = (1 << 3);
        public const byte INT_JOYPAD = (1 << 4);

        const int ADDR_INT_VBLANK = 0x40;
        const int ADDR_INT_LCDSTAT = 0x48;
        const int ADDR_INT_TIMER = 0x50;
        const int ADDR_INT_SERIAL = 0x58;
        const int ADDR_INT_JOYPAD = 0x60;

        public bool fInterruptMasterEnable = false;
        public byte rInterruptEnable = 0;
        public byte rInterruptFlags = 0;

        public void InterruptTick()
        {
            // TODO: Verify if we need to trigger the JOYPAD interrupt if the game wrote to the JOYPD register with different values than the current input

            if ((fHalt || fStop) && (rInterruptEnable & rInterruptFlags & 0x1f) != 0)
            {
                fHalt = false;
                fStop = false;
            }

            if (fInterruptMasterEnable && rInterruptEnable > 0 && rInterruptFlags > 0)
            {
                // Get the set of interrupts that have fired and are enabled
                var fired = rInterruptFlags & rInterruptEnable;

                if ((fired & INT_VBLANK) != 0)
                {
                    FireInterrupt(INT_VBLANK, ADDR_INT_VBLANK);
                    return;
                }

                if ((fired & INT_LCDSTAT) != 0)
                {
                    FireInterrupt(INT_LCDSTAT, ADDR_INT_LCDSTAT);
                    return;
                }

                if ((fired & INT_TIMER) != 0)
                {
                    FireInterrupt(INT_TIMER, ADDR_INT_TIMER);
                    return;
                }

                if ((fired & INT_SERIAL) != 0)
                {
                    FireInterrupt(INT_SERIAL, ADDR_INT_SERIAL);
                    return;
                }

                if ((fired & INT_JOYPAD) != 0)
                {
                    FireInterrupt(INT_JOYPAD, ADDR_INT_JOYPAD);
                    return;
                }
            }
        }

        public void TriggerInterrupt(byte interrupt)
        {
            this.rInterruptFlags |= interrupt;
        }

        private void FireInterrupt(int interrupt, int address)
        {
            // Clear the interrupt flag
            rInterruptFlags = (byte)(rInterruptFlags & ~interrupt);

            // Disable interrupts
            fInterruptMasterEnable = false;

            ram.SetShort(rSP - 2, rPC);
            rPC = address;
            rSP -= 2;

            ticks += 16; // Nintendo says RST instructions are 16
        }
    }
}