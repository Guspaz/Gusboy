namespace GusBoy
{
    /// <summary>
    /// General registers that don't belong in another category.
    /// </summary>
    public partial class CPU
    {
#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable IDE1006 // Naming Styles

        // 16-bit registers
        private Gshort rAF = 0x01B0;
        private Gshort rBC = 0x0013;
        private Gshort rDE = 0x00D8;
        private Gshort rHL = 0x014D;
        private Gshort rSP = 0xFFFE;

        public bool fHalt { get; set; }

        public bool fStop { get; set; }

        public int rPC { get; set; }

        // 8-bit registers
        public byte rA
        {
            get => this.rAF.Hi;
            set => this.rAF.Hi = value;
        }

        public byte rF
        {
            get => this.rAF.Lo;
            set => this.rAF.Lo = value;
        }

        public byte rB
        {
            get => this.rBC.Hi;
            set => this.rBC.Hi = value;
        }

        public byte rC
        {
            get => this.rBC.Lo;
            set => this.rBC.Lo = value;
        }

        public byte rD
        {
            get => this.rDE.Hi;
            set => this.rDE.Hi = value;
        }

        public byte rE
        {
            get => this.rDE.Lo;
            set => this.rDE.Lo = value;
        }

        public byte rH
        {
            get => this.rHL.Hi;
            set => this.rHL.Hi = value;
        }

        public byte rL
        {
            get => this.rHL.Lo;
            set => this.rHL.Lo = value;
        }

        // Flags
        public bool fZ
        {
            get => this.rAF[7];
            set => this.rAF[7] = value;
        }

        public bool fN
        {
            get => this.rAF[6];
            set => this.rAF[6] = value;
        }

        public bool fH
        {
            get => this.rAF[5];
            set => this.rAF[5] = value;
        }

        public bool fC
        {
            get => this.rAF[4];
            set => this.rAF[4] = value;
        }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1300 // Element should begin with upper-case letter
    }
}