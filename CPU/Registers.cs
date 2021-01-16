namespace GusBoy
{
    public partial class CPU
    {
        // 16-bit registers
        public gshort rAF = 0x01B0;
        public gshort rBC = 0x0013;
        public gshort rDE = 0x00D8;
        public gshort rHL = 0x014D;
        public gshort rSP = 0xFFFE;
        public gshort rPC = 0x0000;

        // 8-bit registers
        public byte rA
        {
            get
            {
                return rAF.Hi;
            }
            set
            {
                rAF.Hi = value;
            }
        }

        public byte rF
        {
            get
            {
                return rAF.Lo;
            }
            set
            {
                rAF.Lo = value;
            }
        }

        public byte rB
        {
            get
            {
                return rBC.Hi;
            }
            set
            {
                rBC.Hi = value;
            }
        }

        public byte rC
        {
            get
            {
                return rBC.Lo;
            }
            set
            {
                rBC.Lo = value;
            }
        }

        public byte rD
        {
            get
            {
                return rDE.Hi;
            }
            set
            {
                rDE.Hi = value;
            }
        }

        public byte rE
        {
            get
            {
                return rDE.Lo;
            }
            set
            {
                rDE.Lo = value;
            }
        }

        public byte rH
        {
            get
            {
                return rHL.Hi;
            }
            set
            {
                rHL.Hi = value;
            }
        }

        public byte rL
        {
            get
            {
                return rHL.Lo;
            }
            set
            {
                rHL.Lo = value;
            }
        }

        // Flags
        public bool fZ
        {
            get
            {
                return rAF[7];
                
            }
            set
            {
                rAF[7] = value;
            }
        }

        public bool fN
        {
            get
            {
                return rAF[6];
            }
            set
            {
                rAF[6] = value;
            }
        }

        public bool fH
        {
            get
            {
                return rAF[5];
            }
            set
            {
                rAF[5] = value;
            }
        }

        public bool fC
        {
            get
            {
                return rAF[4];
            }
            set
            {
                rAF[4] = value;
            }
        }

        public bool fHalt = false;
        public bool fStop = false;
    }
}