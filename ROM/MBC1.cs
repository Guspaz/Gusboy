using System;
using System.Linq;

namespace GusBoy
{
    public class MBC1 : Mapper
    {
        private byte BANK1 = 1;
        private byte BANK2 = 0;
        private bool MODE = false;
        private bool MBC1M = false;

        private readonly int bank2RomShift = 19;

        public MBC1(byte[] romFile, byte[] sram, string ramPath) : base(romFile, sram, ramPath)
        {
            // Check for MBC1 multicarts
            if (romFile.Length == 0x10_0000) // Only check 8 megabit
            {
                // We could do this with proper bank switching, but there are only four regions to check, so just do that.
                var logo1 = romFile.Skip(0x104).Take(0x30);
                var logo2 = romFile.Skip(0x40104).Take(0x30);
                var logo3 = romFile.Skip(0x80104).Take(0x30);
                var logo4 = romFile.Skip(0xC0104).Take(0x30);

                MBC1M = logo1.SequenceEqual(logo2) || logo1.SequenceEqual(logo3) || logo1.SequenceEqual(logo4);

                // MBC1M wires bank 2 one bit lower
                bank2RomShift = 18;
            }
        }

        public override byte Read(int address)
        {
            if (address <= 0x3FFF)
            {
                // Only the first 14 bits of the address are wired up to the ROM chips
                address &= 0b0011_1111_1111_1111;

                if (MODE)
                {
                    return romFile[((BANK2 << bank2RomShift) | address) & romAddressMask];
                }
                else
                {
                    return romFile[address & romAddressMask];
                }
            }
            else if (address >= 0x4000 && address <= 0x7FFF)
            {
                // Only the first 14 bits of the address are wired up to the ROM chips
                address &= 0b0011_1111_1111_1111;

                return romFile[((BANK2 << bank2RomShift) | (BANK1 << 14) | address) & romAddressMask];
            }
            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (RAMG)
                {
                    // Only the first 13 bits of the address are wired up to the RAM chips
                    address &= 0b0001_1111_1111_1111;

                    if (MODE)
                    {
                        return sram[((BANK2 << 13) | address) & ramAddressMask];
                    }
                    else
                    {
                        return sram[address & ramAddressMask];
                    }
                }
            }

            return 0xFF;
        }

        public override void Write(int address, byte value)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                // RAMG: 4 bits
                RAMG = (value & 0b1111) == 0b1010;
            }
            else if (address >= 0x2000 && address <= 0x3FFF)
            {
                // BANK1: 5 bits normally, 4 bits MBC1M
                value &= MBC1M ? 0b0000_1111 : 0b0001_1111;

                // MBC1 does not allow BANK1 to be 0
                BANK1 = value == 0 ? 1 : value;
            }
            else if (address >= 0x4000 && address <= 0x5FFF)
            {
                // BANK2: 2 bits
                value &= 0b0000_0011;

                BANK2 = value;
            }
            else if (address >= 0x6000 && address <= 0x7FFF)
            {
                // MODE: 1 bit
                value &= 0b0000_0001;

                MODE = value == 1;
            }
            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (RAMG)
                {
                    // Only the first 13 bits of the address are wired up to the RAM chips
                    address &= 0b0001_1111_1111_1111;

                    if (MODE)
                    {
                        sram[((BANK2 << 13) | address) & ramAddressMask] = value;
                    }
                    else
                    {
                        sram[address & ramAddressMask] = value;
                    }
                }
            }
        }
    }
}
