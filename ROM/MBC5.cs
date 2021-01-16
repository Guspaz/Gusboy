namespace GusBoy
{
    public class MBC5 : Mapper
    {
        private byte rROMB0 = 1;
        private byte rROMB1 = 0;
        private byte rRAMB = 0;

        public MBC5(byte[] romFile, byte[] sram, string ramPath)
            : base(romFile, sram, ramPath)
        {
        }

        public override byte Read(int address)
        {
            if (address <= 0x3FFF)
            {
                // Only the first 14 bits of the address are wired up to the ROM chips
                address &= 0b0011_1111_1111_1111;

                return this.RomFile[address & this.RomAddressMask];
            }
            else if (address >= 0x4000 && address <= 0x7FFF)
            {
                // Only the first 14 bits of the address are wired up to the ROM chips
                address &= 0b0011_1111_1111_1111;

                return this.RomFile[((this.rROMB1 << 22) | (this.rROMB0 << 14) | address) & this.RomAddressMask];
            }
            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (this.RAMG)
                {
                    // Only the first 13 bits of the address are wired up to the RAM chips
                    address &= 0b0001_1111_1111_1111;

                    return this.Sram[(address | (this.rRAMB << 13)) & this.RamAddressMask];
                }
            }

            return 0xFF;
        }

        public override void Write(int address, byte value)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                // RAMG: 8 bits
                this.RAMG = value == 0b1010;
            }
            else if (address >= 0x2000 && address <= 0x2FFF)
            {
                // ROMB0: 8 bits
                this.rROMB0 = value;
            }
            else if (address >= 0x3000 && address <= 0x3FFF)
            {
                // ROMB1: 1 bits
                this.rROMB1 = (byte)(value & 0b0000_0001);
            }
            else if (address >= 0x6000 && address <= 0x7FFF)
            {
                // RAMB: 4 bit
                this.rRAMB = (byte)(value & 0b0000_1111);
            }
            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (this.RAMG)
                {
                    // Only the first 13 bits of the address are wired up to the RAM chips
                    address &= 0b0001_1111_1111_1111;

                    this.Sram[(address | (this.rRAMB << 13)) & this.RamAddressMask] = value;
                }
            }
        }
    }
}
