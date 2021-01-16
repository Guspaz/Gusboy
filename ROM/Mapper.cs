using System.IO;

namespace GusBoy
{
    public abstract class Mapper
    {
        protected int romAddressMask;
        protected int ramAddressMask;

        protected byte[] romFile;
        protected byte[] sram;

        private readonly string ramPath;

        protected bool RAMG
        {
            get => this._RAMG;

            set
            {
                if ( !value & this._RAMG )
                {
                    // RAM access was enabled and is being disabled, dump the RAM to disk
                    this.SaveSRAM();
                }

                this._RAMG = value;
            }
        }

        private bool _RAMG;

        public Mapper(byte[] romFile, byte[] sram, string ramPath)
        {
            this.romFile = romFile;
            this.sram = sram;
            this.ramPath = ramPath;

            if ( sram.Length > 0 )
            {
                if ( File.Exists(ramPath) )
                {
                    sram = File.ReadAllBytes(ramPath);
                }
            }

            this.romAddressMask = romFile.Length - 1;
            this.ramAddressMask = sram.Length - 1;
        }

        public abstract byte Read(int address);
        public abstract void Write(int address, byte value);

        public void SaveSRAM()
        {
            if ( this.sram?.Length > 0 )
            {
                // RAM access was enabled and is being disabled, dump the RAM to disk
                File.WriteAllBytes(this.ramPath, this.sram);
            }
        }
    }
}
