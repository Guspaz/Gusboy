using System.IO;

namespace GusBoy
{
    public partial class ROM
    {
        public string hTitle = string.Empty;

        private Gameboy gb;
        private Mapper mapper;
        private byte[] biosFile;
        private bool biosFlag;

        public ROM(Gameboy gameBoy, string filepath, bool useBios)
        {
            this.gb = gameBoy;

            // Load the ROM file
            var romFile = File.ReadAllBytes(filepath);

            ReadHeader(romFile);

            if (romFile.Length != hRomSize)
            {
                throw new gbException($"ROM file size did not match header ROM size: {romFile.Length} != {hRomSize}");
            }

            var sram = new byte[hRamSize];

            var ramPath = Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath)) + ".sav";

            Bootstrap(useBios);

            switch(hType)
            {
                case CartridgeType.ROM_ONLY:
                    mapper = new DirectRom(romFile);
                    break;
                case CartridgeType.MBC1:
                case CartridgeType.MBC1_RAM:
                case CartridgeType.MBC1_RAM_BATTERY:
                    mapper = new MBC1(romFile, sram, ramPath);
                    break;
                // Treat MBC3 as MBC5 for now
                case CartridgeType.MBC3:
                case CartridgeType.MBC3_RAM:
                case CartridgeType.MBC3_RAM_BATTERY:
                case CartridgeType.MBC3_TIMER_BATTERY:
                case CartridgeType.MBC3_TIMER_RAM_BATTERY:
                case CartridgeType.MBC5:
                case CartridgeType.MBC5_RAM:
                case CartridgeType.MBC5_RAM_BATTERY:
                case CartridgeType.MBC5_RUMBLE:
                case CartridgeType.MBC5_RUMBLE_RAM:
                case CartridgeType.MBC5_RUMBLE_RAM_BATTERY:
                    mapper = new MBC5(romFile, sram, ramPath);
                    break;
                default:
                    throw new gbException("Unsupported mapper type: " + hType.ToString());
            }

            PrintInfo();
        }

        private void PrintInfo()
        {
            gb.messageCallback($"Title:    {hTitle}");
            gb.messageCallback($"Licensee: {hLicensee}");
            gb.messageCallback($"Version:  {hMaskRomVersion}");
            gb.messageCallback($"Region:   {(hDestination ? "International" : "Japan")}");
            gb.messageCallback($"ROM Size: {hRomSize/1024} KiB");
            gb.messageCallback($"RAM Size: {hRamSize/1024} KiB");
            gb.messageCallback($"Mapper:   {hType}");
            gb.messageCallback($"Colour:   {hCGB.ToString()}");
            gb.messageCallback($"SGB:      {(hSGB ? "Yes" : "No")}");
            gb.messageCallback($"Header #: 0x{hHeaderChecksum:X2}");
            gb.messageCallback($"Global #: 0x{hGlobalChecksum:X4}");
        }

        public void SaveSRAM() => mapper.SaveSRAM();

        public byte this[int i]
        {
            get
            {
                if (biosFlag && i <= 0x100)
                {
                    return biosFile[i];
                }
                else
                {
                    return mapper.Read(i);
                }
            }
            set
            {
                if (i == 0xFF50 && value == 1)
                {
                    biosFlag = false;
                }
                else
                {
                    mapper.Write(i, value);
                }
            }
        }

        private void Bootstrap(bool useBios)
        {
            if (useBios && File.Exists("bios.gb"))
            {
                biosFile = File.ReadAllBytes("bios.gb");
                biosFlag = true;
            }
            else
            {
                gb.ram[0xFF05] = 0;
                gb.ram[0xFF06] = 0;
                gb.ram[0xFF07] = 0;
                gb.ram[0xFF10] = 0x80;
                gb.ram[0xFF11] = 0xBF;
                gb.ram[0xFF12] = 0xF3;
                gb.ram[0xFF14] = 0xBF;
                gb.ram[0xFF16] = 0x3F;
                gb.ram[0xFF17] = 0x00;
                gb.ram[0xFF19] = 0xBF;
                gb.ram[0xFF1A] = 0x7F;
                gb.ram[0xFF1B] = 0xFF;
                gb.ram[0xFF1C] = 0x9F;
                gb.ram[0xFF1E] = 0xBF;
                gb.ram[0xFF20] = 0xFF;
                gb.ram[0xFF21] = 0x00;
                gb.ram[0xFF22] = 0x00;
                gb.ram[0xFF23] = 0xBF;
                gb.ram[0xFF24] = 0x77;
                gb.ram[0xFF25] = 0xF3;
                gb.ram[0xFF26] = 0xF1;
                gb.ram[0xFF40] = 0x91;
                gb.ram[0xFF42] = 0x00;
                gb.ram[0xFF43] = 0x00;
                gb.ram[0xFF45] = 0x00;
                gb.ram[0xFF47] = 0xFC;
                gb.ram[0xFF48] = 0xFF;
                gb.ram[0xFF49] = 0xFF;
                gb.ram[0xFF4A] = 0x00;
                gb.ram[0xFF4B] = 0x00;
                gb.ram[0xFFFF] = 0x00;

                // Init timer to roughly where it ought to start
                gb.cpu.rDIV = 0xABD0;

                gb.cpu.rPC = 0x100;
            }
        }
    }
}
