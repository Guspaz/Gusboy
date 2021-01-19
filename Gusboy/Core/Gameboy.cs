[assembly: System.Resources.NeutralResourcesLanguageAttribute("en")]

namespace Gusboy
{
    using System;

    public class Gameboy
    {
        private readonly Func<string, bool, bool> messageCallback;

        public Gameboy(Func<string, bool, bool> messageCallback, Func<bool> drawFramebuffer, int[] framebuffer, int sampleRate)
        {
            this.messageCallback = messageCallback;

            this.Ram = new RAM(this);
            this.Cpu = new CPU(this);
            this.Gpu = new GPU(this, drawFramebuffer, framebuffer);
            this.Apu = new APU(this, sampleRate);
            this.Input = new Input(this);

            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\mooneye\acceptance\interrupts\ie_push.gb", false);
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\instr_timing\instr_timing.gb", false); // PASS
            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\cpu_instrs\cpu_instrs.gb", false); // PASS

            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dr_mario.gb", false);
            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\ff_legend_3.gb", false);
            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\kirby.gb", false);

            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\marioland.gb", false);
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\marioland2.gb", false);
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\pokemon_blue.gb", false);
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\tetris.gb", false);
            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\Mega Man V (USA) (SGB Enhanced).gb", false); // Some off-by-one scanline issues
            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\Legend of Zelda, The - Oracle of Seasons (USA).gbc");
            this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\gbs\Donkey Kong Country.gbs");

            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\Shantae (USA).gbc");
            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\zelda_dx.gbc");

            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\Final Fantasy Adventure (USA).gb", false); // Super high-pitched whine from channel 2

            // The following issues are resolved by bypassing CanAccessVRAM() so I think there's a timing bug, they shouldn't be doing this.
            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\Final Fantasy Legend II (USA).gb", false); // Corrupt background tiles on title screen, hint of squeal
            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\Final Fantasy Legend, The (USA).gb", false); // Garbled title screen
            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\Wario Land II (USA, Europe) (SGB Enhanced).gb", false); // Off-by-one scanline issues
            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\Yoshi's Cookie (USA, Europe).gb");

            // Blargg tests
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\dmg_sound.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\03-trigger.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\04-sweep.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\05-sweep details.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\09-wave read while on.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\10-wave trigger while on.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\12-wave write while on.gb", false);
            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\blargg\dmg_sound-2.gb", false);

            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\blargg\dmg_sound-2\04-sweep.gb", false);
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\blargg\dmg_sound-2\01-registers.gb", false);
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\blargg\interrupt_time.gb", false);

            // Gekkio tests
            // Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\mooneye\acceptance\timer\tima_reload.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\bits_bank1.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\bits_bank2.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\bits_mode.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\bits_ramg.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\multicart_rom_8Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\ram_64kb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\ram_256kb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\rom_1Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\rom_2Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\rom_4Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\rom_8Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\rom_16Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\rom_512kb.gb", false);

            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_1Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_2Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_4Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_8Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_16Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_32Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_64Mb.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_512kb.gb", false);

            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\manual-only\sprite_priority.gb", false);

            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\Prehistorik Man (USA, Europe).gb", true);
        }

        public long CpuTicks => this.Cpu.Ticks;

        public System.Collections.Generic.List<float> AudioBuffer => this.Apu.Buffer;

        internal bool IsCgb { get; set; }

        internal bool UseFilter { get; } = true;

        internal bool UseBios { get; set; } = true;

        internal CPU Cpu { get; }

        internal GPU Gpu { get; }

        internal APU Apu { get; }

        internal ROM Rom { get; }

        internal RAM Ram { get; }

        internal Input Input { get; }

        public void KeyDown(Input.Keys key) => this.Input.KeyDown(key);

        public void KeyUp(Input.Keys key) => this.Input.KeyUp(key);

        public void Tick()
        {
            if (this.Cpu.fHalt || this.Cpu.fStop || (this.Rom.IsGbs && this.Rom.Gbs.ProcedureDone()))
            {
                this.Cpu.Ticks += 4;
            }
            else if (!this.Cpu.fStop)
            {
                this.Cpu.Tick();
            }

            // This was originally at the end, but apparently is checked right after instruction execution?
            this.Cpu.InterruptTick();

            this.Gpu.Tick();

            this.Apu.Tick();

            this.Cpu.TimerTick();

            if (this.Rom.IsGbs)
            {
                this.Rom.Gbs.Tick();
            }
        }

        internal bool MessageCallback(string message, bool removePrevious = false) => this.messageCallback(message, removePrevious);
    }
}