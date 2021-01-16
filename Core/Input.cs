namespace GusBoy
{
    public class Input
    {
        private const byte JOY10MASK = 0b1101_0000;
        private const byte JOY20MASK = 0b1110_0000;

        private readonly Gameboy gb;

        private int Joy10State = 0b1111;
        private int Joy20State = 0b1111;
        private bool SelectJoy10;
        private bool SelectJoy20;

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

        public Input(Gameboy gameBoy)
        {
            this.gb = gameBoy;
        }

        public byte Read()
        {
            // This function is tediously written but I wanted to simplify the steps
            int result = 0b1111;

            if ( this.SelectJoy10 )
            {
                result &= this.Joy10State;
            }

            if ( this.SelectJoy20 )
            {
                result &= this.Joy20State;
            }

            if ( this.SelectJoy10 )
            {
                result |= JOY10MASK;
            }

            if ( this.SelectJoy20 )
            {
                result |= JOY20MASK;
            }

            return (byte)result;
        }

        public void Write(byte value)
        {
            // Possibly trigger an interrupt?
            this.SelectJoy10 = (value & JOY10MASK) != 0;
            this.SelectJoy20 = (value & JOY20MASK) != 0;
        }

        public void KeyDown(Keys key)
        {
            // Trigger CPU interrupt
            this.gb.cpu.TriggerInterrupt(CPU.INT_JOYPAD);

            switch ( key )
            {
                case Keys.A:
                case Keys.B:
                case Keys.Select:
                case Keys.Start:
                    this.Joy10State &= (int)key;
                    this.Joy10State &= 0b1111;
                    break;
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    this.Joy20State &= ((int)key) >> 4;
                    this.Joy20State &= 0b1111;
                    break;
            }
        }

        public void KeyUp(Keys key)
        {
            switch ( key )
            {
                case Keys.A:
                case Keys.B:
                case Keys.Select:
                case Keys.Start:
                    this.Joy10State |= ~((int)key);
                    this.Joy10State &= 0b1111;
                    break;
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    this.Joy20State |= ~((int)key) >> 4;
                    this.Joy20State &= 0b1111;
                    break;
            }
        }
    }
}
