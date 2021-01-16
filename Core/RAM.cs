namespace GusBoy
{
    public class gRAM
    {
        public byte[] sram = new byte[0x2000];
        public byte[] vram = new byte[0x2000];
        public byte[] oam = new byte[0x100];
        public byte[] wram = new byte[0x2000];
        public byte[] hram = new byte[0x80];

        private Gameboy gb;

        private CPU cpu => gb.cpu;
        private APU apu => gb.apu;
        private GPU gpu => gb.gpu;
        private ROM rom => gb.rom;

        public gRAM(Gameboy gameBoy)
        {
            this.gb = gameBoy;
        }

        private byte Unsupported(int i, byte value)
        {
            //messageCallback($"Invalid read from IO address 0x{i:X4}. Ignoring.");
            return value;
        }
        
        public byte this[int i, bool isDma = false]
        {
            get
            {
                // Block access to everything but HRAM during DMA, unless it's the DMA itself.
                if (gpu.IsDmaActive && !isDma && (i < 0xFF80 || i > 0xFFFE))
                {
                    return 0xFF;
                }

                switch (i)
                {
                    case <= 0x7FFF:
                        return rom[i]; // Cartridge ROM

                    case >= 0x8000 and <= 0x9FFF:
                        if (gpu.CanAccessVRAM(isDma))
                        {
                            return vram[i - 0x8000]; // VRAM
                        }
                        else
                        {
                            return 0xFF;
                        }

                    case >= 0xA000 and <= 0xBFFF:
                        return rom[i]; // Cartridge RAM

                    case >= 0xC000 and <= 0xDFFF:
                        return wram[i - 0xC000]; // System RAM

                    case >= 0xE000 and <= 0xFDFF:
                        return wram[i - 0xE000]; // System RAM (mirror)

                    case >= 0xFE00 and <= 0xFEFF:
                        if (gpu.CanAccessOAM(isDma))
                        {
                            return oam[i - 0xFE00]; //Object Attribute Memory (FEA0-FEFF unusable)
                        }
                        else
                        {
                            return 0xFF;
                        }

                    case 0xFF00:
                        return gb.input.Read();
                        //return GetJoypadState(); // Joystick register

                    case 0xFF01:
                        return Unsupported(i, 0x00); // SB (Serial data transfer)

                    case 0xFF02:
                        return Unsupported(i, 0x7E); // SC (serial data transfer)

                    case 0xFF03:
                        return Unsupported(i, 0x00); // Serial data transfer registers 

                    case 0xFF04:
                        return cpu.rDIV.Hi;

                    case 0xFF05:
                        return cpu.rTIMA;

                    case 0xFF06:
                        return cpu.rTMA;

                    case 0xFF07:
                        return (byte)(cpu.rTAC | 0b1111_1000);

                    case >= 0xFF08 and <= 0xFF0E:
                        return Unsupported(i, 0xFF); // Unknown/unused

                    case 0xFF0F:
                        return (byte)(cpu.rInterruptFlags | 0b1110_0000);

                    case 0xFF10:
                        return apu.NR10;

                    case 0xFF11:
                        return apu.NR11;

                    case 0xFF12:
                        return apu.NR12;

                    case 0xFF13:
                        return apu.NR13;

                    case 0xFF14:
                        return apu.NR14;

                    case 0xFF15:
                        return 0xFF;

                    case 0xFF16:
                        return apu.NR21;

                    case 0xFF17:
                        return apu.NR22;

                    case 0xFF18:
                        return apu.NR23;

                    case 0xFF19:
                        return apu.NR24;

                    case 0xFF1A:
                        return apu.NR30;

                    case 0xFF1B:
                        return apu.NR31;

                    case 0xFF1C:
                        return apu.NR32;

                    case 0xFF1D:
                        return apu.NR33;

                    case 0xFF1E:
                        return apu.NR34;

                    case 0xFF1F:
                        return 0xFF;

                    case 0xFF20:
                        return apu.NR41;

                    case 0xFF21:
                        return apu.NR42;

                    case 0xFF22:
                        return apu.NR43;

                    case 0xFF23:
                        return apu.NR44;

                    case 0xFF24:
                        return apu.NR50;

                    case 0xFF25:
                        return apu.NR51;

                    case 0xFF26:
                        return apu.NR52;

                    case >= 0xFF27 and <= 0xFF2F:
                        return 0xFF; // Unused sound bytes

                    case >= 0xFF30 and <= 0xFF3F:
                        if (apu.Channel3.LengthStatus)
                        {
                            return 0xFF;
                        }
                        else
                        {
                            return apu.WaveTable[i - 0xFF30];
                        }

                    case 0xFF40:
                        return gpu.GetLCDC();

                    case 0xFF41:
                        return (byte)(gpu.stat | 0b1000_0000);

                    case 0xFF42:
                        return gpu.scrollY;

                    case 0xFF43:
                        return gpu.scrollX;

                    case 0xFF44:
                        return gpu.currentLine; // rLY register

                    case 0xFF45:
                        return gpu.rLYC;

                    case 0xFF46:
                        return Unsupported(i, 0xFF); // DMA transfer (write-only register)

                    case 0xFF47:
                        return gpu.bgPal;

                    case 0xFF48:
                        return gpu.objPal0;

                    case 0xFF49:
                        return gpu.objPal1;

                    case 0xFF4A:
                        return gpu.winY;

                    case 0xFF4B:
                        return gpu.winX;

                    case >= 0xFF4C and <= 0xFF7F:
                        return Unsupported(i, 0xFF); // Unsupported/unused (mostly CGB stuff)

                    case >= 0xFF80 and <= 0xFFFE:
                        return hram[i - 0xFF80]; // High RAM

                    case 0xFFFF:
                        return cpu.rInterruptEnable; //(byte)(cpu.interruptEnable | 0b1110_0000);

                    default:
                        return 0xFF;
                }
            }
            set
            {
                // Block access to everything but HRAM during DMA, unless it's the DMA itself.
                if (gpu.IsDmaActive && !isDma && (i < 0xFF80 || i > 0xFFFE))
                {
                    return;
                }

                switch (i)
                {
                    case >= 0x0000 and <= 0x7FFF:
                        rom[i] = value; // Cartridge ROM
                        break;

                    case >= 0x8000 and <= 0x9FFF:
                        if (gpu.CanAccessVRAM(isDma))
                        {
                            vram[i - 0x8000] = value; // VRAM
                        }
                        break;

                    case >= 0xA000 and <= 0xBFFF:
                        rom[i] = value; // Cartridge RAM
                        break;

                    case >= 0xC000 and <= 0xDFFF:
                        wram[i - 0xC000] = value; // System RAM
                        break;

                    case >= 0xE000 and <= 0xFDFF:
                        wram[i - 0xE000] = value; // System RAM (mirror)
                        break;

                    case >= 0xFE00 and <= 0xFEFF:
                        if (gpu.CanAccessOAM(isDma))
                        {
                            oam[i - 0xFE00] = value; //Object Attribute Memory (FEA0-FEFF unusable)
                            gpu.spriteCacheDirty = true;
                        }
                        break;

                    case 0xFF00:
                        gb.input.Write(value); // Joystick register
                        break;

                    case >= 0xFF01 and <= 0xFF03:
                        Unsupported(i, 0x00); // Serial data transfer registers 
                        break;

                    case 0xFF04:
                        cpu.rDIV = 0; // May need to adjust TIMA?
                        break;

                    case 0xFF05:
                        cpu.rTIMA = value;
                        cpu.reloadingTima = false;
                        break;

                    case 0xFF06:
                        cpu.rTMA = value;
                        break;

                    case 0xFF07:
                        cpu.rTAC = value;
                        //cpu.UpdateTAC(value);
                        break;

                    case >= 0xFF08 and <= 0xFF0E:
                        Unsupported(i, 0xFF); // Unknown/unused
                        break;

                    case 0xFF0F:
                        cpu.rInterruptFlags = value;
                        break;

                    case 0xFF10:
                        apu.NR10 = value;
                        break;

                    case 0xFF11:
                        apu.NR11 = value;
                        break;

                    case 0xFF12:
                        apu.NR12 = value;
                        break;

                    case 0xFF13:
                        apu.NR13 = value;
                        break;

                    case 0xFF14:
                        apu.NR14 = value;
                        break;

                    case 0xFF15:
                        Unsupported(i, 0xFF);
                        break;

                    case 0xFF16:
                        apu.NR21 = value;
                        break;

                    case 0xFF17:
                        apu.NR22 = value;
                        break;

                    case 0xFF18:
                        apu.NR23 = value;
                        break;

                    case 0xFF19:
                        apu.NR24 = value;
                        break;

                    case 0xFF1A:
                        apu.NR30 = value;
                        break;

                    case 0xFF1B:
                        apu.NR31 = value;
                        break;

                    case 0xFF1C:
                        apu.NR32 = value;
                        break;

                    case 0xFF1D:
                        apu.NR33 = value;
                        break;

                    case 0xFF1E:
                        apu.NR34 = value;
                        break;

                    case 0xFF1F:
                        Unsupported(i, 0xFF);
                        break;

                    case 0xFF20:
                        apu.NR41 = value;
                        break;

                    case 0xFF21:
                        apu.NR42 = value;
                        break;

                    case 0xFF22:
                        apu.NR43 = value;
                        break;

                    case 0xFF23:
                        apu.NR44 = value;
                        break;

                    case 0xFF24:
                        apu.NR50 = value;
                        break;

                    case 0xFF25:
                        apu.NR51 = value;
                        break;

                    case 0xFF26:
                        apu.NR52 = value;
                        break;

                    case >= 0xFF27 and <= 0xFF2F:
                        Unsupported(i, 0xFF);
                        break;

                    case >= 0xFF30 and <= 0xFF3F:
                        if (!apu.Channel3.LengthStatus)
                        {
                            apu.WaveTable[i - 0xFF30] = value;
                        }
                        break;

                    case 0xFF40:
                        gpu.SetLCDC(value);
                        break;

                    case 0xFF41:
                        gpu.stat = value;
                        break;

                    case 0xFF42:
                        gpu.scrollY = value;
                        break;

                    case 0xFF43:
                        gpu.scrollX = value;
                        break;

                    case 0xFF44:
                        Unsupported(i, 0xFF); // Scanline counter is read-only
                        break;

                    case 0xFF45:
                        gpu.rLYC = value;
                        break;

                    case 0xFF46:
                        gpu.TriggerDMA(value);
                        break;

                    case 0xFF47:
                        gpu.bgPal = value;
                        break;

                    case 0xFF48:
                        gpu.objPal0 = value;
                        break;

                    case 0xFF49:
                        gpu.objPal1 = value;
                        break;

                    case 0xFF4A:
                        gpu.winY = value;
                        break;

                    case 0xFF4B:
                        gpu.winX = value;
                        break;

                    case >= 0xFF4C and <= 0xFF4F:
                        Unsupported(i, 0xFF);
                        break;

                    case 0xFF50:
                        rom[0xFF50] = value; // Bootstrap completed
                        break;

                    case >= 0xFF51 and <= 0xFF7F:
                        Unsupported(i, 0xFF); // Unsupported/unused (mostly CGB stuff)
                        break;

                    case >= 0xFF80 and <= 0xFFFE:
                        hram[i - 0xff80] = value; // High RAM
                        break;

                    case 0xFFFF:
                        cpu.rInterruptEnable = value;
                        break;
                }
            }
        }

        public void SetShort(int address, gshort value)
        {
            this[address] = value.Lo;
            this[address + 1] = value.Hi;
        }

        public gshort GetShort(int address)
        {
            return (ushort)((this[address + 1] << 8) + this[address]);
        }
    }
}
