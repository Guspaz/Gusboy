using System;
using System.Collections.Generic;

namespace GusBoy
{
    public class APU
    {
        public byte NR10
        {
            get
            {
                int sweep = (this.Channel1.InitialSweepTimer << 4) & 0b0111_0000;
                int negate = (Convert.ToInt32(this.Channel1.SweepNegate) << 3) & 0b0000_1000;
                int shift = this.Channel1.SweepShift & 0b0000_0111;

                return (byte)(sweep | negate | shift | 0b1000_0000);
            }
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel1.InitialSweepTimer = (value & 0b0111_0000) >> 4;
                this.Channel1.SweepShift = value & 0b0000_0111;

                bool oldNegate = this.Channel1.SweepNegate;
                this.Channel1.SweepNegate = Convert.ToBoolean((value & 0b0000_1000) >> 3);

                // Switching from negative to positive after we've calculated using negative disables the channel
                if ( this.Channel1.NegateDirty && oldNegate && !this.Channel1.SweepNegate )
                {
                    this.Channel1.LengthStatus = false;
                }
            }
        }

        public byte NR11
        {
            get
            {
                int duty = (this.Channel1.Duty << 6) & 0b1100_0000;

                return (byte)(duty | 0b0011_1111);
            }
            set
            {
                this.Channel1.LengthLoad = value & 0b0011_1111;

                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel1.Duty = (value & 0b1100_0000) >> 6;
            }
        }

