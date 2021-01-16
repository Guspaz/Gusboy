namespace GusBoy
{
    public partial class CPU
    {
        // 16-bit registers
        public Gshort rAF = 0x01B0;
        public Gshort rBC = 0x0013;
        public Gshort rDE = 0x00D8;
        public Gshort rHL = 0x014D;
        public Gshort rSP = 0xFFFE;
        public Gshort rPC = 0x0000;

        // 8-bit registers
#pragma warning disable IDE1006 // Naming Styles
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

        public bool fHalt = false;
        public bool fStop = false;
#pragma warning restore IDE1006 // Naming Styles
    }
}