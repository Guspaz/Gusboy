﻿namespace Gusboy
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Largely implemented from https://gbdev.gg8.se/wiki/articles/Gameboy_sound_hardware .
    /// </summary>
    public partial class APU
    {
        private const int CPU_CLOCK = 4194304 / 4; // Convert to m-cycles
        private const int SUPERSAMPLE_MODULO = 2;
        private const double CAPACITOR_BASE_DMG = 0.999958;
        private const double CAPACITOR_BASE_CGB = 0.999943; // gbdev.gg8.se says it should be 0.998943 but that sounds very tinny

        private readonly Gameboy gb;
        private readonly SquareChannel channel1 = new SquareChannel();
        private readonly SquareChannel channel2 = new SquareChannel();
        private readonly WaveChannel channel3 = new WaveChannel();
        private readonly NoiseChannel channel4 = new NoiseChannel();
        private readonly double[] capacitor = new double[2];
        private readonly int sampleClock;

        private bool vInLeftEnable;
        private bool vInRightEnable;
        private int leftMasterVolume;
        private int rightMasterVolume;
        private bool apuPower;
        private double capacitorFactor;

        private long oldCpuTicks;
        private int pendingApuTicks;
        private int timer = 1; // Can be set to 0 to run at uncapped framerate, but this has major side effects, only use for benchmarking
        private int frameSequencerStep;

        private int superSampleTimer = SUPERSAMPLE_MODULO;
        private int superSampleCount;
        private double accumulationBufferLeft;
        private double accumulationBufferRight;

        private int divFallingEdge;

        public APU(Gameboy gameBoy, int sampleRate)
        {
            this.gb = gameBoy;
            this.sampleClock = (int)Math.Round(CPU_CLOCK / (double)sampleRate);
        }

        public List<float> Buffer { get; } = new List<float>(8000);

        public byte[] WaveTable => this.channel3.WaveTable;

        public bool WaveEnabled => this.channel3.ChannelEnable;

        public void Initialize(bool powerOff = false)
        {
            // I think this math is right?
            this.capacitorFactor = Math.Pow(this.gb.IsCgb ? CAPACITOR_BASE_CGB : CAPACITOR_BASE_DMG, SUPERSAMPLE_MODULO * 4);

            if (!powerOff || this.gb.IsCgb)
            {
                this.NR11 = 0;
                this.NR21 = 0;
                this.NR31 = 0;
                this.NR41 = 0;
            }
            else
            {
                this.NR11 &= 0b0011_1111;
                this.NR21 &= 0b0011_1111;
                this.NR41 &= 0b0011_1111;
            }

            this.NR10 = 0;
            this.NR12 = 0;
            this.NR13 = 0;
            this.NR14 = 0;
            this.NR22 = 0;
            this.NR23 = 0;
            this.NR24 = 0;
            this.NR30 = 0;
            this.NR32 = 0;
            this.NR33 = 0;
            this.NR34 = 0;
            this.NR42 = 0;
            this.NR43 = 0;
            this.NR44 = 0;
            this.NR50 = 0;
            this.NR51 = 0;

            this.channel1.ChannelEnable = false;
            this.channel2.ChannelEnable = false;
            this.channel3.ChannelEnable = false;
            this.channel4.ChannelEnable = false;
        }

        public void Tick()
        {
            this.pendingApuTicks += (int)(this.gb.Cpu.Ticks - this.oldCpuTicks);
            this.oldCpuTicks = this.gb.Cpu.Ticks;

            int cycleLength = this.gb.Cpu.fSpeed ? 8 : 4;
            int divMask = 1 << (14 - (int)this.gb.Cpu.fSpeedTimerMultiplier);

            // TODO: Should this use a falling edge, or rising edge?
            // TODO: Using this instead of the old counter approach means we might step the frame sequencer before some of the frequency ticks that should go before it, does that matter?
            if (this.divFallingEdge != 0 && (this.gb.Cpu.rDIV & divMask) == 0)
            {
                if (this.apuPower)
                {
                    switch (this.frameSequencerStep)
                    {
                        case 0:
                            this.channel1.LengthTimerTick();
                            this.channel2.LengthTimerTick();
                            this.channel3.LengthTimerTick();
                            this.channel4.LengthTimerTick();
                            break;
                        case 2:
                            this.channel1.SweepTick();
                            this.channel1.LengthTimerTick();
                            this.channel2.LengthTimerTick();
                            this.channel3.LengthTimerTick();
                            this.channel4.LengthTimerTick();
                            break;
                        case 4:
                            this.channel1.LengthTimerTick();
                            this.channel2.LengthTimerTick();
                            this.channel3.LengthTimerTick();
                            this.channel4.LengthTimerTick();
                            break;
                        case 6:
                            this.channel1.SweepTick();
                            this.channel1.LengthTimerTick();
                            this.channel2.LengthTimerTick();
                            this.channel3.LengthTimerTick();
                            this.channel4.LengthTimerTick();
                            break;
                        case 7:
                            this.channel1.VolumeEnvelopeTick();
                            this.channel2.VolumeEnvelopeTick();
                            this.channel4.VolumeEnvelopeTick();
                            break;
                    }

                    if (++this.frameSequencerStep > 7)
                    {
                        this.frameSequencerStep = 0;
                    }
                }
            }

            this.divFallingEdge = this.gb.Cpu.rDIV & divMask;

            if (this.pendingApuTicks >= cycleLength)
            {
                while (this.pendingApuTicks > 0)
                {
                    this.pendingApuTicks -= cycleLength;

                    if (this.apuPower)
                    {
                        this.channel1.ClockTick();
                        this.channel2.ClockTick();
                        this.channel3.ClockTick();
                        this.channel3.ClockTick(); // Tick channel 3 twice because it runs at double speed
                        this.channel4.ClockTick();
                    }

                    if (--this.superSampleTimer == 0)
                    {
                        this.superSampleCount++;

                        if (this.apuPower && (this.channel1.DacEnable || this.channel2.DacEnable || this.channel3.DacEnable || this.channel4.DacEnable))
                        {
                            this.accumulationBufferLeft += this.HighPass((this.channel1.OutputLeft + this.channel2.OutputLeft + this.channel3.OutputLeft + this.channel4.OutputLeft) / 4 * (this.leftMasterVolume + 1), 1);
                            this.accumulationBufferRight += this.HighPass((this.channel1.OutputRight + this.channel2.OutputRight + this.channel3.OutputRight + this.channel4.OutputRight) / 4 * (this.rightMasterVolume + 1), 2);
                        }
                        else
                        {
                            this.accumulationBufferLeft += this.HighPass(7.5 / 100, 1);
                            this.accumulationBufferRight += this.HighPass(7.5 / 100, 1);
                        }

                        this.superSampleTimer = SUPERSAMPLE_MODULO;
                    }

                    if (--this.timer == 0)
                    {
                        if (this.superSampleCount != 0)
                        {
                            this.Buffer.Add((float)(this.accumulationBufferLeft / this.superSampleCount));
                            this.Buffer.Add((float)(this.accumulationBufferRight / this.superSampleCount));
                        }
                        else
                        {
                            // TODO: Something else here?
                            this.Buffer.Add(0);
                            this.Buffer.Add(0);
                        }

                        this.accumulationBufferLeft = 0;
                        this.accumulationBufferRight = 0;
                        this.superSampleCount = 0;

                        this.timer = this.sampleClock;
                    }
                }
            }
        }

        internal void FakeBootstrap()
        {
            // NOTE: As we're replicating the internal state of the APU after the bootrom, these values may need to be updated in the future
            // TODO: This is for DMG, will need to supplement for CGB, the timers will probably be different.
            this.channel1.ChannelEnable = true;
            this.channel1.DacEnable = true;
            this.channel1.Duty = 2;
            this.channel1.DutyStep = 3;
            this.channel1.Frequency = 1985;
            this.channel1.InitialVolume = 15;
            this.channel1.InitialVolumeTimer = 3;
            this.channel1.LeftEnable = true;
            this.channel1.RightEnable = true;
            this.channel1.FakeBootstrap(4, 1985, 5, 1);

            this.channel2.DutyStep = 2;
            this.channel2.LeftEnable = true;
            this.channel2.RightEnable = true;
            this.channel2.FakeBootstrap(1309);

            this.channel3.LeftEnable = true;
            this.channel3.FakeBootstrap(569, 3);

            this.channel4.LeftEnable = true;
            this.channel4.FakeBootstrap(1);

            this.leftMasterVolume = 7;
            this.rightMasterVolume = 7;
            this.apuPower = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double HighPass(double input, int channel)
        {
            double output = input - this.capacitor[channel - 1];

            // capacitor slowly charges to 'in' via their difference
            this.capacitor[channel - 1] = input - (output * this.capacitorFactor);

            return output;
        }
    }
}
