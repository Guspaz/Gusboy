namespace GusBoy
{
    using System;

    public class Gameboy
    {
        public Gameboy(Func<string, bool> messageCallback, Func<bool> drawFramebuffer, int[] framebuffer, int sampleRate)
        {
            this.MessageCallback = messageCallback;

            this.Ram = new RAM(this);
            this.Cpu = new CPU(this);
            this.Gpu = new GPU(this, drawFramebuffer, framebuffer);
            this.Apu = new APU(this, sampleRate);
            this.Input = new Input(this);

            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\mooneye\acceptance\interrupts\ie_push.gb", false);

            // Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\cpu_instrs\cpu_instrs.gb", false); // PASS
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\instr_timing\instr_timing.gb", false); // PASS

            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dr_mario.gb", false);
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\ff_legend_3.gb", false);
            // this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\kirby.gb", false);

            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\marioland.gb", false);
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\marioland2.gb", false);
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\pokemon_blue.gb", false);
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\tetris.gb", false);
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\zelda.gb", false);
            this.Rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\zelda_dx.gbc", false);

            // Blargg tests
            // rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\dmg_sound.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\03-trigger.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\04-sweep.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\05-sweep details.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\09-wave read while on.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\10-wave trigger while on.gb", false);
            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\12-wave write while on.gb", false);

            // rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\blargg\dmg_sound-2.gb", false);
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

        public CPU Cpu { get; }

        public GPU Gpu { get; }

        public APU Apu { get; }

        public ROM Rom { get; }

        public RAM Ram { get; }

        public Input Input { get; }

        internal Func<string, bool> MessageCallback { get; }

        public void Tick()
        {
            if (this.Cpu.fHalt || this.Cpu.fStop)
            {
                this.Cpu.Ticks += 4;
            }
            else if (!this.Cpu.fStop)
            {
                this.Cpu.Tick();
            }

            this.Gpu.Tick();

            this.Apu.Tick();

            this.Cpu.TimerTick();

            this.Cpu.InterruptTick();
        }
    }
}