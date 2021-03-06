﻿namespace Gusboy
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// Main ROM support.
    /// </summary>
    public partial class ROM
    {
        private readonly Mapper mapper;

        private readonly Gameboy gb;
        private byte[] biosFile;
        private bool biosFlag;

        public ROM(Gameboy gameBoy, string filepath)
        {
            this.gb = gameBoy;

            // Load the ROM file
            byte[] romFile = File.ReadAllBytes(filepath);

            // Check for GBS file, need to replace the rom file
            if (Encoding.ASCII.GetString(romFile, 0, 3) == "GBS")
            {
                this.IsGbs = true;
                this.Gbs = new GBS(romFile, this.gb, this);
                this.mapper = this.Gbs;

                return;
            }

            this.ReadHeader(romFile);

            this.PrintInfo();

            if (romFile.Length != this.hRomSize)
            {
                this.gb.MessageCallback($"Malformed ROM file, invalid ROM size header ({romFile.Length} != {this.hRomSize}), using compatibility mode");
                byte[] oldRomFile = romFile;
                romFile = new byte[0x80_0000];
                Array.Copy(oldRomFile, 0, romFile, 0, oldRomFile.Length);
            }

            byte[] sram;

            if (this.hRamSize == 0 && this.hType.ToString().Contains("_RAM"))
            {
                sram = new byte[this.ramSizeTable[1]];
                this.gb.MessageCallback($"Malformed ROM file, invalid RAM size header (mapper is {this.hType} but RAM size is 0), using compatibility mode");
            }
            else
            {
                sram = new byte[this.hRamSize];
            }

            string ramPath = Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath)) + ".sav";

            this.InitializeState();

            this.mapper = this.hType switch
            {
                CartridgeType.ROM_ONLY => new DirectRom(romFile),
                CartridgeType.MBC1 or
                CartridgeType.MBC1_RAM or
                CartridgeType.MBC1_RAM_BATTERY => new MBC1(romFile, sram, ramPath),

                CartridgeType.MBC3 or
                CartridgeType.MBC3_RAM or
                CartridgeType.MBC3_RAM_BATTERY or
                CartridgeType.MBC3_TIMER_BATTERY or
                CartridgeType.MBC3_TIMER_RAM_BATTERY => new MBC3(romFile, sram, ramPath, this.gb),

                CartridgeType.MBC5 or
                CartridgeType.MBC5_RAM or
                CartridgeType.MBC5_RAM_BATTERY or
                CartridgeType.MBC5_RUMBLE or
                CartridgeType.MBC5_RUMBLE_RAM or
                CartridgeType.MBC5_RUMBLE_RAM_BATTERY => new MBC5(romFile, sram, ramPath),

                // Treat unknown as MBC5 for now
                _ => new MBC5(romFile, sram, ramPath),
            };
        }

        // Special case, this mapper needs to influence outside the ROM, so it gets its own property.
        public GBS Gbs { get; }

        public bool IsGbs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            set;
        }

        public byte this[int i]
        {
            get
            {
                if (this.biosFlag)
                {
                    if (i < 0x100 || (this.gb.IsCgb && i >= 0x200 && i < 0x900))
                    {
                        return this.biosFile[i];
                    }
                }

                return this.mapper.Read(i);
            }

            set
            {
                if (i == 0xFF50 && value != 0)
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

        internal void InitializeState()
        {
            // Determine CGB mode based on the header
            if (this.hCGB == CGBType.Yes || this.hCGB == CGBType.Both)
            {
                this.gb.IsCgb = true;
            }

            this.gb.Apu.Initialize();

            string filename = this.gb.IsCgb ? "cgb.bin" : "dmg.bin";

            if (this.gb.UseBios && File.Exists(filename))
            {
                this.biosFile = File.ReadAllBytes(filename);
                this.biosFlag = true;
            }
            else
            {
                this.gb.Cpu.FakeBootstrap();

                this.gb.Ram[0xFF05] = 0;
                this.gb.Ram[0xFF06] = 0;
                this.gb.Ram[0xFF07] = 0;
                this.gb.Apu.FakeBootstrap();
                this.gb.Ram[0xFF42] = 0x00;
                this.gb.Ram[0xFF43] = 0x00;
                this.gb.Ram[0xFF45] = 0x00;
                this.gb.Ram[0xFF47] = 0xFC;
                this.gb.Ram[0xFF48] = 0xFF;
                this.gb.Ram[0xFF49] = 0xFF;
                this.gb.Ram[0xFF4A] = 0x00;
                this.gb.Ram[0xFF4B] = 0x00;
                this.gb.Ram[0xFFFF] = 0x00;
                this.gb.Gpu.FakeBootstrap();
            }
        }

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
    }
}
