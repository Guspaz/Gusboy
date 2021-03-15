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
        private byte rB;
        private byte rC;
        private byte rD;
        private byte rE;
        private byte rH;
        private byte rL;

        // Flags
        private bool fZ;
        private bool fN;
        private bool fH;
        private bool fC;

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
        public ushort rPC { get; set; }

        public ushort rSP { get; set; }

        // 16-bit registers
        private ushort rAF
        {
            get => (ushort)((this.rA << 8) | (this.fZ ? 0b1000_0000 : 0) | (this.fN ? 0b0100_0000 : 0) | (this.fH ? 0b0010_0000 : 0) | (this.fC ? 0b0001_0000 : 0));

            set
            {
                this.rA = (byte)(value >> 8);
                this.fZ = (value & 0b1000_0000) != 0;
                this.fN = (value & 0b0100_0000) != 0;
                this.fH = (value & 0b0010_0000) != 0;
                this.fC = (value & 0b0001_0000) != 0;
            }
        }

        private ushort rBC
        {
            get => (ushort)((this.rB << 8) | this.rC);

            set
            {
                this.rB = (byte)(value >> 8);
                this.rC = (byte)value;
            }
        }

        private ushort rDE
        {
            get => (ushort)((this.rD << 8) | this.rE);

            set
            {
                this.rD = (byte)(value >> 8);
                this.rE = (byte)value;
            }
        }

        private ushort rHL
        {
            get => (ushort)((this.rH << 8) | this.rL);

            set
            {
                this.rH = (byte)(value >> 8);
                this.rL = (byte)value;
            }
        }

        public void InitGbs(byte rA, ushort rSP)
        {
            this.rA = rA;
            this.rSP = rSP;
        }
    }
}
