namespace Gusboy
{
    public class RAM
    {
        private readonly Gameboy gb;

        private readonly byte[] oam = new byte[0x100];
        private readonly byte[] hram = new byte[0x80];
        private readonly byte[,] wram = new byte[8, 0x1000];

        private int wramBank;
        private int backgroundPaletteIndex;
        private bool backgroundPaletteAutoIncrement;
        private int spritePaletteIndex;
        private bool spritePaletteAutoIncrement;

        public RAM(Gameboy gameBoy)
        {
            this.gb = gameBoy;
        }

        public byte[,] Vram { get; set; } = new byte[2, 0x2000];

        public int VramBank { get; set; }

        private CPU Cpu => this.gb.Cpu;

        private APU Apu => this.gb.Apu;

        private GPU Gpu => this.gb.Gpu;

        private ROM Rom => this.gb.Rom;

        public byte this[int i, bool isDma = false]
        {
            get
            {
                // Block access to everything but HRAM during DMA, unless it's the DMA itself.
                if (this.Gpu.IsDmaActive && !isDma && (i < 0xFF80 || i > 0xFFFE))
                {
                    return 0xFF;
                }

                switch (i)
                {
                    case <= 0x7FFF:
                        return this.Rom[i]; // Cartridge ROM

                    case >= 0x8000 and <= 0x9FFF:
                        return this.Gpu.CanAccessVRAM(isDma) ? this.Vram[this.VramBank, i - 0x8000] : 0xFF;

                    case >= 0xA000 and <= 0xBFFF:
                        return this.Rom[i]; // Cartridge RAM

                    case >= 0xC000 and <= 0xCFFF:
                        return this.wram[0, i - 0xC000]; // System RAM

                    case >= 0xD000 and <= 0xDFFF:
                        return this.wram[this.wramBank == 0 ? 1 : this.wramBank, i - 0xD000]; // System RAM, bank 0 maps to 1

                    case >= 0xE000 and <= 0xEDFF:
                        return this.wram[0, i - 0xE000]; // System RAM (mirror)

                    case >= 0xF000 and <= 0xFDFF:
                        return this.wram[this.wramBank == 0 ? 1 : this.wramBank, i - 0xF000]; // System RAM (mirror), bank 0 maps to 1

                    case >= 0xFE00 and <= 0xFEFF:
                        return this.Gpu.CanAccessOAM(isDma) ? this.oam[i - 0xFE00] : 0xFF; // Object Attribute Memory (FEA0-FEFF unusable)

                    case 0xFF00:
                        return this.gb.Input.Read();

                    case 0xFF01:
                        return RAM.Unsupported(0x00); // SB (Serial data transfer)

                    case 0xFF02:
                        return RAM.Unsupported(this.gb.IsCgb ? 0x7C : 0x7E); // SC (serial data transfer)

                    case 0xFF03:
                        return RAM.Unsupported(0x00); // Serial data transfer registers

                    case 0xFF04:
                        return (byte)(this.Cpu.rDIV >> 8);

                    case 0xFF05:
                        return this.Cpu.rTIMA;

                    case 0xFF06:
                        return this.Cpu.rTMA;

                    case 0xFF07:
                        return (byte)(this.Cpu.rTAC | 0b1111_1000);

                    case >= 0xFF08 and <= 0xFF0E:
                        return RAM.Unsupported(0xFF); // Unknown/unused

                    case 0xFF0F:
                        return (byte)(this.Cpu.rInterruptFlags | 0b1110_0000);

                    case 0xFF10:
                        return this.Apu.NR10;

                    case 0xFF11:
                        return this.Apu.NR11;

                    case 0xFF12:
                        return this.Apu.NR12;

                    case 0xFF13:
                        return this.Apu.NR13;

                    case 0xFF14:
                        return this.Apu.NR14;

                    case 0xFF15:
                        return 0xFF;

                    case 0xFF16:
                        return this.Apu.NR21;

                    case 0xFF17:
                        return this.Apu.NR22;

                    case 0xFF18:
                        return this.Apu.NR23;

                    case 0xFF19:
                        return this.Apu.NR24;

                    case 0xFF1A:
                        return this.Apu.NR30;

                    case 0xFF1B:
                        return this.Apu.NR31;

                    case 0xFF1C:
                        return this.Apu.NR32;

                    case 0xFF1D:
                        return this.Apu.NR33;

                    case 0xFF1E:
                        return this.Apu.NR34;

                    case 0xFF1F:
                        return 0xFF;

                    case 0xFF20:
                        return this.Apu.NR41;

                    case 0xFF21:
                        return this.Apu.NR42;

                    case 0xFF22:
                        return this.Apu.NR43;

                    case 0xFF23:
                        return this.Apu.NR44;

                    case 0xFF24:
                        return this.Apu.NR50;

                    case 0xFF25:
                        return this.Apu.NR51;

                    case 0xFF26:
                        return this.Apu.NR52;

                    case >= 0xFF27 and <= 0xFF2F:
                        return 0xFF; // Unused sound bytes

                    case >= 0xFF30 and <= 0xFF3F:
                        return this.Apu.WaveEnabled ? 0xFF : this.Apu.WaveTable[i - 0xFF30];

                    case 0xFF40:
                        return this.Gpu.GetLCDC();

                    case 0xFF41:
                        return (byte)(this.Gpu.Stat | 0b1000_0000);

                    case 0xFF42:
                        return this.Gpu.ScrollY;

                    case 0xFF43:
                        return this.Gpu.ScrollX;

                    case 0xFF44:
                        return this.Gpu.CurrentLine; // rLY register

                    case 0xFF45:
                        return this.Gpu.rLYC;

                    case 0xFF46:
                        return RAM.Unsupported(0xFF); // DMA transfer (write-only register)

                    case 0xFF47:
                        return this.Gpu.BgPal;

                    case 0xFF48:
                        return this.Gpu.ObjPal0;

                    case 0xFF49:
                        return this.Gpu.ObjPal1;

                    case 0xFF4A:
                        return this.Gpu.WinY;

                    case 0xFF4B:
                        return this.Gpu.WinX;

                    // TODO: Optimize this to make the switch contiguous again
                    case 0xFF4D:
                        return this.gb.IsCgb ? (byte)(((this.Cpu.fSpeed ? 1 : 0) << 7) | (this.Cpu.fPrepareSwitch ? 1 : 0) | 0b0111_1110) : 0xFF;

                    case 0xFF4F:
                        return this.gb.IsCgb ? (byte)(this.VramBank | 0x1111_1110) : 0xFF;

                    case 0xFF55:
                        // Since we copy instantly, always signal done
                        return 0xFF; // this.gb.Gpu.HDMA5Control;

                    case 0xFF68:
                        return 0xFF; // TODO: Check if this is supposed to be readable?

                    case 0xFF69:
                        int x = this.backgroundPaletteIndex >> 3;
                        int y = (this.backgroundPaletteIndex >> 1) & 0b00_0011;

                        if ((this.backgroundPaletteIndex & 0b0001) == 0)
                        {
                            return (byte)(this.Gpu.PalCgbBackground[x, y] & 0x00FF);
                        }
                        else
                        {
                            return (byte)((this.Gpu.PalCgbBackground[x, y] & 0xFF00) >> 8);
                        }

                    case 0xFF6A:
                        return 0xFF; // TODO: Check if this is supposed to be readable?

                    case 0xFF6B:
                        int a = this.spritePaletteIndex >> 3;
                        int b = (this.spritePaletteIndex >> 1) & 0b00_0011;

                        if ((this.spritePaletteIndex & 0b0001) == 0)
                        {
                            return (byte)(this.Gpu.PalCgbSprites[a, b] & 0x00FF);
                        }
                        else
                        {
                            return (byte)((this.Gpu.PalCgbSprites[a, b] & 0xFF00) >> 8);
                        }

                    case 0xFF6C:
                        return this.Gpu.OAMPriorityMode ? 0 : 1; // NOTE: This is inverted to default to DMG mode.

                    case 0xFF70:
                        return this.gb.IsCgb ? (byte)(this.wramBank | 0x1111_1000) : 0xFF;

                    case >= 0xFF4C and <= 0xFF7F:
                        return RAM.Unsupported(0xFF); // Unsupported/unused (mostly CGB stuff)

                    case >= 0xFF80 and <= 0xFFFE:
                        return this.hram[i - 0xFF80]; // High RAM

                    case 0xFFFF:
                        return this.Cpu.rInterruptEnable; // (byte)(cpu.interruptEnable | 0b1110_0000);

                    default:
                        // Just to shut up the compiler suggesting this be made into a switch expression
                        if (true)
                        {
                            return 0xFF;
                        }
                }
            }

            set
            {
                // Block access to everything but HRAM during DMA, unless it's the DMA itself.
                if (this.Gpu.IsDmaActive && !isDma && (i < 0xFF80 || i > 0xFFFE))
                {
                    return;
                }

                switch (i)
                {
                    case >= 0x0000 and <= 0x7FFF:
                        this.Rom[i] = value; // Cartridge ROM
                        break;

                    case >= 0x8000 and <= 0x9FFF:
                        if (this.Gpu.CanAccessVRAM(isDma))
                        {
                            this.Vram[this.VramBank, i - 0x8000] = value; // VRAM

                            if (this.VramBank == 1 && i >= 0x9800)
                            {
                                this.Gpu.BackgroundCacheDirty = true;
                            }
                        }

                        break;

                    case >= 0xA000 and <= 0xBFFF:
                        this.Rom[i] = value; // Cartridge RAM
                        break;

                    case >= 0xC000 and <= 0xCFFF:
                        this.wram[0, i - 0xC000] = value; // System RAM
                        break;

                    case >= 0xD000 and <= 0xDFFF:
                        this.wram[this.wramBank == 0 ? 1 : this.wramBank, i - 0xD000] = value; // System RAM
                        break;

                    case >= 0xE000 and <= 0xEDFF:
                        this.wram[0, i - 0xE000] = value; // System RAM (mirror)
                        break;

                    case >= 0xF000 and <= 0xFDFF:
                        this.wram[this.wramBank == 0 ? 1 : this.wramBank, i - 0xF000] = value; // System RAM (mirror)
                        break;

                    case >= 0xFE00 and <= 0xFEFF:
                        if (this.Gpu.CanAccessOAM(isDma))
                        {
                            this.oam[i - 0xFE00] = value; // Object Attribute Memory (FEA0-FEFF unusable)
                            this.Gpu.SpriteCacheDirty = true;
                        }

                        break;

                    case 0xFF00:
                        this.gb.Input.Write(value); // Joystick register
                        break;

                    case >= 0xFF01 and <= 0xFF03:
                        RAM.Unsupported(0x00); // Serial data transfer registers
                        break;

                    case 0xFF04:
                        this.Cpu.rDIV = 0; // May need to adjust TIMA?
                        break;

                    case 0xFF05:
                        this.Cpu.rTIMA = value;
                        this.Cpu.ReloadingTima = false;
                        break;

                    case 0xFF06:
                        this.Cpu.rTMA = value;
                        break;

                    case 0xFF07:
                        this.Cpu.rTAC = value;
                        break;

                    case >= 0xFF08 and <= 0xFF0E:
                        RAM.Unsupported(0xFF); // Unknown/unused
                        break;

                    case 0xFF0F:
                        this.Cpu.rInterruptFlags = (byte)(value | 0b1110_0000);
                        break;

                    case 0xFF10:
                        this.Apu.NR10 = value;
                        break;

                    case 0xFF11:
                        this.Apu.NR11 = value;
                        break;

                    case 0xFF12:
                        this.Apu.NR12 = value;
                        break;

                    case 0xFF13:
                        this.Apu.NR13 = value;
                        break;

                    case 0xFF14:
                        this.Apu.NR14 = value;
                        break;

                    case 0xFF15:
                        RAM.Unsupported(0xFF);
                        break;

                    case 0xFF16:
                        this.Apu.NR21 = value;
                        break;

                    case 0xFF17:
                        this.Apu.NR22 = value;
                        break;

                    case 0xFF18:
                        this.Apu.NR23 = value;
                        break;

                    case 0xFF19:
                        this.Apu.NR24 = value;
                        break;

                    case 0xFF1A:
                        this.Apu.NR30 = value;
                        break;

                    case 0xFF1B:
                        this.Apu.NR31 = value;
                        break;

                    case 0xFF1C:
                        this.Apu.NR32 = value;
                        break;

                    case 0xFF1D:
                        this.Apu.NR33 = value;
                        break;

                    case 0xFF1E:
                        this.Apu.NR34 = value;
                        break;

                    case 0xFF1F:
                        RAM.Unsupported(0xFF);
                        break;

                    case 0xFF20:
                        this.Apu.NR41 = value;
                        break;

                    case 0xFF21:
                        this.Apu.NR42 = value;
                        break;

                    case 0xFF22:
                        this.Apu.NR43 = value;
                        break;

                    case 0xFF23:
                        this.Apu.NR44 = value;
                        break;

                    case 0xFF24:
                        this.Apu.NR50 = value;
                        break;

                    case 0xFF25:
                        this.Apu.NR51 = value;
                        break;

                    case 0xFF26:
                        this.Apu.NR52 = value;
                        break;

                    case >= 0xFF27 and <= 0xFF2F:
                        RAM.Unsupported(0xFF);
                        break;

                    case >= 0xFF30 and <= 0xFF3F:
                        if (!this.Apu.WaveEnabled)
                        {
                            this.Apu.WaveTable[i - 0xFF30] = value;
                        }

                        break;

                    case 0xFF40:
                        this.Gpu.SetLCDC(value);
                        break;

                    case 0xFF41:
                        this.Gpu.Stat = value;
                        break;

                    case 0xFF42:
                        this.Gpu.ScrollY = value;
                        break;

                    case 0xFF43:
                        this.Gpu.ScrollX = value;
                        break;

                    case 0xFF44:
                        RAM.Unsupported(0xFF); // Scanline counter is read-only
                        break;

                    case 0xFF45:
                        this.Gpu.rLYC = value;
                        break;

                    case 0xFF46:
                        this.Gpu.TriggerDMA(value);
                        break;

                    case 0xFF47:
                        this.Gpu.BgPal = value;
                        break;

                    case 0xFF48:
                        this.Gpu.ObjPal0 = value;
                        break;

                    case 0xFF49:
                        this.Gpu.ObjPal1 = value;
                        break;

                    case 0xFF4A:
                        this.Gpu.WinY = value;
                        break;

                    case 0xFF4B:
                        this.Gpu.WinX = value;
                        break;

                    case 0xFF4D:
                        if (this.gb.IsCgb)
                        {
                            this.Cpu.fPrepareSwitch = (value & 0b0000_0001) != 0;
                        }

                        break;

                    // TODO: Restore contiguity
                    case >= 0xFF4C and <= 0xFF4E:
                        RAM.Unsupported(0xFF);
                        break;

                    case 0xFF4F:
                        if (this.gb.IsCgb)
                        {
                            this.VramBank = value & 0b0001;
                        }

                        break;

                    case 0xFF50:
                        this.Rom[0xFF50] = value; // Bootstrap completed
                        break;

                    case 0xFF51:
                        this.Gpu.HDMA1SourceHi = value;
                        break;

                    case 0xFF52:
                        this.Gpu.HDMA2SourceLo = (byte)(value & 0b1111_0000);
                        break;

                    case 0xFF53:
                        this.Gpu.HDMA3DestHi = (byte)(value & 0b0001_1111);
                        break;

                    case 0xFF54:
                        this.Gpu.HDMA4DestLo = (byte)(value & 0b1111_0000);
                        break;

                    case 0xFF55:
                        this.gb.Gpu.HDMA5Control = value;
                        break;

                    case 0xFF68:
                        this.backgroundPaletteIndex = value & 0b0011_1111;
                        this.backgroundPaletteAutoIncrement = (value & 0b1000_0000) != 0;
                        break;

                    case 0xFF69:

                        int x = this.backgroundPaletteIndex >> 3;
                        int y = (this.backgroundPaletteIndex >> 1) & 0b00_0011;

                        if ((this.backgroundPaletteIndex & 0b0001) == 0)
                        {
                            this.Gpu.PalCgbBackground[x, y] = (this.Gpu.PalCgbBackground[x, y] & 0xFF00) | value;
                        }
                        else
                        {
                            this.Gpu.PalCgbBackground[x, y] = (this.Gpu.PalCgbBackground[x, y] & 0x00FF) | (value << 8);
                        }

                        this.Gpu.BackgroundCacheDirty = true;

                        if (this.backgroundPaletteAutoIncrement)
                        {
                            this.backgroundPaletteIndex++;
                        }

                        break;

                    case 0xFF6A:
                        this.spritePaletteIndex = value & 0b0011_1111;
                        this.spritePaletteAutoIncrement = (value & 0b1000_0000) != 0;
                        break;

                    case 0xFF6B:

                        int a = this.spritePaletteIndex >> 3;
                        int b = (this.spritePaletteIndex >> 1) & 0b00_0011;

                        if ((this.spritePaletteIndex & 0b0001) == 0)
                        {
                            this.Gpu.PalCgbSprites[a, b] = (this.Gpu.PalCgbSprites[a, b] & 0xFF00) | value;
                        }
                        else
                        {
                            this.Gpu.PalCgbSprites[a, b] = (this.Gpu.PalCgbSprites[a, b] & 0x00FF) | (value << 8);
                        }

                        this.Gpu.SpriteCacheDirty = true;

                        if (this.spritePaletteAutoIncrement)
                        {
                            this.spritePaletteIndex++;
                        }

                        break;

                    case 0xFF6C:
                        this.Gpu.OAMPriorityMode = (value & 1) == 1; // NOTE: This is inverted so that it defaults to DMG behaviour.
                        break;

                    // TODO: Make contiguous again for performance
                    case 0xFF70:
                        if (this.gb.IsCgb)
                        {
                            this.wramBank = value & 0b0111;
                        }

                        break;

                    case >= 0xFF51 and <= 0xFF7F:
                        RAM.Unsupported(0xFF); // Unsupported/unused (mostly CGB stuff)
                        break;

                    case >= 0xFF80 and <= 0xFFFE:
                        this.hram[i - 0xff80] = value; // High RAM
                        break;

                    case 0xFFFF:
                        this.Cpu.rInterruptEnable = value;
                        break;
                }
            }
        }

        public void SetShort(int address, Gshort value)
        {
            this[address] = value.Lo;
            this[address + 1] = value.Hi;
        }

        public Gshort GetShort(int address) => (ushort)((this[address + 1] << 8) + this[address]);

        internal void ClearRAM()
        {
            for (int bank = 0; bank < 8; bank++)
            {
                for (int address = 0; address < 0x1000; address++)
                {
                    this.wram[bank, address] = 0x00;
                }
            }
        }

        // This exists mainly to make it easy to print debug statements if required.
        private static byte Unsupported(byte value) => value;
    }
}
