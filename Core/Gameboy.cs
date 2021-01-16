using System;

namespace GusBoy
{
    public class GbException : Exception
    {
        public GbException(string message) : base(message)
        {
        }
    }

    public partial class Gameboy
    {
        internal Func<string, bool> messageCallback;

        public CPU cpu;
        public GPU gpu;
        public APU apu;
        public ROM rom;
        public RAM ram;
        public Input input;

        public Gameboy(Func<string, bool> messageCallback, Func<bool> drawFramebuffer, int[] framebuffer, int sampleRate)
        {
            this.messageCallback = messageCallback;

            this.ram = new RAM(this);
            this.cpu = new CPU(this);
            this.gpu = new GPU(this, drawFramebuffer, framebuffer);
            this.apu = new APU(this, sampleRate);
            this.input = new Input(this);

            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\mooneye\acceptance\interrupts\ie_push.gb", false);

            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\cpu_instrs\cpu_instrs.gb", false); // PASS
            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\instr_timing\instr_timing.gb", false); // PASS

            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dr_mario.gb", false);
            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\ff_legend_3.gb", false);
            this.rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\kirby.gb", false);
            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\marioland.gb", false);
            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\marioland2.gb", false);
            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\pokemon_blue.gb", false);
            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\tetris.gb", false);
            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\zelda.gb", false);
            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\zelda_dx.gbc", false);

            // Blargg tests
            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\dmg_sound.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\03-trigger.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\04-sweep.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\05-sweep details.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\09-wave read while on.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\10-wave trigger while on.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\dmg_sound\12-wave write while on.gb", false);

            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\blargg\dmg_sound-2.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\blargg\dmg_sound-2\04-sweep.gb", false);
            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\blargg\dmg_sound-2\01-registers.gb", false);
            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\test roms\blargg\interrupt_time.gb", false);


            // Gekkio tests
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\bits_bank1.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\bits_bank2.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\bits_mode.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\bits_ramg.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\multicart_rom_8Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\ram_64kb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\ram_256kb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\rom_1Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\rom_2Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\rom_4Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\rom_8Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\rom_16Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc1\rom_512kb.gb", false);

            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_1Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_2Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_4Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_8Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_16Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_32Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_64Mb.gb", false);
            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\emulator-only\mbc5\rom_512kb.gb", false);

            //rom = new ROM(ram, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\mooneye-gb_hwtests\manual-only\sprite_priority.gb", false);

            //rom = new ROM(this, @"H:\Backups\Intel\files\Users\Adam\Desktop\gbc\nondumped\Prehistorik Man (USA, Europe).gb", true);
        }

        public void Tick()
        {
            if ( this.cpu.fHalt || this.cpu.fStop )
            {
                this.cpu.ticks += 4;
            }
            else if ( !this.cpu.fStop )
            {
                this.cpu.Tick();
            }

            this.gpu.Tick();

            this.apu.Tick();

            this.cpu.TimerTick();

            this.cpu.InterruptTick();
        }
    }
}