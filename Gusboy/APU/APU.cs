namespace Gusboy
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Largely implemented from https://gbdev.gg8.se/wiki/articles/Gameboy_sound_hardware .
    /// </summary>
    public class APU
    {
        private const int CPU_CLOCK = 4194304;
        private const double CAPACITOR_BASE_DMG = 0.999958;
        private const double CAPACITOR_BASE_CGB = 0.999958; // Should be 0.998943 but that sounds very tinny

        private readonly Gameboy gb;
        private readonly SquareChannel channel1 = new SquareChannel();
        private readonly SquareChannel channel2 = new SquareChannel();
        private readonly WaveChannel channel3 = new WaveChannel();
        private readonly NoiseChannel channel4 = new NoiseChannel();
        private readonly float[] capacitor = new float[2];
        private readonly int sampleClock;
        private readonly int sampleRate;

        private bool vInLeftEnable;
        private bool vInRightEnable;
        private int leftMasterVolume;
        private int rightMasterVolume;
        private bool apuPower;
        private float capacitorFactor;

        private long oldCpuTicks;
        private long timer;
        private long frameSequencerTimer;
        private long frameSequencerStep;

        public APU(Gameboy gameBoy, int sampleRate)
        {
            this.gb = gameBoy;
            this.sampleClock = (int)Math.Round(CPU_CLOCK / (double)sampleRate);
            this.sampleRate = sampleRate;
        }

        public List<float> Buffer { get; } = new List<float>(8000);

        public byte NR10
        {
            get
            {
                int sweep = (this.channel1.InitialSweepTimer << 4) & 0b0111_0000;
                int negate = (Convert.ToInt32(this.channel1.SweepNegate) << 3) & 0b0000_1000;
                int shift = this.channel1.SweepShift & 0b0000_0111;

                return (byte)(sweep | negate | shift | 0b1000_0000);
            }

            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel1.InitialSweepTimer = (value & 0b0111_0000) >> 4;
                this.channel1.SweepShift = value & 0b0000_0111;

                bool oldNegate = this.channel1.SweepNegate;
                this.channel1.SweepNegate = Convert.ToBoolean((value & 0b0000_1000) >> 3);

                // Switching from negative to positive after we've calculated using negative disables the channel
                if (this.channel1.NegateDirty && oldNegate && !this.channel1.SweepNegate)
                {
                    this.channel1.ChannelEnable = false;
                }
            }
        }

        public byte NR11
        {
            get
            {
                int duty = (this.channel1.Duty << 6) & 0b1100_0000;

                return (byte)(duty | 0b0011_1111);
            }

            set
            {
                this.channel1.LengthLoad = value & 0b0011_1111;

                if (!this.apuPower)
                {
                    return;
                }

                this.channel1.Duty = (value & 0b1100_0000) >> 6;
            }
        }

        public byte NR12
        {
            get
            {
                int volume = (this.channel1.InitialVolume << 4) & 0b1111_0000;
                int envelope = (Convert.ToInt32(this.channel1.EnvelopeAddMode) << 3) & 0b0000_1000;
                int period = this.channel1.InitialVolumeTimer & 0b0000_0111;

                return (byte)(volume | envelope | period);
            }

            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel1.InitialVolume = (value & 0b1111_0000) >> 4;
                this.channel1.EnvelopeAddMode = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                this.channel1.InitialVolumeTimer = value & 0b0000_0111;

                if ((value & 0b1111_1000) == 0)
                {
                    // Disable DAC
                    this.channel1.ChannelEnable = false;
                    this.channel1.DacEnable = false;
                }
                else
                {
                    this.channel1.DacEnable = true;
                }
            }
        }

        public byte NR13
        {
            get => 0b1111_1111;
            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel1.Frequency = (this.channel1.Frequency & 0b111_0000_0000) | value;
            }
        }

        public byte NR14
        {
            get
            {
                int enable = (Convert.ToInt32(this.channel1.LengthEnable) << 6) & 0b0100_0000;

                return (byte)(enable | 0b1011_1111);
            }

            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel1.LengthEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);
                this.channel1.Frequency = (this.channel1.Frequency & 0b000_1111_1111) | ((value & 0b0000_0111) << 8);

                if ((value & 0b1000_0000) != 0)
                {
                    this.channel1.Trigger();
                }
            }
        }

        public byte NR21
        {
            get
            {
                int duty = (this.channel2.Duty << 6) & 0b1100_0000;

                return (byte)(duty | 0b0011_1111);
            }

            set
            {
                this.channel2.LengthLoad = value & 0b0011_1111;

                if (!this.apuPower)
                {
                    return;
                }

                this.channel2.Duty = (value & 0b1100_0000) >> 6;
            }
        }

        public byte NR22
        {
            get
            {
                int volume = (this.channel2.InitialVolume << 4) & 0b1111_0000;
                int envelope = (Convert.ToInt32(this.channel2.EnvelopeAddMode) << 3) & 0b0000_1000;
                int period = this.channel2.InitialVolumeTimer & 0b0000_0111;

                return (byte)(volume | envelope | period);
            }

            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel2.InitialVolume = (value & 0b1111_0000) >> 4;
                this.channel2.EnvelopeAddMode = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                this.channel2.InitialVolumeTimer = value & 0b0000_0111;

                if ((value & 0b1111_1000) == 0)
                {
                    // Disable DAC
                    this.channel2.ChannelEnable = false;
                    this.channel2.DacEnable = false;
                }
                else
                {
                    this.channel2.DacEnable = true;
                }
            }
        }

        public byte NR23
        {
            get => 0b1111_1111;
            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel2.Frequency = (this.channel2.Frequency & 0b111_0000_0000) | value;
            }
        }

        public byte NR24
        {
            get
            {
                int enable = (Convert.ToInt32(this.channel2.LengthEnable) << 6) & 0b0100_0000;

                return (byte)(enable | 0b1011_1111);
            }

            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel2.LengthEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);
                this.channel2.Frequency = (this.channel2.Frequency & 0b000_1111_1111) | ((value & 0b0000_0111) << 8);

                if ((value & 0b1000_0000) != 0)
                {
                    this.channel2.Trigger();
                }
            }
        }

        public byte NR30
        {
            get
            {
                int power = (Convert.ToInt32(this.channel3.DacEnable) << 7) & 0b1000_0000;

                return (byte)(power | 0b0111_1111);
            }

            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel3.DacEnable = Convert.ToBoolean((value & 0b1000_0000) >> 7);

                if (!this.channel3.DacEnable)
                {
                    this.channel3.ChannelEnable = false;
                }
            }
        }

        public byte NR31
        {
            get => 0b1111_1111;
            set => this.channel3.LengthLoad = value;
        }

        public byte NR32
        {
            get => (byte)(((this.channel3.Volume << 5) & 0b0110_0000) | 0b1001_1111);
            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel3.Volume = (value & 0b0110_0000) >> 5;
            }
        }

        public byte NR33
        {
            get => 0b1111_1111;
            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel3.Frequency = (this.channel3.Frequency & 0b111_0000_0000) | value;
            }
        }

        public byte NR34
        {
            get
            {
                int enable = (Convert.ToInt32(this.channel3.LengthEnable) << 6) & 0b0100_0000;

                return (byte)(enable | 0b1011_1111);
            }

            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel3.LengthEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);
                this.channel3.Frequency = (this.channel3.Frequency & 0b000_1111_1111) | ((value & 0b0000_0111) << 8);

                if ((value & 0b1000_0000) != 0)
                {
                    this.channel3.Trigger();
                }
            }
        }

        public byte NR41
        {
            get => 0b1111_1111;
            set => this.channel4.LengthLoad = value & 0b0011_1111;
        }

        public byte NR42
        {
            get
            {
                int volume = (this.channel4.InitialVolume << 4) & 0b1111_0000;
                int envelope = (Convert.ToInt32(this.channel4.EnvelopeAddMode) << 3) & 0b0000_1000;
                int period = this.channel4.InitialVolumeTimer & 0b0000_0111;

                return (byte)(volume | envelope | period);
            }

            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel4.InitialVolume = (value & 0b1111_0000) >> 4;
                this.channel4.EnvelopeAddMode = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                this.channel4.InitialVolumeTimer = value & 0b0000_0111;

                if ((value & 0b1111_1000) == 0)
                {
                    // Disable DAC
                    this.channel4.ChannelEnable = false;
                    this.channel4.DacEnable = false;
                }
                else
                {
                    this.channel4.DacEnable = true;
                }
            }
        }

        public byte NR43
        {
            get
            {
                int shift = (this.channel4.ClockShift << 4) & 0b1111_0000;
                int width = (Convert.ToInt32(this.channel4.WidthMode) << 3) & 0b0000_1000;
                int divisor = this.channel4.DivisorCode & 0b0000_0111;

                return (byte)(shift | width | divisor);
            }

            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel4.ClockShift = (value & 0b1111_0000) >> 4;
                this.channel4.WidthMode = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                this.channel4.DivisorCode = value & 0b0000_0111;
            }
        }

        public byte NR44
        {
            get
            {
                int enable = (Convert.ToInt32(this.channel4.LengthEnable) << 6) & 0b0100_0000;

                return (byte)(enable | 0b1011_1111);
            }

            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel4.LengthEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);

                if ((value & 0b1000_0000) != 0)
                {
                    this.channel4.Trigger();
                }
            }
        }

        public byte NR50
        {
            get
            {
                int vlenable = (Convert.ToInt32(this.vInLeftEnable) << 7) & 0b1000_0000;
                int lvol = (this.leftMasterVolume << 4) & 0b0111_0000;
                int vrenable = (Convert.ToInt32(this.vInRightEnable) << 3) & 0b0000_1000;
                int rvol = this.rightMasterVolume & 0b0000_0111;

                return (byte)(vlenable | lvol | vrenable | rvol);
            }

            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.vInLeftEnable = Convert.ToBoolean((value & 0b1000_0000) >> 7);
                this.leftMasterVolume = (value & 0b0111_0000) >> 4;
                this.vInRightEnable = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                this.rightMasterVolume = value & 0b0000_0111;
            }
        }

        public byte NR51
        {
            get
            {
                int l1 = (Convert.ToInt32(this.channel4.LeftEnable) << 7) & 0b1000_0000;
                int l2 = (Convert.ToInt32(this.channel3.LeftEnable) << 6) & 0b0100_0000;
                int l3 = (Convert.ToInt32(this.channel2.LeftEnable) << 5) & 0b0010_0000;
                int l4 = (Convert.ToInt32(this.channel1.LeftEnable) << 4) & 0b0001_0000;
                int r1 = (Convert.ToInt32(this.channel4.RightEnable) << 3) & 0b0000_1000;
                int r2 = (Convert.ToInt32(this.channel3.RightEnable) << 2) & 0b0000_0100;
                int r3 = (Convert.ToInt32(this.channel2.RightEnable) << 1) & 0b0000_0010;
                int r4 = (Convert.ToInt32(this.channel1.RightEnable) << 0) & 0b0000_0001;

                return (byte)(l1 | l2 | l3 | l4 | r1 | r2 | r3 | r4);
            }

            set
            {
                if (!this.apuPower)
                {
                    return;
                }

                this.channel4.LeftEnable = Convert.ToBoolean((value & 0b1000_0000) >> 7);
                this.channel3.LeftEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);
                this.channel2.LeftEnable = Convert.ToBoolean((value & 0b0010_0000) >> 5);
                this.channel1.LeftEnable = Convert.ToBoolean((value & 0b0001_0000) >> 4);
                this.channel4.RightEnable = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                this.channel3.RightEnable = Convert.ToBoolean((value & 0b0000_0100) >> 2);
                this.channel2.RightEnable = Convert.ToBoolean((value & 0b0000_0010) >> 1);
                this.channel1.RightEnable = Convert.ToBoolean((value & 0b0000_0001) >> 0);
            }
        }

        public byte NR52
        {
            get
            {
                int power = (Convert.ToInt32(this.apuPower) << 7) & 0b1000_0000;
                int l4 = (Convert.ToInt32(this.channel4.ChannelEnable) << 3) & 0b0000_1000;
                int l3 = (Convert.ToInt32(this.channel3.ChannelEnable) << 2) & 0b0000_0100;
                int l2 = (Convert.ToInt32(this.channel2.ChannelEnable) << 1) & 0b0000_0010;
                int l1 = (Convert.ToInt32(this.channel1.ChannelEnable) << 0) & 0b0000_0001;

                return (byte)(power | l4 | l3 | l2 | l1 | 0b0111_0000);
            }

            set
            {
                bool initialPower = this.apuPower;
                bool newPower = Convert.ToBoolean((value & 0b1000_0000) << 7);

                if (!initialPower && newPower)
                {
                    // Power off => on
                    this.frameSequencerStep = 0;
                    this.channel1.DutyStep = 0;
                    this.channel2.DutyStep = 0;
                    this.channel3.SampleBuffer = 0;
                }
                else if (initialPower && !newPower)
                {
                    // TODO: DMG preserves length counters on poweroff

                    // Power on => off
                    this.NR10 = 0;
                    this.NR11 = 0;
                    this.NR12 = 0;
                    this.NR13 = 0;
                    this.NR14 = 0;
                    this.NR21 = 0;
                    this.NR22 = 0;
                    this.NR23 = 0;
                    this.NR24 = 0;
                    this.NR30 = 0;
                    this.NR31 = 0;
                    this.NR32 = 0;
                    this.NR33 = 0;
                    this.NR34 = 0;
                    this.NR41 = 0;
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

                this.apuPower = newPower;
            }
        }

        public byte[] WaveTable => this.channel3.WaveTable;

        public bool WaveEnabled => this.channel3.ChannelEnable;

        public void Initialize()
        {
            this.capacitorFactor = (float)Math.Pow(this.gb.IsCgb ? CAPACITOR_BASE_CGB : CAPACITOR_BASE_DMG, CPU_CLOCK / (double)this.sampleRate);
        }

        public void Tick()
        {
            long apuTicks = this.gb.Cpu.Ticks - this.oldCpuTicks;
            this.oldCpuTicks = this.gb.Cpu.Ticks;

            if (this.gb.Cpu.fSpeed)
            {
                // CGB runs the GPU/APU at half-speed
                apuTicks >>= 1;
            }

            for (int i = 0; i < apuTicks; i++)
            {
                if (this.frameSequencerTimer > 0)
                {
                    this.frameSequencerTimer--;
                }

                if (this.frameSequencerTimer == 0)
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

                    this.frameSequencerTimer = 8192;
                }

                if (this.apuPower)
                {
                    this.channel1.ClockTick();
                    this.channel2.ClockTick();
                    this.channel3.ClockTick();
                    this.channel4.ClockTick();
                }

                if (this.timer == 0)
                {
                    if (this.apuPower && (this.channel2.DacEnable || this.channel1.DacEnable || this.channel3.DacEnable || this.channel4.DacEnable))
                    {
                        this.Buffer.Add(this.HighPass((this.channel1.OutputLeft + this.channel2.OutputLeft + this.channel3.OutputLeft + this.channel4.OutputLeft) / 4 * (this.leftMasterVolume + 1), 1));
                        this.Buffer.Add(this.HighPass((this.channel1.OutputRight + this.channel2.OutputRight + this.channel3.OutputRight + this.channel4.OutputRight) / 4 * (this.rightMasterVolume + 1), 2));
                    }
                    else
                    {
                        this.Buffer.Add(0);
                        this.Buffer.Add(0);
                    }

                    this.timer = this.sampleClock;
                }
                else
                {
                    this.timer--;
                }
            }
        }

        private float HighPass(float input, int channel)
        {
            float output = input - this.capacitor[channel - 1];

            // capacitor slowly charges to 'in' via their difference
            this.capacitor[channel - 1] = input - (output * this.capacitorFactor);

            return output;
        }
    }
}
