namespace GusBoy
{
    public class WaveChannel
    {
        public int LengthLoad
        {
            set
            {
                LengthTimer = 256 - value;
            }
        }

        public bool DacEnable;

        public int LengthTimer;
        public bool LengthStatus;
        public bool LengthEnable;

        public int Volume;

        public int Frequency;
        public int FrequencyTimer;

        public byte[] WaveTable = new byte[16];

        public bool LeftEnable;
        public bool RightEnable;

        public int sampleBuffer;
        private int wavePosition;


        private float[] VolumeFactor = { 0.00f, 1.00f, 0.50f, 0.25f };

        public float OutputLeft => (LengthStatus && LeftEnable) ? (sampleBuffer * VolumeFactor[Volume]) / 100f : 0;

        public float OutputRight => (LengthStatus && RightEnable) ? (sampleBuffer * VolumeFactor[Volume]) / 100f : 0;

        public void Trigger()
        {
            this.LengthStatus = this.DacEnable;

            if (this.LengthTimer == 0)
            {
                this.LengthTimer = 256;
            }

            // Per Binji: "The trick is to add a 6 cycle delay to the wave period whenever it is triggered." Dunno if this is needed.
            this.FrequencyTimer = (2048 - this.Frequency) * 2;// + 6;

            this.wavePosition = 0;
        }

        public void ClockTick()
        {
            if (FrequencyTimer > 0)
            {
                FrequencyTimer--;
            }

            if (FrequencyTimer == 0)
            {
                FrequencyTimer = (2048 - this.Frequency) * 2;

                if (++wavePosition > 31)
                {
                    wavePosition = 0;
                }

                if ((wavePosition & 0b0000_0001) == 0)
                {
                    sampleBuffer = (WaveTable[wavePosition >> 1] >> 4) & 0b0000_1111;
                }
                else
                {
                    sampleBuffer = WaveTable[wavePosition >> 1] & 0b0000_1111;
                }
            }
        }

        public void LengthTimerTick()
        {
            if (LengthEnable)
            {
                if (LengthTimer > 0)
                {
                    LengthTimer--;
                }

                if (LengthTimer == 0)
                {
                    LengthStatus = false;
                }
            }
        }
    }
}
