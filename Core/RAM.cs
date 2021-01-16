namespace GusBoy
{
    public class gRAM
    {
        public byte[] sram = new byte[0x2000];
        public byte[] vram = new byte[0x2000];
        public byte[] oam = new byte[0x100];
        public byte[] wram = new byte[0x2000];
        public byte[] hram = new byte[0x80];

        private readonly Gameboy gb;

        private CPU cpu => this.gb.cpu;
        private APU apu => this.gb.apu;
        private GPU gpu => this.gb.gpu;
        private ROM rom => this.gb.rom;

        public gRAM(Gameboy gameBoy)
        {
            this.gb = gameBoy;
        }

        private byte Unsupported(int i, byte value) =>
            //messageCallback($"Invalid read from IO address 0x{i:X4}. Ignoring.");
            value;

        public byte this[int i, bool isDma = false]
        {
            get
            {
                // Block access to everything but HRAM during DMA, unless it's the DMA itself.
                if ( this.gpu.IsDmaActive && !isDma && (i < 0xFF80 || i > 0xFFFE) )
                {
                    return 0xFF;
                }

                switch ( i )
                {
                    case <= 0x7FFF:
                        return this.rom[i]; // Cartridge ROM

                    case >= 0x8000 and <= 0x9FFF:
                        if ( this.gpu.CanAccessVRAM(isDma) )
                        {
                            return this.vram[i - 0x8000]; // VRAM
                        }
                        else
                        {
                            return 0xFF;
                        }

                    case >= 0xA000 and <= 0xBFFF:
                        return this.rom[i]; // Cartridge RAM

                    case >= 0xC000 and <= 0xDFFF:
                        return this.wram[i - 0xC000]; // System RAM

                    case >= 0xE000 and <= 0xFDFF:
                        return this.wram[i - 0xE000]; // System RAM (mirror)

                    case >= 0xFE00 and <= 0xFEFF:
                        if ( this.gpu.CanAccessOAM(isDma) )
                        {
                            return this.oam[i - 0xFE00]; //Object Attribute Memory (FEA0-FEFF unusable)
                        }
                        else
                        {
                            return 0xFF;
                        }

                    case 0xFF00:
                        return this.gb.input.Read();
                    //return GetJoypadState(); // Joystick register

                    case 0xFF01:
                        return this.Unsupported(i, 0x00); // SB (Serial data transfer)

                    case 0xFF02:
                        return this.Unsupported(i, 0x7E); // SC (serial data transfer)

                    case 0xFF03:
                        return this.Unsupported(i, 0x00); // Serial data transfer registers 

                    case 0xFF04:
                        return this.cpu.rDIV.Hi;

                    case 0xFF05:
                        return this.cpu.rTIMA;

                    case 0xFF06:
                        return this.cpu.rTMA;

                    case 0xFF07:
                        return (byte)(this.cpu.rTAC | 0b1111_1000);

                    case >= 0xFF08 and <= 0xFF0E:
                        return this.Unsupported(i, 0xFF); // Unknown/unused

                    case 0xFF0F:
                        return (byte)(this.cpu.rInterruptFlags | 0b1110_0000);

                    case 0xFF10:
                        return this.apu.NR10;

                    case 0xFF11:
                        return this.apu.NR11;

                    case 0xFF12:
                        return this.apu.NR12;

                    case 0xFF13:
                        return this.apu.NR13;

                    case 0xFF14:
                        return this.apu.NR14;

                    case 0xFF15:
                        return 0xFF;

                    case 0xFF16:
                        return this.apu.NR21;

                    case 0xFF17:
                        return this.apu.NR22;

                    case 0xFF18:
                        return this.apu.NR23;

                    case 0xFF19:
                        return this.apu.NR24;

                    case 0xFF1A:
                        return this.apu.NR30;

                    case 0xFF1B:
                        return this.apu.NR31;

                    case 0xFF1C:
                        return this.apu.NR32;

                    case 0xFF1D:
                        return this.apu.NR33;

                    case 0xFF1E:
                        return this.apu.NR34;

                    case 0xFF1F:
                        return 0xFF;

                    case 0xFF20:
                        return this.apu.NR41;

                    case 0xFF21:
                        return this.apu.NR42;

                    case 0xFF22:
                        return this.apu.NR43;

                    case 0xFF23:
                        return this.apu.NR44;

                    case 0xFF24:
                        return this.apu.NR50;

                    case 0xFF25:
                        return this.apu.NR51;

                    case 0xFF26:
                        return this.apu.NR52;

                    case >= 0xFF27 and <= 0xFF2F:
                        return 0xFF; // Unused sound bytes

                    case >= 0xFF30 and <= 0xFF3F:
                        if ( this.apu.Channel3.LengthStatus )
                        {
                            return 0xFF;
                        }
                        else
                        {
                            return this.apu.WaveTable[i - 0xFF30];
                        }

                    case 0xFF40:
                        return this.gpu.GetLCDC();

                    case 0xFF41:
                        return (byte)(this.gpu.stat | 0b1000_0000);

                    case 0xFF42:
                        return this.gpu.scrollY;

                    case 0xFF43:
                        return this.gpu.scrollX;

                    case 0xFF44:
                        return this.gpu.currentLine; // rLY register

                    case 0xFF45:
                        return this.gpu.rLYC;

                    case 0xFF46:
                        return this.Unsupported(i, 0xFF); // DMA transfer (write-only register)

                    case 0xFF47:
                        return this.gpu.bgPal;

                    case 0xFF48:
                        return this.gpu.objPal0;

                    case 0xFF49:
                        return this.gpu.objPal1;

                    case 0xFF4A:
                        return this.gpu.winY;

                    case 0xFF4B:
                        return this.gpu.winX;

                    case >= 0xFF4C and <= 0xFF7F:
                        return this.Unsupported(i, 0xFF); // Unsupported/unused (mostly CGB stuff)

                    case >= 0xFF80 and <= 0xFFFE:
                        return this.hram[i - 0xFF80]; // High RAM

                    case 0xFFFF:
                        return this.cpu.rInterruptEnable; //(byte)(cpu.interruptEnable | 0b1110_0000);

                    default:
                        return 0xFF;
                }
            }
            set
            {
                // Block access to everything but HRAM during DMA, unless it's the DMA itself.
                if ( this.gpu.IsDmaActive && !isDma && (i < 0xFF80 || i > 0xFFFE) )
                {
                    return;
                }

                switch ( i )
                {
                    case >= 0x0000 and <= 0x7FFF:
                        this.rom[i] = value; // Cartridge ROM
                        break;

                    case >= 0x8000 and <= 0x9FFF:
                        if ( this.gpu.CanAccessVRAM(isDma) )
                        {
                            this.vram[i - 0x8000] = value; // VRAM
                        }
                        break;

                    case >= 0xA000 and <= 0xBFFF:
                        this.rom[i] = value; // Cartridge RAM
                        break;

                    case >= 0xC000 and <= 0xDFFF:
                        this.wram[i - 0xC000] = value; // System RAM
                        break;

                    case >= 0xE000 and <= 0xFDFF:
                        this.wram[i - 0xE000] = value; // System RAM (mirror)
                        break;

                    case >= 0xFE00 and <= 0xFEFF:
                        if ( this.gpu.CanAccessOAM(isDma) )
                        {
                            this.oam[i - 0xFE00] = value; //Object Attribute Memory (FEA0-FEFF unusable)
                            this.gpu.spriteCacheDirty = true;
                        }
                        break;

                    case 0xFF00:
                        this.gb.input.Write(value); // Joystick register
                        break;

                    case >= 0xFF01 and <= 0xFF03:
                        this.Unsupported(i, 0x00); // Serial data transfer registers 
                        break;

                    case 0xFF04:
                        this.cpu.rDIV = 0; // May need to adjust TIMA?
                        break;

                    case 0xFF05:
                        this.cpu.rTIMA = value;
                        this.cpu.reloadingTima = false;
                        break;

                    case 0xFF06:
                        this.cpu.rTMA = value;
                        break;

                    case 0xFF07:
                        this.cpu.rTAC = value;
                        //cpu.UpdateTAC(value);
                        break;

                    case >= 0xFF08 and <= 0xFF0E:
                        this.Unsupported(i, 0xFF); // Unknown/unused
                        break;

                    case 0xFF0F:
                        this.cpu.rInterruptFlags = value;
                        break;

                    case 0xFF10:
                        this.apu.NR10 = value;
                        break;

                    case 0xFF11:
                        this.apu.NR11 = value;
                        break;

                    case 0xFF12:
                        this.apu.NR12 = value;
                        break;

                    case 0xFF13:
                        this.apu.NR13 = value;
                        break;

                    case 0xFF14:
                        this.apu.NR14 = value;
                        break;

                    case 0xFF15:
                        this.Unsupported(i, 0xFF);
                        break;

                    case 0xFF16:
                        this.apu.NR21 = value;
                        break;

                    case 0xFF17:
                        this.apu.NR22 = value;
                        break;

                    case 0xFF18:
                        this.apu.NR23 = value;
                        break;

                    case 0xFF19:
                        this.apu.NR24 = value;
                        break;

                    case 0xFF1A:
                        this.apu.NR30 = value;
                        break;

                    case 0xFF1B:
                        this.apu.NR31 = value;
                        break;

                    case 0xFF1C:
                        this.apu.NR32 = value;
                        break;

                    case 0xFF1D:
                        this.apu.NR33 = value;
                        break;

                    case 0xFF1E:
                        this.apu.NR34 = value;
                        break;

                    case 0xFF1F:
                        this.Unsupported(i, 0xFF);
                        break;

                    case 0xFF20:
                        this.apu.NR41 = value;
                        break;

                    case 0xFF21:
                        this.apu.NR42 = value;
                        break;

                    case 0xFF22:
                        this.apu.NR43 = value;
                        break;

                    case 0xFF23:
                        this.apu.NR44 = value;
                        break;

                    case 0xFF24:
                        this.apu.NR50 = value;
                        break;

                    case 0xFF25:
                        this.apu.NR51 = value;
                        break;

                    case 0xFF26:
                        this.apu.NR52 = value;
                        break;

                    case >= 0xFF27 and <= 0xFF2F:
                        this.Unsupported(i, 0xFF);
                        break;

                    case >= 0xFF30 and <= 0xFF3F:
                        if ( !this.apu.Channel3.LengthStatus )
                        {
                            this.apu.WaveTable[i - 0xFF30] = value;
                        }
                        break;

                    case 0xFF40:
                        this.gpu.SetLCDC(value);
                        break;

                    case 0xFF41:
                        this.gpu.stat = value;
                        break;

                    case 0xFF42:
                        this.gpu.scrollY = value;
                        break;

                    case 0xFF43:
                        this.gpu.scrollX = value;
                        break;

                    case 0xFF44:
                        this.Unsupported(i, 0xFF); // Scanline counter is read-only
                        break;

                    case 0xFF45:
                        this.gpu.rLYC = value;
                        break;

                    case 0xFF46:
                        this.gpu.TriggerDMA(value);
                        break;

                    case 0xFF47:
                        this.gpu.bgPal = value;
                        break;

                    case 0xFF48:
                        this.gpu.objPal0 = value;
                        break;

                    case 0xFF49:
                        this.gpu.objPal1 = value;
                        break;

                    case 0xFF4A:
                        this.gpu.winY = value;
                        break;

                    case 0xFF4B:
                        this.gpu.winX = value;
                        break;

                    case >= 0xFF4C and <= 0xFF4F:
                        this.Unsupported(i, 0xFF);
                        break;

                    case 0xFF50:
                        this.rom[0xFF50] = value; // Bootstrap completed
                        break;

                    case >= 0xFF51 and <= 0xFF7F:
                        this.Unsupported(i, 0xFF); // Unsupported/unused (mostly CGB stuff)
                        break;

                    case >= 0xFF80 and <= 0xFFFE:
                        this.hram[i - 0xff80] = value; // High RAM
                        break;

                    case 0xFFFF:
                        this.cpu.rInterruptEnable = value;
                        break;
                }
            }
        }

        public void SetShort(int address, gshort value)
        {
            this[address] = value.Lo;
            this[address + 1] = value.Hi;
        }

        public gshort GetShort(int address) => (ushort)((this[address + 1] << 8) + this[address]);
    }
}
