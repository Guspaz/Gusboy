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
                int sweep = (Channel1.InitialSweepTimer << 4) & 0b0111_0000;
                int negate = (Convert.ToInt32(Channel1.SweepNegate) << 3) & 0b0000_1000;
                int shift = Channel1.SweepShift & 0b0000_0111;

                return (byte)(sweep | negate | shift | 0b1000_0000);
            }
            set
            {
                if (!NR52_Power) return;

                Channel1.InitialSweepTimer = (value & 0b0111_0000) >> 4;
                Channel1.SweepShift = value & 0b0000_0111;

                bool oldNegate = Channel1.SweepNegate;
                Channel1.SweepNegate = Convert.ToBoolean((value & 0b0000_1000) >> 3);

                // Switching from negative to positive after we've calculated using negative disables the channel
                if (Channel1.NegateDirty && oldNegate && !Channel1.SweepNegate)
                {
                    Channel1.LengthStatus = false;
                }
            }
        }

        public byte NR11
        {
            get
            {
                int duty = (Channel1.Duty << 6) & 0b1100_0000;

                return (byte)(duty | 0b0011_1111);
            }
            set
            {
                Channel1.LengthLoad = value & 0b0011_1111;

                if (!NR52_Power) return;

                Channel1.Duty = (value & 0b1100_0000) >> 6;
            }
        }

        public byte NR12
        {
            get
            {
                int volume = (Channel1.InitialVolume << 4) & 0b1111_0000;
                int envelope = (Convert.ToInt32(Channel1.EnvelopeAddMode) << 3) & 0b0000_1000;
                int period = Channel1.InitialVolumeTimer & 0b0000_0111;

                return (byte)(volume | envelope | period);
            }
            set
            {
                if (!NR52_Power) return;

                Channel1.InitialVolume = (value & 0b1111_0000) >> 4;
                Channel1.EnvelopeAddMode = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                Channel1.InitialVolumeTimer = value & 0b0000_0111;

                if ((value & 0b1111_1000) == 0)
                {
                    // Disable DAC
                    Channel1.LengthStatus = false;
                    Channel1.DacEnable = false;
                }
                else
                {
                    Channel1.DacEnable = true;
                }
            }
        }

        public byte NR13
        {
            get
            {
                return 0b1111_1111;
            }
            set
            {
                if (!NR52_Power) return;

                Channel1.Frequency = (Channel1.Frequency & 0b111_0000_0000) | value;
            }
        }

        public byte NR14
        {
            get
            {
                int enable = (Convert.ToInt32(Channel1.LengthEnable) << 6) & 0b0100_0000;

                return (byte)(enable | 0b1011_1111);
            }
            set
            {
                if (!NR52_Power) return;

                Channel1.LengthEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);
                Channel1.Frequency = (Channel1.Frequency & 0b000_1111_1111) | ((value & 0b0000_0111) << 8);

                if ((value & 0b1000_0000) != 0)
                {
                    Channel1.Trigger();
                }
            }
        }

        public byte NR21
        {
            get
            {
                int duty = (Channel2.Duty << 6) & 0b1100_0000;

                return (byte)(duty | 0b0011_1111);
            }
            set
            {
                Channel2.LengthLoad = value & 0b0011_1111;

                if (!NR52_Power) return;

                Channel2.Duty = (value & 0b1100_0000) >> 6;
            }
        }

        public byte NR22
        {
            get
            {
                int volume = (Channel2.InitialVolume << 4) & 0b1111_0000;
                int envelope = (Convert.ToInt32(Channel2.EnvelopeAddMode) << 3) & 0b0000_1000;
                int period = Channel2.InitialVolumeTimer & 0b0000_0111;

                return (byte)(volume | envelope | period);
            }
            set
            {
                if (!NR52_Power) return;

                Channel2.InitialVolume = (value & 0b1111_0000) >> 4;
                Channel2.EnvelopeAddMode = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                Channel2.InitialVolumeTimer = value & 0b0000_0111;

                if ((value & 0b1111_1000) == 0)
                {
                    // Disable DAC
                    Channel2.LengthStatus = false;
                    Channel2.DacEnable = false;
                }
                else
                {
                    Channel2.DacEnable = true;
                }
            }
        }

        public byte NR23
        {
            get
            {
                return 0b1111_1111;
            }
            set
            {
                if (!NR52_Power) return;

                Channel2.Frequency = (Channel2.Frequency & 0b111_0000_0000) | value;
            }
        }

        public byte NR24
        {
            get
            {
                int enable = (Convert.ToInt32(Channel2.LengthEnable) << 6) & 0b0100_0000;

                return (byte)(enable | 0b1011_1111);
            }
            set
            {
                if (!NR52_Power) return;

                Channel2.LengthEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);
                Channel2.Frequency = (Channel2.Frequency & 0b000_1111_1111) | ((value & 0b0000_0111) << 8);

                if ((value & 0b1000_0000) != 0)
                {
                    Channel2.Trigger();
                }
            }
        }

        public byte NR30
        {
            get
            {
                int power = (Convert.ToInt32(Channel3.DacEnable) << 7) & 0b1000_0000;

                return (byte)(power | 0b0111_1111);
            }
            set
            {
                if (!NR52_Power) return;

                Channel3.DacEnable = Convert.ToBoolean((value & 0b1000_0000) >> 7);

                if (!Channel3.DacEnable)
                {
                    Channel3.LengthStatus = false;
                }
            }
        }

        public byte NR31
        {
            get
            {
                return 0b1111_1111;
            }
            set
            {
                Channel3.LengthLoad = value;
            }
        }

        public byte NR32
        {
            get
            {
                return (byte)(((Channel3.Volume << 5) & 0b0110_0000) | 0b1001_1111);
            }
            set
            {
                if (!NR52_Power) return;

                Channel3.Volume = (value & 0b0110_0000) >> 5;
            }
        }

        public byte NR33
        {
            get
            {
                return 0b1111_1111;
            }
            set
            {
                if (!NR52_Power) return;

                Channel3.Frequency = (Channel3.Frequency & 0b111_0000_0000) | value;
            }
        }

        public byte NR34
        {
            get
            {
                int enable = (Convert.ToInt32(Channel3.LengthEnable) << 6) & 0b0100_0000;

                return (byte)(enable | 0b1011_1111);
            }
            set
            {
                if (!NR52_Power) return;

                Channel3.LengthEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);
                Channel3.Frequency = (Channel3.Frequency & 0b000_1111_1111) | ((value & 0b0000_0111) << 8);

                if ((value & 0b1000_0000) != 0)
                {
                    Channel3.Trigger();
                }
            }
        }

        public byte NR41
        {
            get
            {
                return 0b1111_1111;
            }
            set
            {
                Channel4.LengthLoad = value & 0b0011_1111;
            }
        }

        public byte NR42
        {
            get
            {
                int volume = (Channel4.InitialVolume << 4) & 0b1111_0000;
                int envelope = (Convert.ToInt32(Channel4.EnvelopeAddMode) << 3) & 0b0000_1000;
                int period = Channel4.InitialVolumeTimer & 0b0000_0111;

                return (byte)(volume | envelope | period);
            }
            set
            {
                if (!NR52_Power) return;

                Channel4.InitialVolume = (value & 0b1111_0000) >> 4;
                Channel4.EnvelopeAddMode = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                Channel4.InitialVolumeTimer = value & 0b0000_0111;

                if ((value & 0b1111_1000) == 0)
                {
                    // Disable DAC
                    Channel4.LengthStatus = false;
                    Channel4.DacEnable = false;
                }
                else
                {
                    Channel4.DacEnable = true;
                }
            }
        }

        public byte NR43
        {
            get
            {
                int shift = (Channel4.ClockShift << 4) & 0b1111_0000;
                int width = (Convert.ToInt32(Channel4.WidthMode) << 3) & 0b0000_1000;
                int divisor = Channel4.DivisorCode & 0b0000_0111;

                return (byte)(shift | width | divisor);
            }
            set
            {
                if (!NR52_Power) return;

                Channel4.ClockShift = (value & 0b1111_0000) >> 4;
                Channel4.WidthMode = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                Channel4.DivisorCode = value & 0b0000_0111;
            }
        }

        public byte NR44
        {
            get
            {
                int enable = (Convert.ToInt32(Channel4.LengthEnable) << 6) & 0b0100_0000;

                return (byte)(enable | 0b1011_1111);
            }
            set
            {
                if (!NR52_Power) return;

                Channel4.LengthEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);

                if ((value & 0b1000_0000) != 0)
                {
                    Channel4.Trigger();
                }
            }
        }

        public byte NR50
        {
            get
            {
                int vlenable = (Convert.ToInt32(NR50_VinLEnable) << 7) & 0b1000_0000;
                int lvol = (NR50_LeftVol << 4) & 0b0111_0000;
                int vrenable = (Convert.ToInt32(NR50_VinREnable) << 3) & 0b0000_1000;
                int rvol = NR50_RightVol & 0b0000_0111;

                return (byte)(vlenable | lvol | vrenable | rvol);
            }

            set
            {
                if (!NR52_Power) return;

                NR50_VinLEnable = Convert.ToBoolean((value & 0b1000_0000) >> 7);
                NR50_LeftVol = (value & 0b0111_0000) >> 4;
                NR50_VinREnable = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                NR50_RightVol = value & 0b0000_0111;
            }
        }

        public byte NR51
        {
            get
            {
                int l1 = (Convert.ToInt32(Channel4.LeftEnable) << 7) & 0b1000_0000;
                int l2 = (Convert.ToInt32(Channel3.LeftEnable) << 6) & 0b0100_0000;
                int l3 = (Convert.ToInt32(Channel2.LeftEnable) << 5) & 0b0010_0000;
                int l4 = (Convert.ToInt32(Channel1.LeftEnable) << 4) & 0b0001_0000;
                int r1 = (Convert.ToInt32(Channel4.RightEnable) << 3) & 0b0000_1000;
                int r2 = (Convert.ToInt32(Channel3.RightEnable) << 2) & 0b0000_0100;
                int r3 = (Convert.ToInt32(Channel2.RightEnable) << 1) & 0b0000_0010;
                int r4 = (Convert.ToInt32(Channel1.RightEnable) << 0) & 0b0000_0001;

                return (byte)(l1 | l2 | l3 | l4 | r1 | r2 | r3 | r4);
            }
            set
            {
                if (!NR52_Power) return;

                Channel4.LeftEnable = Convert.ToBoolean((value & 0b1000_0000) >> 7);
                Channel3.LeftEnable = Convert.ToBoolean((value & 0b0100_0000) >> 6);
                Channel2.LeftEnable = Convert.ToBoolean((value & 0b0010_0000) >> 5);
                Channel1.LeftEnable = Convert.ToBoolean((value & 0b0001_0000) >> 4);
                Channel4.RightEnable = Convert.ToBoolean((value & 0b0000_1000) >> 3);
                Channel3.RightEnable = Convert.ToBoolean((value & 0b0000_0100) >> 2);
                Channel2.RightEnable = Convert.ToBoolean((value & 0b0000_0010) >> 1);
                Channel1.RightEnable = Convert.ToBoolean((value & 0b0000_0001) >> 0);
            }
        }

        public byte NR52
        {
            get
            {
                int power = (Convert.ToInt32(NR52_Power) << 7) & 0b1000_0000;
                int l4 = (Convert.ToInt32(Channel4.LengthStatus) << 3) & 0b0000_1000;
                int l3 = (Convert.ToInt32(Channel3.LengthStatus) << 2) & 0b0000_0100;
                int l2 = (Convert.ToInt32(Channel2.LengthStatus) << 1) & 0b0000_0010;
                int l1 = (Convert.ToInt32(Channel1.LengthStatus) << 0) & 0b0000_0001;

                return (byte)(power | l4 | l3 | l2 | l1 | 0b0111_0000);
            }

            set
            {
                bool initialPower = NR52_Power;
                bool newPower  = Convert.ToBoolean((value & 0b1000_0000) << 7);

                if (!initialPower && newPower)
                {
                    // Power off => on
                    frameSequencerStep = 0;
                    Channel1.DutyStep = 0;
                    Channel2.DutyStep = 0;
                    Channel3.sampleBuffer = 0;
                }
                else if (initialPower && !newPower)
                {
                    // TODO: DMG preserves length counters on poweroff

                    // Power on => off
                    NR10 = 0;
                    NR11 = 0;
                    NR12 = 0;
                    NR13 = 0;
                    NR14 = 0;
                    NR21 = 0;
                    NR22 = 0;
                    NR23 = 0;
                    NR24 = 0;
                    NR30 = 0;
                    NR31 = 0;
                    NR32 = 0;
                    NR33 = 0;
                    NR34 = 0;
                    NR41 = 0;
                    NR42 = 0;
                    NR43 = 0;
                    NR44 = 0;
                    NR50 = 0;
                    NR51 = 0;

                    Channel1.LengthStatus = false;
                    Channel2.LengthStatus = false;
                    Channel3.LengthStatus = false;
                    Channel4.LengthStatus = false;
                }

                NR52_Power = newPower;
            }
        }

        public SquareChannel Channel1 = new SquareChannel();
        public SquareChannel Channel2 = new SquareChannel();
        public WaveChannel Channel3 = new WaveChannel();
        public NoiseChannel Channel4 = new NoiseChannel();

        public byte[] WaveTable => Channel3.WaveTable;

        private bool NR50_VinLEnable;
        private int NR50_LeftVol;
        private bool NR50_VinREnable;
        private int NR50_RightVol;
        private bool NR52_Power;

        public List<float> buffer = new List<float>(8000);

        private Gameboy gb;
        private long oldCpuTicks;
        private long timer;
        private long frameSequencerTimer;
        private long frameSequencerStep;

        private double[] capacitor = new double[2];
        private double capacitorFactor;
        private int sampleClock;

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
            long apuTicks = gb.cpu.ticks - oldCpuTicks;
            oldCpuTicks = gb.cpu.ticks;

            for (int i = 0; i < apuTicks; i++)
            {
                if (frameSequencerTimer > 0)
                {
                    frameSequencerTimer--;
                }

                if (frameSequencerTimer == 0)
                {
                    if (NR52_Power)
                    {
                        switch (frameSequencerStep)
                        {
                            case 0:
                                Channel1.LengthTimerTick();
                                Channel2.LengthTimerTick();
                                Channel3.LengthTimerTick();
                                Channel4.LengthTimerTick();
                                break;
                            case 2:
                                Channel1.SweepTick();
                                Channel1.LengthTimerTick();
                                Channel2.LengthTimerTick();
                                Channel3.LengthTimerTick();
                                Channel4.LengthTimerTick();
                                break;
                            case 4:
                                Channel1.LengthTimerTick();
                                Channel2.LengthTimerTick();
                                Channel3.LengthTimerTick();
                                Channel4.LengthTimerTick();
                                break;
                            case 6:
                                Channel1.SweepTick();
                                Channel1.LengthTimerTick();
                                Channel2.LengthTimerTick();
                                Channel3.LengthTimerTick();
                                Channel4.LengthTimerTick();
                                break;
                            case 7:
                                Channel1.VolumeEnvelopeTick();
                                Channel2.VolumeEnvelopeTick();
                                Channel4.VolumeEnvelopeTick();
                                break;
                        }

                        if (++frameSequencerStep > 7)
                        {
                            frameSequencerStep = 0;
                        }
                    }

                    frameSequencerTimer = 8192;
                }

                if (NR52_Power)
                {
                    Channel1.ClockTick();
                    Channel2.ClockTick();
                    Channel3.ClockTick();
                    Channel4.ClockTick();
                }

                if (timer == 0)
                {
                    if (NR52_Power)
                    {
                        buffer.Add(high_pass((Channel1.OutputLeft + Channel2.OutputLeft + Channel3.OutputLeft + Channel4.OutputLeft) / 4 * (NR50_LeftVol + 1), 1));
                        buffer.Add(high_pass((Channel1.OutputRight + Channel2.OutputRight + Channel3.OutputRight + Channel4.OutputRight) / 4 * (NR50_RightVol + 1), 2));
                    }
                    else
                    {
                        buffer.Add(0);
                        buffer.Add(0);
                    }

                    timer = sampleClock;
                }
                else
                {
                    timer--;
                }
            }
        }

        float high_pass(float input, int channel, bool dacs_enabled = true)
        {
            // TODO: Implement this flag
            if (dacs_enabled)
            {
                double output = input - capacitor[channel-1];

                // capacitor slowly charges to 'in' via their difference
                capacitor[channel-1] = input - output * capacitorFactor;

                return (float)output;
            }

            return 0;
        }
    }
}
