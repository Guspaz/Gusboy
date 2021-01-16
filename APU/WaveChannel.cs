namespace GusBoy
{
    public class WaveChannel
    {
        public int LengthLoad
        {
            set => this.LengthTimer = 256 - value;
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


        private readonly float[] VolumeFactor = { 0.00f, 1.00f, 0.50f, 0.25f };

        public float OutputLeft => (this.LengthStatus && this.LeftEnable) ? (this.sampleBuffer * this.VolumeFactor[this.Volume]) / 100f : 0;

        public float OutputRight => (this.LengthStatus && this.RightEnable) ? (this.sampleBuffer * this.VolumeFactor[this.Volume]) / 100f : 0;

        public void Trigger()
        {
            this.LengthStatus = this.DacEnable;

            if ( this.LengthTimer == 0 )
            {
                this.LengthTimer = 256;
            }

            // Per Binji: "The trick is to add a 6 cycle delay to the wave period whenever it is triggered." Dunno if this is needed.
            this.FrequencyTimer = (2048 - this.Frequency) * 2;// + 6;

            this.wavePosition = 0;
        }

        public void ClockTick()
        {
            if ( this.FrequencyTimer > 0 )
            {
                this.FrequencyTimer--;
            }

            if ( this.FrequencyTimer == 0 )
            {
                this.FrequencyTimer = (2048 - this.Frequency) * 2;

                if ( ++this.wavePosition > 31 )
                {
                    this.wavePosition = 0;
                }

                if ( (this.wavePosition & 0b0000_0001) == 0 )
                {
                    this.sampleBuffer = (this.WaveTable[this.wavePosition >> 1] >> 4) & 0b0000_1111;
                }
                else
                {
                    this.sampleBuffer = this.WaveTable[this.wavePosition >> 1] & 0b0000_1111;
                }
            }
        }

        public void LengthTimerTick()
        {
            if ( this.LengthEnable )
            {
                if ( this.LengthTimer > 0 )
                {
                    this.LengthTimer--;
                }

                if ( this.LengthTimer == 0 )
                {
                    this.LengthStatus = false;
                }
            }
        }
    }
}
