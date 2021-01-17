namespace Gusboy
{
    /// <summary>
    /// This class is used as the core 16-bit data type in the CPU. It was originally based on UInt16 and was much longer, but it turned out to be a bad design decision.
    /// .NET insists on always turning everything into Int32, so all all the casting to/from UInt16 as well as the overhead of using this struct was quite slow.
    /// I've modified it now to just use int internally, and hope to remove this struct entirely in the future to improve performance.
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
