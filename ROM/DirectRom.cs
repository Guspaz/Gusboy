namespace GusBoy
{
    public class DirectRom : Mapper
    {
        public DirectRom(byte[] romFile) : base(romFile, new byte[0], null)
        {
        }

        public override byte Read(int address)
        {
            return address <= 0x7FFF ? romFile[address] : 0xFF;
        }

        public override void Write(int address, byte value)
        {
        }
    }
}
