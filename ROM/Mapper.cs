namespace GusBoy
{
    using System.IO;

    public abstract class Mapper
    {
        private readonly string ramPath;
        private bool ramg;

        public Mapper(byte[] romFile, byte[] sram, string ramPath)
        {
            this.RomFile = romFile;
            this.Sram = sram;
            this.ramPath = ramPath;

            if (sram.Length > 0)
            {
                if (File.Exists(ramPath))
                {
                    sram = File.ReadAllBytes(ramPath);
                }
            }

            this.RomAddressMask = romFile.Length - 1;
            this.RamAddressMask = sram.Length - 1;
        }

        protected int RomAddressMask { get; }

        protected int RamAddressMask { get; }

        protected byte[] RomFile { get; }

        protected byte[] Sram { get; }

        protected bool RAMG
        {
            get => this.ramg;

            set
            {
                if (!value & this.ramg)
                {
                    // RAM access was enabled and is being disabled, dump the RAM to disk
                    this.SaveSRAM();
                }

                this.ramg = value;
            }
        }

        public abstract byte Read(int address);

        public abstract void Write(int address, byte value);

        public void SaveSRAM()
        {
            if (this.Sram?.Length > 0)
            {
                // RAM access was enabled and is being disabled, dump the RAM to disk
                File.WriteAllBytes(this.ramPath, this.Sram);
            }
        }
    }
}
