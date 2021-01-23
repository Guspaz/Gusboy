namespace Gusboy
{
    /// <summary>
    /// Support for interrupts.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Constants *should* have underscores.")]
    public partial class CPU
    {
        public const byte INT_VBLANK = 1 << 0;
        public const byte INT_LCDSTAT = 1 << 1;
        public const byte INT_TIMER = 1 << 2;
        public const byte INT_SERIAL = 1 << 3;
        public const byte INT_JOYPAD = 1 << 4;
        private const int ADDR_INT_VBLANK = 0x40;
        private const int ADDR_INT_LCDSTAT = 0x48;
        private const int ADDR_INT_TIMER = 0x50;
        private const int ADDR_INT_SERIAL = 0x58;
        private const int ADDR_INT_JOYPAD = 0x60;

        private bool fInterruptMasterEnable;
        private bool fHaltBug;

#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable IDE1006 // Naming Styles
        public byte rInterruptEnable { get; set; }

        public byte rInterruptFlags { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1300 // Element should begin with upper-case letter

        public void InterruptTick()
        {
            // TODO: Pokemon Yellow stalls for a while on the surf bit, possibly interrupts?
            if (this.rInterruptEnable > 0 && this.rInterruptFlags > 0)
            {
                // Get the set of interrupts that have fired and are enabled
                int fired = this.rInterruptFlags & this.rInterruptEnable;

                if ((fired & INT_VBLANK) != 0)
                {
                    this.FireInterrupt(INT_VBLANK, ADDR_INT_VBLANK);
                    return;
                }

                if ((fired & INT_LCDSTAT) != 0)
                {
                    this.FireInterrupt(INT_LCDSTAT, ADDR_INT_LCDSTAT);
                    return;
                }

                if ((fired & INT_TIMER) != 0)
                {
                    this.FireInterrupt(INT_TIMER, ADDR_INT_TIMER);
                    return;
                }

                if ((fired & INT_SERIAL) != 0)
                {
                    this.FireInterrupt(INT_SERIAL, ADDR_INT_SERIAL);
                    return;
                }

                if ((fired & INT_JOYPAD) != 0)
                {
                    this.FireInterrupt(INT_JOYPAD, ADDR_INT_JOYPAD);
                    return;
                }
            }
        }

        public void TriggerInterrupt(byte interrupt) => this.rInterruptFlags |= interrupt;

        private void FireInterrupt(int interrupt, ushort address)
        {
            if (this.fHalt || this.fStop)
            {
                // TODO: Check if stop should also add 4
                this.Ticks += 4;
                this.fStop = false;
                this.fHalt = false;
            }

            if (this.fInterruptMasterEnable)
            {
                // Clear the interrupt flag
                this.rInterruptFlags = (byte)(this.rInterruptFlags & ~interrupt);

                // Disable interrupts
                this.fInterruptMasterEnable = false;

                this.Ram.SetShort(this.rSP - 2, this.rPC);
                this.rPC = address;
                this.rSP -= 2;

                this.Ticks += 20;
            }
        }
    }
}