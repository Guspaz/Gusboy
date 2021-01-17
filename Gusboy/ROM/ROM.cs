namespace Gusboy
{
    using System;
    using System.IO;

    /// <summary>
    /// Main ROM support.
    /// </summary>
    public partial class ROM
    {
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

            if (romFile.Length != this.hRomSize)
            {
                throw new Exception($"ROM file size did not match header ROM size: {romFile.Length} != {this.hRomSize}");
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
                _ => new MBC5(romFile, sram, ramPath),
            };

            this.PrintInfo();
        }

        public byte this[int i]
        {
            get
            {
                if (this.biosFlag && i <= 0x100)
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
                if (i == 0xFF50 && value == 1)
                {
                    this.biosFlag = false;
                }
                else
                {
                    this.mapper.Write(i, value);
                }
            }
        }

        public void SaveSRAM() => this.mapper.SaveSRAM();

        private void PrintInfo()
        {
            this.gb.MessageCallback($"Title:    {this.hTitle}");
            this.gb.MessageCallback($"Licensee: {this.hLicensee}");
            this.gb.MessageCallback($"Version:  {this.hMaskRomVersion}");
            this.gb.MessageCallback($"Region:   {(this.hDestination ? "International" : "Japan")}");
            this.gb.MessageCallback($"ROM Size: {this.hRomSize / 1024} KiB");
            this.gb.MessageCallback($"RAM Size: {this.hRamSize / 1024} KiB");
            this.gb.MessageCallback($"Mapper:   {this.hType}");
            this.gb.MessageCallback($"Colour:   {this.hCGB}");
            this.gb.MessageCallback($"SGB:      {(this.hSGB ? "Yes" : "No")}");
            this.gb.MessageCallback($"Header #: 0x{this.hHeaderChecksum:X2}");
            this.gb.MessageCallback($"Global #: 0x{this.hGlobalChecksum:X4}");
        }

        private void Bootstrap(bool useBios)
        {
            if (useBios && File.Exists("bios.gb"))
            {
                this.biosFile = File.ReadAllBytes("bios.gb");
                this.biosFlag = true;
            }
            else
            {
                this.gb.Ram[0xFF05] = 0;
                this.gb.Ram[0xFF06] = 0;
                this.gb.Ram[0xFF07] = 0;
                this.gb.Ram[0xFF10] = 0x80;
                this.gb.Ram[0xFF11] = 0xBF;
                this.gb.Ram[0xFF12] = 0xF3;
                this.gb.Ram[0xFF14] = 0xBF;
                this.gb.Ram[0xFF16] = 0x3F;
                this.gb.Ram[0xFF17] = 0x00;
                this.gb.Ram[0xFF19] = 0xBF;
                this.gb.Ram[0xFF1A] = 0x7F;
                this.gb.Ram[0xFF1B] = 0xFF;
                this.gb.Ram[0xFF1C] = 0x9F;
                this.gb.Ram[0xFF1E] = 0xBF;
                this.gb.Ram[0xFF20] = 0xFF;
                this.gb.Ram[0xFF21] = 0x00;
                this.gb.Ram[0xFF22] = 0x00;
                this.gb.Ram[0xFF23] = 0xBF;
                this.gb.Ram[0xFF24] = 0x77;
                this.gb.Ram[0xFF25] = 0xF3;
                this.gb.Ram[0xFF26] = 0xF1;
                this.gb.Ram[0xFF40] = 0x91;
                this.gb.Ram[0xFF42] = 0x00;
                this.gb.Ram[0xFF43] = 0x00;
                this.gb.Ram[0xFF45] = 0x00;
                this.gb.Ram[0xFF47] = 0xFC;
                this.gb.Ram[0xFF48] = 0xFF;
                this.gb.Ram[0xFF49] = 0xFF;
                this.gb.Ram[0xFF4A] = 0x00;
                this.gb.Ram[0xFF4B] = 0x00;
                this.gb.Ram[0xFFFF] = 0x00;

                // Init timer to roughly where it ought to start
                this.gb.Cpu.rDIV = 0xABD0;

                this.gb.Cpu.rPC = 0x100;
            }
        }
    }
}
