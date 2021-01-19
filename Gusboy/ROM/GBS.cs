namespace Gusboy
{
    using System;
    using System.Linq;

    /// <summary>
    /// A mapper that adds GBS support to the emulator.
    /// This implementation uses https://github.com/frestr/gbs_player/ as a reference.
    /// </summary>
    public class GBS : Mapper
    {
        private readonly Gameboy gb;
        private readonly ROM rom;

        private readonly byte[] memory = new byte[0xFFFF];
        private int initialStackPointer;
        private int interruptCounter;
        private long oldCpuTicks;
        private int interruptRate;
        private int currentSong;

        public GBS(byte[] romFile, Gameboy gb, ROM rom)
            : base(romFile, null, null)
        {
            this.Version = romFile[0x03];
            this.NumSongs = romFile[0x04];
            this.FirstSong = romFile[0x05];
            this.LoadAddress = romFile[0x07] << 8 | romFile[0x06];
            this.InitAddress = romFile[0x09] << 8 | romFile[0x08];
            this.PlayAddress = romFile[0x0B] << 8 | romFile[0x0A];
            this.StackPointer = romFile[0x0D] << 8 | romFile[0x0C];
            this.TimerModulo = romFile[0x0E];
            this.TimerControl = romFile[0x0F];
            this.Title = ROM.GetRomString(romFile, 0x10, 0x20);
            this.Author = ROM.GetRomString(romFile, 0x30, 0x20);
            this.Copyright = ROM.GetRomString(romFile, 0x50, 0x20);

            this.gb = gb;
            this.rom = rom;

            if ((this.TimerControl & 0b1000_0000) != 0)
            {
                rom.hCGB = ROM.CGBType.Yes;

                // GBS files won't switch the CPU speed themselves, so we have to force it for them.
                this.gb.Cpu.fSpeed = true;
            }
            else
            {
                rom.hCGB = ROM.CGBType.No;
            }

            // TODO: Draw something to the framebuffer to indicate it's a GBS file?
            this.gb.MessageCallback($"Title:   {this.Title}");
            this.gb.MessageCallback($"Version: {this.Version}");
            this.gb.MessageCallback($"Author:  {this.Author}");
            this.gb.MessageCallback($"©:       {this.Author}");
            this.gb.MessageCallback($"Colour:  {rom.hCGB}");
            this.gb.MessageCallback($"Songs:   {this.NumSongs}");
            this.gb.MessageCallback($"Playing: {this.currentSong}");

            this.gb.UseBios = false;

            // Remove the GBS header since all addresses/offsets in the future are relative to it
            this.RomFile = romFile.Skip(0x70).ToArray();

            // Load the memory space, it will be rewritten as required.
            Array.Copy(this.RomFile, 0, this.memory, this.LoadAddress, Math.Min(0x8000, this.RomFile.Length));

            this.SetSong(this.FirstSong);

            this.SetInterruptRate();
        }

        public bool InitDone { get; set; }

        public int Version { get; set; }

        public int NumSongs { get; set; }

        public byte FirstSong { get; set; }

        public int LoadAddress { get; set; }

        public int InitAddress { get; set; }

        public int PlayAddress { get; set; }

        public int StackPointer { get; set; }

        public byte TimerModulo { get; set; }

        public byte TimerControl { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Copyright { get; set; }

        public void Init()
        {
            this.gb.Cpu.FakeBootstrap();
            this.rom.InitializeState();
            this.gb.Apu.Initialize();
            this.gb.Ram.ClearRAM();
            Array.Copy(Enumerable.Repeat((byte)0x00, 0x2000).ToArray(), 0, this.memory, 0xA000, 0x2000);

            this.gb.Cpu.InitGbs((byte)(this.currentSong - 1), this.StackPointer);

            this.initialStackPointer = this.StackPointer;
            this.gb.Cpu.rPC = this.InitAddress;

            this.gb.Cpu.rTMA = this.TimerModulo;
            this.gb.Cpu.rTAC = this.TimerControl;

            this.InitDone = false;
        }

        public void Tick()
        {
            this.interruptCounter += (int)(this.gb.Cpu.Ticks - this.oldCpuTicks);
            this.oldCpuTicks = this.gb.Cpu.Ticks;

            if ((this.ProcedureDone() && !this.InitDone) || (this.InitDone && this.interruptCounter >= this.interruptRate))
            {
                this.interruptCounter = 0;
                this.SetInterruptRate();
                this.InitDone = true;
                this.Play();
            }
        }

        public void KeyDown(Input.Keys key)
        {
            if (key == Input.Keys.Left)
            {
                this.PrevSong();
            }

            if (key == Input.Keys.Right)
            {
                this.NextSong();
            }
        }

        public void Play()
        {
            if (this.ProcedureDone())
            {
                this.gb.Cpu.rSP = this.initialStackPointer;
            }
            else
            {
                this.initialStackPointer = this.gb.Cpu.rSP;
            }

            this.gb.Cpu.rPC = this.PlayAddress;
            this.gb.Cpu.fHalt = false;
        }

        public bool ProcedureDone() => this.gb.Cpu.rSP > this.initialStackPointer;

        public override byte Read(int address)
        {
            return this.memory[address];
        }

        public override void Write(int address, byte value)
        {
            if (address <= 0x7FFF)
            {
                if (value == 0)
                {
                    value = 1;
                }

                int pageOffset;

                if (this.LoadAddress < 0x4000)
                {
                    pageOffset = (0x4000 - this.LoadAddress) + (0x4000 * (value - 1));
                }
                else if (value == 1)
                {
                    pageOffset = 0;
                }
                else
                {
                    pageOffset = (0x8000 - this.LoadAddress) + (0x4000 * (value - 2));
                }

                // TODO: Is this a bug in the GBS, or in my emulation? Donkey Kong Country jumps to bank 53 despite its highest bank number being 2.
                if (pageOffset >= this.RomFile.Length)
                {
                    this.gb.MessageCallback($"Invalid bank switch ({pageOffset:X4}): {value}");
                    return;
                }

                // Maybe optimize this to only zero out the missing part of a final bank.
                Array.Copy(Enumerable.Repeat((byte)0x00, 0x4000).ToArray(), 0, this.memory, 0x4000, 0x4000);
                Array.Copy(this.RomFile, pageOffset, this.memory, 0x4000, Math.Min(0x4000, this.RomFile.Length - pageOffset));
            }
            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                this.memory[address] = value;
            }
        }

        public void NextSong()
        {
            this.SetSong(this.currentSong + 1);
        }

        public void PrevSong()
        {
            this.SetSong(this.currentSong - 1);
        }

        private void SetSong(int songNum)
        {
            this.currentSong = songNum;

            if (this.currentSong < 1)
            {
                this.currentSong = 1;
            }

            if (this.currentSong > this.NumSongs)
            {
                this.currentSong = this.NumSongs;
            }

            this.gb.MessageCallback($"Playing: {this.currentSong}", true);

            this.Init();
        }

        private void SetInterruptRate()
        {
            // TODO: The CPU should do some of this math.
            int clock = this.gb.Cpu.fSpeed ? 8388608 : 4194304;

            // Use v-blank interrupt timing (59.7 Hz)
            // TODO: Drive this from the GPU somehow? Or by the APU? The math here isn't going to line up to the APU sampling.
            if (((this.gb.Cpu.rTAC >> 2) & 1) == 0)
            {
                this.interruptRate = (int)(clock / 59.7275);
                return;
            }

            // Use timer interrupt
            // TODO: Drive this from the actual CPU timer somehow?
            int counter_rate = (this.gb.Cpu.rTAC & 0b11) switch
            {
                0 => 4096,
                1 => 262144,
                2 => 65536,
                3 => 16384,
                _ => 4096,
            };

            this.interruptRate = clock / (counter_rate / (256 - this.gb.Cpu.rTMA));
            return;
        }
    }
}
