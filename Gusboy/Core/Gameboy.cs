[assembly: System.Resources.NeutralResourcesLanguageAttribute("en")]

namespace Gusboy
{
    using System;

    public class Gameboy
    {
        private readonly Func<string, bool, bool> messageCallback;

        public Gameboy(Func<string, bool, bool> messageCallback, Func<bool> drawFramebuffer, int[] framebuffer, int sampleRate, string filePath)
        {
            this.messageCallback = messageCallback;

            this.Ram = new RAM(this);
            this.Cpu = new CPU(this);
            this.Gpu = new GPU(this, drawFramebuffer, framebuffer);
            this.Apu = new APU(this, sampleRate);
            this.Input = new Input(this);
            this.Rom = new ROM(this, filePath);
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

            // Skip the GPU for GBS music
            if (!this.Rom.IsGbs)
            {
                this.Gpu.Tick();
            }

            this.Apu.Tick();

            this.Cpu.TimerTick();

            if (this.Rom.IsGbs)
            {
                this.Rom.Gbs.Tick();
            }

            // Apparently is checked right after instruction execution? But that would add a one-cycle delay to all interrupts?
            this.Cpu.InterruptTick();
        }

        internal bool MessageCallback(string message, bool removePrevious = false) => this.messageCallback(message, removePrevious);
    }
}