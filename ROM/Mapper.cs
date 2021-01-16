using System;
using System.IO;

namespace GusBoy
{
    public abstract class Mapper
    {
        protected int romAddressMask;
        protected int ramAddressMask;

        protected byte[] romFile;
        protected byte[] sram;

        private string ramPath;

        protected bool RAMG
        {
            get => _RAMG;

            set
            {
                if (!value & _RAMG)
                {
                    // RAM access was enabled and is being disabled, dump the RAM to disk
                    SaveSRAM();
                }

                _RAMG = value;
            }
        }

        private bool _RAMG;

        public Mapper(byte[] romFile, byte[] sram, string ramPath)
        {
            this.romFile = romFile;
            this.sram = sram;
            this.ramPath = ramPath;

            if (sram.Length > 0)
            {
                if (File.Exists(ramPath))
                {
                    sram = File.ReadAllBytes(ramPath);
                }
            }

            romAddressMask = romFile.Length - 1;
            ramAddressMask = sram.Length - 1;
        }

        public abstract byte Read(int address);
        public abstract void Write(int address, byte value);

        public void SaveSRAM()
        {
            if (sram?.Length > 0)
            {
                // RAM access was enabled and is being disabled, dump the RAM to disk
                File.WriteAllBytes(ramPath, sram);
            }
        }
    }
}
