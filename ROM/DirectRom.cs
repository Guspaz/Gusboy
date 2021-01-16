namespace GusBoy
{
    using System;

    public class DirectRom : Mapper
    {
        public DirectRom(byte[] romFile)
            : base(romFile, Array.Empty<byte>(), null)
        {
        }

        public override byte Read(int address) => address <= 0x7FFF ? this.RomFile[address] : 0xFF;

        public override void Write(int address, byte value)
        {
        }
    }
}
