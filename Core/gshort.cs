namespace GusBoy
{
    /// <summary>
    /// Basically just a ushort with access to the hi and lo bytes and some extra implicit conversions.
    /// Changed to treat it more like int because .NET insists on converting everything to int when you do operations on stuff.
    /// </summary>
    public struct Gshort
    {
        public int Var;

        public Gshort(int u)
        {
            this.Var = u;
        }

        public byte Lo
        {
            get => (byte)(this.Var & 0x00FF);
            set => this.Var = (this.Var & 0xFF00) | value;
        }

        public byte Hi
        {
            get => (byte)(this.Var >> 8);
            set => this.Var = (this.Var & 0x00FF) | (value << 8);
        }

        public bool this[int i]
        {
            get => (this.Var & (1 << i)) != 0;
            set
            {
                if (value)
                {
                    this.Var |= 1 << i;
                }
                else
                {
                    this.Var &= ~(1 << i);
                }
            }
        }

        public static implicit operator Gshort(int i)
        {
            return new Gshort(i);
        }

        public static implicit operator int(Gshort a)
        {
            return a.Var;
        }

        public static Gshort operator ++(Gshort a)
        {
            a.Var++;

            return a;
        }
    }
}