        public byte NR12
        {
            get
            {
                int volume = (this.Channel1.InitialVolume << 4) & 0b1111_0000;
                int envelope = (Convert.ToInt32(this.Channel1.EnvelopeAddMode) << 3) & 0b0000_1000;
                int period = this.Channel1.InitialVolumeTimer & 0b0000_0111;

                return (byte)(volume | envelope | period);
            }
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel1.InitialVolume = (value & 0b1111_0000) >> 4;
                this.Channel1.EnvelopeAddMode = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                this.Channel1.InitialVolumeTimer = value & 0b0000_0111;

                if ( (value & 0b1111_1000) == 0 )
                {
                    // Disable DAC
                    this.Channel1.LengthStatus = false;
                    this.Channel1.DacEnable = false;
                }
                else
                {
                    this.Channel1.DacEnable = true;
                }
            }
        }

        public byte NR13
        {
            get => 0b1111_1111;
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel1.Frequency = (this.Channel1.Frequency & 0b111_0000_0000) | value;
            }
        }

        public byte NR14
        {
            get
            {
                int enable = (Convert.ToInt32(this.Channel1.LengthEnable) << 6) & 0b0100_0000;

                return (byte)(enable | 0b1011_1111);
            }
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel1.LengthEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);
                this.Channel1.Frequency = (this.Channel1.Frequency & 0b000_1111_1111) | ((value & 0b0000_0111) << 8);

                if ( (value & 0b1000_0000) != 0 )
                {
                    this.Channel1.Trigger();
                }
            }
        }

        public byte NR21
        {
            get
            {
                int duty = (this.Channel2.Duty << 6) & 0b1100_0000;

                return (byte)(duty | 0b0011_1111);
            }
            set
            {
                this.Channel2.LengthLoad = value & 0b0011_1111;

                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel2.Duty = (value & 0b1100_0000) >> 6;
            }
        }

        public byte NR22
        {
            get
            {
                int volume = (this.Channel2.InitialVolume << 4) & 0b1111_0000;
                int envelope = (Convert.ToInt32(this.Channel2.EnvelopeAddMode) << 3) & 0b0000_1000;
                int period = this.Channel2.InitialVolumeTimer & 0b0000_0111;

                return (byte)(volume | envelope | period);
            }
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel2.InitialVolume = (value & 0b1111_0000) >> 4;
                this.Channel2.EnvelopeAddMode = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                this.Channel2.InitialVolumeTimer = value & 0b0000_0111;

                if ( (value & 0b1111_1000) == 0 )
                {
                    // Disable DAC
                    this.Channel2.LengthStatus = false;
                    this.Channel2.DacEnable = false;
                }
                else
                {
                    this.Channel2.DacEnable = true;
                }
            }
        }

        public byte NR23
        {
            get => 0b1111_1111;
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel2.Frequency = (this.Channel2.Frequency & 0b111_0000_0000) | value;
            }
        }

        public byte NR24
        {
            get
            {
                int enable = (Convert.ToInt32(this.Channel2.LengthEnable) << 6) & 0b0100_0000;

                return (byte)(enable | 0b1011_1111);
            }
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel2.LengthEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);
                this.Channel2.Frequency = (this.Channel2.Frequency & 0b000_1111_1111) | ((value & 0b0000_0111) << 8);

                if ( (value & 0b1000_0000) != 0 )
                {
                    this.Channel2.Trigger();
                }
            }
        }

        public byte NR30
        {
            get
            {
                int power = (Convert.ToInt32(this.Channel3.DacEnable) << 7) & 0b1000_0000;

                return (byte)(power | 0b0111_1111);
            }
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel3.DacEnable = Convert.ToBoolean((value & 0b1000_0000) >> 7);

                if ( !this.Channel3.DacEnable )
                {
                    this.Channel3.LengthStatus = false;
                }
            }
        }

        public byte NR31
        {
            get => 0b1111_1111;
            set => this.Channel3.LengthLoad = value;
        }

        public byte NR32
        {
            get => (byte)(((this.Channel3.Volume << 5) & 0b0110_0000) | 0b1001_1111);
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel3.Volume = (value & 0b0110_0000) >> 5;
            }
        }

        public byte NR33
        {
            get => 0b1111_1111;
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel3.Frequency = (this.Channel3.Frequency & 0b111_0000_0000) | value;
            }
        }

        public byte NR34
        {
            get
            {
                int enable = (Convert.ToInt32(this.Channel3.LengthEnable) << 6) & 0b0100_0000;

                return (byte)(enable | 0b1011_1111);
            }
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel3.LengthEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);
                this.Channel3.Frequency = (this.Channel3.Frequency & 0b000_1111_1111) | ((value & 0b0000_0111) << 8);

                if ( (value & 0b1000_0000) != 0 )
                {
                    this.Channel3.Trigger();
                }
            }
        }

        public byte NR41
        {
            get => 0b1111_1111;
            set => this.Channel4.LengthLoad = value & 0b0011_1111;
        }

        public byte NR42
        {
            get
            {
                int volume = (this.Channel4.InitialVolume << 4) & 0b1111_0000;
                int envelope = (Convert.ToInt32(this.Channel4.EnvelopeAddMode) << 3) & 0b0000_1000;
                int period = this.Channel4.InitialVolumeTimer & 0b0000_0111;

                return (byte)(volume | envelope | period);
            }
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel4.InitialVolume = (value & 0b1111_0000) >> 4;
                this.Channel4.EnvelopeAddMode = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                this.Channel4.InitialVolumeTimer = value & 0b0000_0111;

                if ( (value & 0b1111_1000) == 0 )
                {
                    // Disable DAC
                    this.Channel4.LengthStatus = false;
                    this.Channel4.DacEnable = false;
                }
                else
                {
                    this.Channel4.DacEnable = true;
                }
            }
        }

        public byte NR43
        {
            get
            {
                int shift = (this.Channel4.ClockShift << 4) & 0b1111_0000;
                int width = (Convert.ToInt32(this.Channel4.WidthMode) << 3) & 0b0000_1000;
                int divisor = this.Channel4.DivisorCode & 0b0000_0111;

                return (byte)(shift | width | divisor);
            }
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel4.ClockShift = (value & 0b1111_0000) >> 4;
                this.Channel4.WidthMode = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                this.Channel4.DivisorCode = value & 0b0000_0111;
            }
        }

        public byte NR44
        {
            get
            {
                int enable = (Convert.ToInt32(this.Channel4.LengthEnable) << 6) & 0b0100_0000;

                return (byte)(enable | 0b1011_1111);
            }
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel4.LengthEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);

                if ( (value & 0b1000_0000) != 0 )
                {
                    this.Channel4.Trigger();
                }
            }
        }

        public byte NR50
        {
            get
            {
                int vlenable = (Convert.ToInt32(this.NR50_VinLEnable) << 7) & 0b1000_0000;
                int lvol = (this.NR50_LeftVol << 4) & 0b0111_0000;
                int vrenable = (Convert.ToInt32(this.NR50_VinREnable) << 3) & 0b0000_1000;
                int rvol = this.NR50_RightVol & 0b0000_0111;

                return (byte)(vlenable | lvol | vrenable | rvol);
            }

            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.NR50_VinLEnable = Convert.ToBoolean((value & 0b1000_0000) >> 7);
                this.NR50_LeftVol = (value & 0b0111_0000) >> 4;
                this.NR50_VinREnable = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                this.NR50_RightVol = value & 0b0000_0111;
            }
        }

        public byte NR51
        {
            get
            {
                int l1 = (Convert.ToInt32(this.Channel4.LeftEnable) << 7) & 0b1000_0000;
                int l2 = (Convert.ToInt32(this.Channel3.LeftEnable) << 6) & 0b0100_0000;
                int l3 = (Convert.ToInt32(this.Channel2.LeftEnable) << 5) & 0b0010_0000;
                int l4 = (Convert.ToInt32(this.Channel1.LeftEnable) << 4) & 0b0001_0000;
                int r1 = (Convert.ToInt32(this.Channel4.RightEnable) << 3) & 0b0000_1000;
                int r2 = (Convert.ToInt32(this.Channel3.RightEnable) << 2) & 0b0000_0100;
                int r3 = (Convert.ToInt32(this.Channel2.RightEnable) << 1) & 0b0000_0010;
                int r4 = (Convert.ToInt32(this.Channel1.RightEnable) << 0) & 0b0000_0001;

                return (byte)(l1 | l2 | l3 | l4 | r1 | r2 | r3 | r4);
            }
            set
            {
                if ( !this.NR52_Power )
                {
                    return;
                }

                this.Channel4.LeftEnable = Convert.ToBoolean((value & 0b1000_0000) >> 7);
                this.Channel3.LeftEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);
                this.Channel2.LeftEnable = Convert.ToBoolean((value & 0b0010_0000) >> 5);
                this.Channel1.LeftEnable = Convert.ToBoolean((value & 0b0001_0000) >> 4);
                this.Channel4.RightEnable = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                this.Channel3.RightEnable = Convert.ToBoolean((value & 0b0000_0100) >> 2);
                this.Channel2.RightEnable = Convert.ToBoolean((value & 0b0000_0010) >> 1);
                this.Channel1.RightEnable = Convert.ToBoolean((value & 0b0000_0001) >> 0);
            }
        }

        public byte NR52
        {
            get
            {
                int power = (Convert.ToInt32(this.NR52_Power) << 7) & 0b1000_0000;
                int l4 = (Convert.ToInt32(this.Channel4.LengthStatus) << 3) & 0b0000_1000;
                int l3 = (Convert.ToInt32(this.Channel3.LengthStatus) << 2) & 0b0000_0100;
                int l2 = (Convert.ToInt32(this.Channel2.LengthStatus) << 1) & 0b0000_0010;
                int l1 = (Convert.ToInt32(this.Channel1.LengthStatus) << 0) & 0b0000_0001;

                return (byte)(power | l4 | l3 | l2 | l1 | 0b0111_0000);
            }

            set
            {
                bool initialPower = this.NR52_Power;
                bool newPower = Convert.ToBoolean((value & 0b1000_0000) << 7);

                if ( !initialPower && newPower )
                {
                    // Power off => on
                    this.frameSequencerStep = 0;
                    this.Channel1.DutyStep = 0;
                    this.Channel2.DutyStep = 0;
                    this.Channel3.sampleBuffer = 0;
                }
                else if ( initialPower && !newPower )
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

                    this.Channel1.LengthStatus = false;
                    this.Channel2.LengthStatus = false;
                    this.Channel3.LengthStatus = false;
                    this.Channel4.LengthStatus = false;
                }

                this.NR52_Power = newPower;
            }
        }

        public SquareChannel Channel1 = new SquareChannel();
        public SquareChannel Channel2 = new SquareChannel();
        public WaveChannel Channel3 = new WaveChannel();
        public NoiseChannel Channel4 = new NoiseChannel();

        public byte[] WaveTable => this.Channel3.WaveTable;

        private bool NR50_VinLEnable;
        private int NR50_LeftVol;
        private bool NR50_VinREnable;
        private int NR50_RightVol;
        private bool NR52_Power;

        public List<float> buffer = new List<float>(8000);

        private readonly Gameboy gb;
        private long oldCpuTicks;
        private long timer;
        private long frameSequencerTimer;
        private long frameSequencerStep;

        private readonly double[] capacitor = new double[2];
        private readonly double capacitorFactor;
        private readonly int sampleClock;

        private const int CPU_CLOCK = 4194304;
        private const double CAPACITOR_BASE = 0.999958;

        public APU(Gameboy gameBoy, int sampleRate)
        {
            this.gb = gameBoy;
            this.capacitorFactor = Math.Pow(CAPACITOR_BASE, CPU_CLOCK / sampleRate); // use 0.998943 for MGB&CGB or 0.999958 for DMG
            this.sampleClock = (int)Math.Round(CPU_CLOCK / (decimal)sampleRate);
        }

        public void Tick()
        {
            long apuTicks = this.gb.cpu.ticks - this.oldCpuTicks;
            this.oldCpuTicks = this.gb.cpu.ticks;

            for ( int i = 0; i < apuTicks; i++ )
            {
                if ( this.frameSequencerTimer > 0 )
                {
                    this.frameSequencerTimer--;
                }

                if ( this.frameSequencerTimer == 0 )
                {
                    if ( this.NR52_Power )
                    {
                        switch ( this.frameSequencerStep )
                        {
                            case 0:
                                this.Channel1.LengthTimerTick();
                                this.Channel2.LengthTimerTick();
                                this.Channel3.LengthTimerTick();
                                this.Channel4.LengthTimerTick();
                                break;
                            case 2:
                                this.Channel1.SweepTick();
                                this.Channel1.LengthTimerTick();
                                this.Channel2.LengthTimerTick();
                                this.Channel3.LengthTimerTick();
                                this.Channel4.LengthTimerTick();
                                break;
                            case 4:
                                this.Channel1.LengthTimerTick();
                                this.Channel2.LengthTimerTick();
                                this.Channel3.LengthTimerTick();
                                this.Channel4.LengthTimerTick();
                                break;
                            case 6:
                                this.Channel1.SweepTick();
                                this.Channel1.LengthTimerTick();
                                this.Channel2.LengthTimerTick();
                                this.Channel3.LengthTimerTick();
                                this.Channel4.LengthTimerTick();
                                break;
                            case 7:
                                this.Channel1.VolumeEnvelopeTick();
                                this.Channel2.VolumeEnvelopeTick();
                                this.Channel4.VolumeEnvelopeTick();
                                break;
                        }

                        if ( ++this.frameSequencerStep > 7 )
                        {
                            this.frameSequencerStep = 0;
                        }
                    }

                    this.frameSequencerTimer = 8192;
                }

                if ( this.NR52_Power )
                {
                    this.Channel1.ClockTick();
                    this.Channel2.ClockTick();
                    this.Channel3.ClockTick();
                    this.Channel4.ClockTick();
                }

                if ( this.timer == 0 )
                {
                    if ( this.NR52_Power )
                    {
                        this.buffer.Add(this.HighPass((this.Channel1.OutputLeft + this.Channel2.OutputLeft + this.Channel3.OutputLeft + this.Channel4.OutputLeft) / 4 * (this.NR50_LeftVol + 1), 1));
                        this.buffer.Add(this.HighPass((this.Channel1.OutputRight + this.Channel2.OutputRight + this.Channel3.OutputRight + this.Channel4.OutputRight) / 4 * (this.NR50_RightVol + 1), 2));
                    }
                    else
                    {
                        this.buffer.Add(0);
                        this.buffer.Add(0);
                    }

                    this.timer = this.sampleClock;
                }
                else
                {
                    this.timer--;
                }
            }
        }

        private float HighPass(float input, int channel, bool dacs_enabled = true)
        {
            // TODO: Implement this flag
            if ( dacs_enabled )
            {
                double output = input - this.capacitor[channel - 1];

                // capacitor slowly charges to 'in' via their difference
                this.capacitor[channel - 1] = input - output * this.capacitorFactor;

                return (float)output;
            }

            return 0;
        }
    }
}
