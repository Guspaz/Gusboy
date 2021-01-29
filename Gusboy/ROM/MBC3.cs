namespace Gusboy
{
    using System;
    using System.Linq;

    public class MBC3 : Mapper
    {
        private byte rROMB = 1;
        private byte rRAMB = 0;

        // RTC registers
        private byte rRTCS;
        private byte rRTCM;
        private byte rRTCH;
        private byte rRTCDL;
        private byte rRTCDH;
        private bool rLatch;

        private byte openBus;

        private Gameboy gb;

        public MBC3(byte[] romFile, byte[] sram, string ramPath, Gameboy gb)
            : base(romFile, sram, ramPath)
        {
            this.gb = gb;
            this.gb.Cpu.EnableRTC = true;

            // TODO: Implement RTC save/load (with real-world timestamp to increment time while closed)
            // TODO: Test ROM/RAM banking
            // TODO: Test open bus behaviour
        }

        public override byte Read(int address)
        {
            if (address <= 0x3FFF)
            {
                // Only the first 14 bits of the address are wired up to the ROM chips
                address &= 0b0011_1111_1111_1111;

                return this.openBus = this.RomFile[address & this.RomAddressMask];
            }
            else if (address >= 0x4000 && address <= 0x7FFF)
            {
                // Only the first 14 bits of the address are wired up to the ROM chips
                address &= 0b0011_1111_1111_1111;

                return this.openBus = this.RomFile[((this.rROMB << 14) | address) & this.RomAddressMask];
            }
            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (this.RAMG)
                {
                    // Only the first 13 bits of the address are wired up to the RAM chips
                    address &= 0b0001_1111_1111_1111;

                    if (this.rRAMB >= 0x08 && this.rRAMB <= 0x0C)
                    {
                        return this.rRAMB switch
                        {
                            0x08 => this.rRTCS,
                            0x09 => this.rRTCM,
                            0x0A => this.rRTCH,
                            0x0B => this.rRTCDL,
                            0x0C => this.rRTCDH,
                            _ => this.openBus,
                        };
                    }
                    else if ( this.rRAMB >= 0x00 && this.rRAMB <= 0x03 && this.Sram.Length > 0)
                    {
                        return this.openBus = this.Sram[(address | (this.rRAMB << 13)) & this.RamAddressMask];
                    }
                }

                return this.openBus;
            }

            return 0xFF;
        }

        public override void Write(int address, byte value)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                // RAMG: 4 bits
                this.RAMG = (value & 0b1111) == 0b1010;
            }
            else if (address >= 0x2000 && address <= 0x3FFF)
            {
                // BANK1: 5 bits
                value &= 0b0001_1111;

                // MBC3 does not allow ROMB to be 0
                this.rROMB = value == 0 ? 1 : value;
            }
            else if (address >= 0x4000 && address <= 0x5FFF)
            {
                // RAM bank or RTC select
                // RAMB: 4 bits
                value &= 0b0000_1111;

                this.rRAMB = value;
            }
            else if (address >= 0x6000 && address <= 0x7FFF)
            {
                // LATCH: 1 bit
                bool newLatch = (value & 0b0000_0001) == 1;

                if (newLatch && !this.rLatch)
                {
                    // LATCH
                    this.rRTCS = (byte)this.gb.Cpu.RtcSeconds;
                    this.rRTCM = (byte)this.gb.Cpu.RtcMinutes;
                    this.rRTCH = (byte)this.gb.Cpu.RtcHours;
                    this.rRTCDL = (byte)this.gb.Cpu.RtcDays; // This will get the first 8 bits
                    this.rRTCDH = (byte)((this.gb.Cpu.RtcDays > 255 ? 1 : 0) | (this.gb.Cpu.EnableRTC ? 0 : 0b0100_0000) | (this.gb.Cpu.RtcCarry ? 0b1000_0000 : 0));
                }

                this.rLatch = newLatch;
            }
            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (this.RAMG)
                {
                    // Only the first 13 bits of the address are wired up to the RAM chips
                    address &= 0b0001_1111_1111_1111;

                    if (this.rRAMB >= 0x08 && this.rRAMB <= 0x0C)
                    {
                        // RTC
                        switch (this.rRAMB)
                        {
                            case 0x08:
                                value &= 0b0011_1111;
                                this.gb.Cpu.RtcSeconds = value;
                                this.gb.Cpu.RtcSubseconds = 0;
                                break;
                            case 0x09:
                                value &= 0b0011_1111;
                                this.gb.Cpu.RtcMinutes = value;
                                break;
                            case 0x0A:
                                value &= 0b0001_1111;
                                this.gb.Cpu.RtcHours = value;
                                break;
                            case 0x0B:
                                this.gb.Cpu.RtcDays = (this.gb.Cpu.RtcDays & 0b1_0000_0000) | value;
                                break;
                            case 0x0C:
                                this.gb.Cpu.RtcDays = (this.gb.Cpu.RtcDays & 0b1111_1111) | ((value & 0b0000_0001) << 8);
                                this.gb.Cpu.RtcCarry = (value & 0b1000_0000) != 0;
                                this.gb.Cpu.EnableRTC = (value & 0b0100_0000) == 0;
                                break;
                        }
                    }
                    else if (this.rRAMB >= 0x00 && this.rRAMB <= 0x03 && this.Sram.Length > 0)
                    {
                        this.Sram[(address | (this.rRAMB << 13)) & this.RamAddressMask] = value;
                    }

                    this.DirtySram();
                }
            }
        }

        //private (int Seconds, int Minutes, int Hours, int Days, bool Carry) CalculateTime()
        //{
        //    int tickSeconds = (int)((this.gb.Cpu.RtcTicks - this.tickOffset) / (8192 * 256));
        //    int offsetSeconds = this.secondOffset + (this.minuteOffset * 60) + (this.hourOffset * 60 * 60) + (this.dayOffset * 60 * 60 * 24);
        //    int totalSeconds = this.rHalt ? offsetSeconds : tickSeconds + offsetSeconds;

        //    int days = totalSeconds / (60 * 60 * 24);
        //    bool carryBit = false;

        //    totalSeconds -= days * 60 * 60 * 24;

        //    if (days > 511)
        //    {
        //        carryBit = true;
        //        days -= 511;
        //    }

        //    int hours = (byte)(totalSeconds / (60 * 60));
        //    totalSeconds -= this.rRTCH * (60 * 60);

        //    int minutes = (byte)(totalSeconds / 60);
        //    totalSeconds -= this.rRTCM * 60;

        //    return (totalSeconds, minutes, hours, days, carryBit);
        //}
    }
}
