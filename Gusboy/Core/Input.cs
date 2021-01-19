namespace Gusboy
{
    public class Input
    {
        private const byte JOY10MASK = 0b1101_0000;
        private const byte JOY20MASK = 0b1110_0000;

        private readonly Gameboy gb;

        private int joy10State = 0b1111;
        private int joy20State = 0b1111;
        private bool selectJoy10;
        private bool selectJoy20;

        public Input(Gameboy gameBoy)
        {
            this.gb = gameBoy;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Unecessary")]
        public enum Keys
        {
            A = 0b1110,
            B = 0b1101,
            Select = 0b1011,
            Start = 0b0111,
            Right = 0b1110 << 4,
            Left = 0b1101 << 4,
            Up = 0b1011 << 4,
            Down = 0b0111 << 4,
        }

        public byte Read()
        {
            // This function is tediously written but I wanted to simplify the steps
            int result = 0b1111;

            if (this.selectJoy10)
            {
                result &= this.joy10State;
            }

            if (this.selectJoy20)
            {
                result &= this.joy20State;
            }

            if (this.selectJoy10)
            {
                result |= JOY10MASK;
            }

            if (this.selectJoy20)
            {
                result |= JOY20MASK;
            }

            return (byte)result;
        }

        public void Write(byte value)
        {
            // Possibly trigger an interrupt?
            this.selectJoy10 = (value & JOY10MASK) != 0;
            this.selectJoy20 = (value & JOY20MASK) != 0;
        }

        public void KeyDown(Keys key)
        {
            // Trigger CPU interrupt
            this.gb.Cpu.TriggerInterrupt(CPU.INT_JOYPAD);

            // Control for the GBS player
            if (this.gb.Rom.IsGbs)
            {
                this.gb.Rom.Gbs.KeyDown(key);
            }

            switch (key)
            {
                case Keys.A:
                case Keys.B:
                case Keys.Select:
                case Keys.Start:
                    this.joy10State &= (int)key;
                    this.joy10State &= 0b1111;
                    break;
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    // TODO: Should proably put logic here to stop simultaneous left/right or up/down.
                    this.joy20State &= ((int)key) >> 4;
                    this.joy20State &= 0b1111;
                    break;
            }
        }

        public void KeyUp(Keys key)
        {
            switch (key)
            {
                case Keys.A:
                case Keys.B:
                case Keys.Select:
                case Keys.Start:
                    this.joy10State |= ~((int)key);
                    this.joy10State &= 0b1111;
                    break;
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    this.joy20State |= ~((int)key) >> 4;
                    this.joy20State &= 0b1111;
                    break;
            }
        }
    }
}
