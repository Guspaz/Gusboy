using System;
using System.Linq;

namespace GusBoy
{
    public class MBC1 : Mapper
    {
        private byte BANK1 = 1;
        private byte BANK2 = 0;
        private bool MODE = false;
        private readonly bool MBC1M = false;

        private readonly int bank2RomShift = 19;

        public MBC1(byte[] romFile, byte[] sram, string ramPath) : base(romFile, sram, ramPath)
        {
            // Check for MBC1 multicarts
            if ( romFile.Length == 0x10_0000 ) // Only check 8 megabit
            {
                // We could do this with proper bank switching, but there are only four regions to check, so just do that.
                var logo1 = romFile.Skip(0x104).Take(0x30);
                var logo2 = romFile.Skip(0x40104).Take(0x30);
                var logo3 = romFile.Skip(0x80104).Take(0x30);
                var logo4 = romFile.Skip(0xC0104).Take(0x30);

                this.MBC1M = logo1.SequenceEqual(logo2) || logo1.SequenceEqual(logo3) || logo1.SequenceEqual(logo4);

                // MBC1M wires bank 2 one bit lower
                this.bank2RomShift = 18;
            }
        }

        public override byte Read(int address)
        {
            if ( address <= 0x3FFF )
            {
                // Only the first 14 bits of the address are wired up to the ROM chips
                address &= 0b0011_1111_1111_1111;

                if ( this.MODE )
                {
                    return this.romFile[((this.BANK2 << this.bank2RomShift) | address) & this.romAddressMask];
                }
                else
                {
                    return this.romFile[address & this.romAddressMask];
                }
            }
            else if ( address >= 0x4000 && address <= 0x7FFF )
            {
                // Only the first 14 bits of the address are wired up to the ROM chips
                address &= 0b0011_1111_1111_1111;

                return this.romFile[((this.BANK2 << this.bank2RomShift) | (this.BANK1 << 14) | address) & this.romAddressMask];
            }
            else if ( address >= 0xA000 && address <= 0xBFFF )
            {
                if ( this.RAMG )
                {
                    // Only the first 13 bits of the address are wired up to the RAM chips
                    address &= 0b0001_1111_1111_1111;

                    if ( this.MODE )
                    {
                        return this.sram[((this.BANK2 << 13) | address) & this.ramAddressMask];
                    }
                    else
                    {
                        return this.sram[address & this.ramAddressMask];
                    }
                }
            }

            return 0xFF;
        }

        public override void Write(int address, byte value)
        {
            if ( address >= 0x0000 && address <= 0x1FFF )
            {
                // RAMG: 4 bits
                this.RAMG = (value & 0b1111) == 0b1010;
            }
            else if ( address >= 0x2000 && address <= 0x3FFF )
            {
                // BANK1: 5 bits normally, 4 bits MBC1M
                value &= this.MBC1M ? 0b0000_1111 : 0b0001_1111;

                // MBC1 does not allow BANK1 to be 0
                this.BANK1 = value == 0 ? 1 : value;
            }
            else if ( address >= 0x4000 && address <= 0x5FFF )
            {
                // BANK2: 2 bits
                value &= 0b0000_0011;

                this.BANK2 = value;
            }
            else if ( address >= 0x6000 && address <= 0x7FFF )
            {
                // MODE: 1 bit
                value &= 0b0000_0001;

                this.MODE = value == 1;
            }
            else if ( address >= 0xA000 && address <= 0xBFFF )
            {
                if ( this.RAMG )
                {
                    // Only the first 13 bits of the address are wired up to the RAM chips
                    address &= 0b0001_1111_1111_1111;

                    if ( this.MODE )
                    {
                        this.sram[((this.BANK2 << 13) | address) & this.ramAddressMask] = value;
                    }
                    else
                    {
                        this.sram[address & this.ramAddressMask] = value;
                    }
                }
            }
        }
    }
}
