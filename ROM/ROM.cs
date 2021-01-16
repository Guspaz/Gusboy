using System.IO;

namespace GusBoy
{
    public partial class ROM
    {
        public string hTitle = string.Empty;

        private readonly Gameboy gb;
        private readonly Mapper mapper;
        private byte[] biosFile;
        private bool biosFlag;

        public ROM(Gameboy gameBoy, string filepath, bool useBios)
        {
            this.gb = gameBoy;

            // Load the ROM file
            byte[] romFile = File.ReadAllBytes(filepath);

            this.ReadHeader(romFile);

            if ( romFile.Length != this.hRomSize )
            {
                throw new GbException($"ROM file size did not match header ROM size: {romFile.Length} != {this.hRomSize}");
            }

            byte[] sram = new byte[this.hRamSize];

            string ramPath = Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath)) + ".sav";

            this.Bootstrap(useBios);

            this.mapper = this.hType switch
            {
                CartridgeType.ROM_ONLY => new DirectRom(romFile),
                CartridgeType.MBC1 or
                CartridgeType.MBC1_RAM or
                CartridgeType.MBC1_RAM_BATTERY => new MBC1(romFile, sram, ramPath),
                // Treat MBC3 as MBC5 for now
                CartridgeType.MBC3 or
                CartridgeType.MBC3_RAM or
                CartridgeType.MBC3_RAM_BATTERY or
                CartridgeType.MBC3_TIMER_BATTERY or
                CartridgeType.MBC3_TIMER_RAM_BATTERY or
                CartridgeType.MBC5 or
                CartridgeType.MBC5_RAM or
                CartridgeType.MBC5_RAM_BATTERY or
                CartridgeType.MBC5_RUMBLE or
                CartridgeType.MBC5_RUMBLE_RAM or
                CartridgeType.MBC5_RUMBLE_RAM_BATTERY => new MBC5(romFile, sram, ramPath),
                _ => throw new GbException("Unsupported mapper type: " + this.hType.ToString()),
            };

            this.PrintInfo();
        }

        private void PrintInfo()
        {
            this.gb.messageCallback($"Title:    {this.hTitle}");
            this.gb.messageCallback($"Licensee: {this.hLicensee}");
            this.gb.messageCallback($"Version:  {this.hMaskRomVersion}");
            this.gb.messageCallback($"Region:   {(this.hDestination ? "International" : "Japan")}");
            this.gb.messageCallback($"ROM Size: {this.hRomSize / 1024} KiB");
            this.gb.messageCallback($"RAM Size: {this.hRamSize / 1024} KiB");
            this.gb.messageCallback($"Mapper:   {this.hType}");
            this.gb.messageCallback($"Colour:   {this.hCGB}");
            this.gb.messageCallback($"SGB:      {(this.hSGB ? "Yes" : "No")}");
            this.gb.messageCallback($"Header #: 0x{this.hHeaderChecksum:X2}");
            this.gb.messageCallback($"Global #: 0x{this.hGlobalChecksum:X4}");
        }

        public void SaveSRAM() => this.mapper.SaveSRAM();

        public byte this[int i]
        {
            get
            {
                if ( this.biosFlag && i <= 0x100 )
                {
                    return this.biosFile[i];
                }
                else
                {
                    return this.mapper.Read(i);
                }
            }
            set
            {
                if ( i == 0xFF50 && value == 1 )
                {
                    this.biosFlag = false;
                }
                else
                {
                    this.mapper.Write(i, value);
                }
            }
        }

        private void Bootstrap(bool useBios)
        {
            if ( useBios && File.Exists("bios.gb") )
            {
                this.biosFile = File.ReadAllBytes("bios.gb");
                this.biosFlag = true;
            }
            else
            {
                this.gb.ram[0xFF05] = 0;
                this.gb.ram[0xFF06] = 0;
                this.gb.ram[0xFF07] = 0;
                this.gb.ram[0xFF10] = 0x80;
                this.gb.ram[0xFF11] = 0xBF;
                this.gb.ram[0xFF12] = 0xF3;
                this.gb.ram[0xFF14] = 0xBF;
                this.gb.ram[0xFF16] = 0x3F;
                this.gb.ram[0xFF17] = 0x00;
                this.gb.ram[0xFF19] = 0xBF;
                this.gb.ram[0xFF1A] = 0x7F;
                this.gb.ram[0xFF1B] = 0xFF;
                this.gb.ram[0xFF1C] = 0x9F;
                this.gb.ram[0xFF1E] = 0xBF;
                this.gb.ram[0xFF20] = 0xFF;
                this.gb.ram[0xFF21] = 0x00;
                this.gb.ram[0xFF22] = 0x00;
                this.gb.ram[0xFF23] = 0xBF;
                this.gb.ram[0xFF24] = 0x77;
                this.gb.ram[0xFF25] = 0xF3;
                this.gb.ram[0xFF26] = 0xF1;
                this.gb.ram[0xFF40] = 0x91;
                this.gb.ram[0xFF42] = 0x00;
                this.gb.ram[0xFF43] = 0x00;
                this.gb.ram[0xFF45] = 0x00;
                this.gb.ram[0xFF47] = 0xFC;
                this.gb.ram[0xFF48] = 0xFF;
                this.gb.ram[0xFF49] = 0xFF;
                this.gb.ram[0xFF4A] = 0x00;
                this.gb.ram[0xFF4B] = 0x00;
                this.gb.ram[0xFFFF] = 0x00;

                // Init timer to roughly where it ought to start
                this.gb.cpu.rDIV = 0xABD0;

                this.gb.cpu.rPC = 0x100;
            }
        }
    }
}
