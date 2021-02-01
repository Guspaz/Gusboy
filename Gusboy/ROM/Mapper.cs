namespace Gusboy
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public abstract class Mapper
    {
        private readonly string ramPath;
        private bool ramg;
        private long lastSramSave;
        private bool sramDirty;

        public Mapper(byte[] romFile, byte[] sram, string ramPath)
        {
            this.RomFile = romFile;
            this.Sram = sram ?? System.Array.Empty<byte>();
            this.ramPath = ramPath;

            if (this.Sram.Length > 0)
            {
                if (File.Exists(ramPath) && new FileInfo(ramPath).Length == this.Sram.Length)
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
                    // RAM access was enabled and is being disabled, try to save
                    this.SaveSRAM();
                }

                this.ramg = value;
            }
        }

        public abstract byte Read(int address);

        public abstract void Write(int address, byte value);

        // This function should be called periodically, such as once per vblank
        internal void SaveSRAM()
        {
            // Require at least one second between saving sram to disk
            // TODO: Flag SRAM as dirty on RTC change
            if (this.sramDirty && this.Sram?.Length > 0 && Stopwatch.GetTimestamp() - this.lastSramSave > Stopwatch.Frequency)
            {
                // It's been at least a second, it's OK to save
                byte[] fileToSave = this.Sram;

                if (this is MBC3)
                {
                    fileToSave = fileToSave.Concat((this as MBC3).GetState()).ToArray();
                }

                File.WriteAllBytes(this.ramPath, fileToSave);
                this.sramDirty = false;
                this.lastSramSave = Stopwatch.GetTimestamp();
            }
        }

        protected void DirtySram() => this.sramDirty = true;
    }
}
