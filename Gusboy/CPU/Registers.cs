namespace Gusboy
{
    /// <summary>
    /// General registers that don't belong in another category.
    /// </summary>
    public partial class CPU
    {
#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable IDE1006 // Naming Styles

        // 8-bit registers
        private byte rA;
        private byte rF;
        private byte rB;
        private byte rC;
        private byte rD;
        private byte rE;
        private byte rH;
        private byte rL;

        // Internal flags
        private bool fSpeedInternal;

        // Flags
        public bool fHalt { get; set; }

        public bool fStop { get; set; }

        public bool fSpeed
        {
            get => this.fSpeedInternal;
            set
            {
                this.fSpeedInternal = value;
                this.fSpeedTimerMultiplier = this.fSpeed ? 1 : 2;
            }
        }

        public uint fSpeedTimerMultiplier { get; set; } = 2;

        public bool fPrepareSwitch { get; set; }

        // 16-bit registers
        public int rPC { get; set; }

        public int rSP { get; set; }

        // 16-bit registers
        private int rAF
        {
            get => (this.rA << 8) | this.rF;

            set
            {
                this.rA = (byte)(value >> 8);
                this.rF = (byte)value;
            }
        }

        private int rBC
        {
            get => (this.rB << 8) | this.rC;

            set
            {
                this.rB = (byte)(value >> 8);
                this.rC = (byte)value;
            }
        }

        private int rDE
        {
            get => (this.rD << 8) | this.rE;

            set
            {
                this.rD = (byte)(value >> 8);
                this.rE = (byte)value;
            }
        }

        private int rHL
        {
            get => (this.rH << 8) | this.rL;

            set
            {
                this.rH = (byte)(value >> 8);
                this.rL = (byte)value;
            }
        }

        // Flags
        private bool fZ
        {
            get => this.GetFlag(7);
            set => this.SetFlag(value, 7);
        }

        private bool fN
        {
            get => this.GetFlag(6);
            set => this.SetFlag(value, 6);
        }

        private bool fH
        {
            get => this.GetFlag(5);
            set => this.SetFlag(value, 5);
        }

        private bool fC
        {
            get => this.GetFlag(4);
            set => this.SetFlag(value, 4);
        }

        public void InitGbs(byte rA, int rSP)
        {
            this.rA = rA;
            this.rSP = rSP;
        }

        private bool GetFlag(int i) => (this.rF & (1 << i)) != 0;

        private void SetFlag(bool value, int i)
        {
            if (value)
            {
                this.rF |= (byte)(1 << i);
            }
            else
            {
                this.rF &= (byte)(~(1 << i));
            }
        }
    }
}