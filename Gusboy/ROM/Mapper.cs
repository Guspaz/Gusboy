﻿namespace Gusboy
{
    using System.IO;

    public abstract class Mapper
    {
        private readonly string ramPath;
        private bool ramg;

        public Mapper(byte[] romFile, byte[] sram, string ramPath)
        {
            this.RomFile = romFile;
            this.Sram = sram ?? System.Array.Empty<byte>();
            this.ramPath = ramPath;

            if (this.Sram.Length > 0)
            {
                if (File.Exists(ramPath))
                {
                    File.ReadAllBytes(ramPath).CopyTo(this.Sram, 0);
                }
            }

            this.RomAddressMask = romFile.Length - 1;
            this.RamAddressMask = this.Sram.Length - 1;
        }

        protected int RomAddressMask { get; }

        protected int RamAddressMask { get; }

        protected byte[] RomFile { get; set; }

        protected byte[] Sram { get; }

        protected bool RAMG
        {
            get => this.ramg;

            set
            {
                if (!value & this.ramg)
                {
                    // RAM access was enabled and is being disabled, dump the RAM to disk
                    // TODO: Implement a minimum interval here, some games toggle ramg super rapidly.
                    // this.SaveSRAM();
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
