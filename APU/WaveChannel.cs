namespace Gusboy
{
    public class WaveChannel
    {
        private readonly float[] volumeFactor = { 0.00f, 1.00f, 0.50f, 0.25f };

        private int lengthTimer;
        private int frequencyTimer;
        private int wavePosition;

        public int LengthLoad
        {
            set => this.lengthTimer = 256 - value;
        }

        public bool DacEnable { get; set; }

        public bool LengthStatus { get; set; }

        public bool LengthEnable { get; set; }

        public int Volume { get; set; }

        public byte[] WaveTable { get; set; } = new byte[16];

        public bool LeftEnable { get; set; }

        public bool RightEnable { get; set; }

        public int SampleBuffer { get; set; }

        public int Frequency { get; set; }

        public float OutputLeft => (this.LengthStatus && this.LeftEnable) ? (this.SampleBuffer * this.volumeFactor[this.Volume]) / 100f : 0;

        public float OutputRight => (this.LengthStatus && this.RightEnable) ? (this.SampleBuffer * this.volumeFactor[this.Volume]) / 100f : 0;

        public void Trigger()
        {
            this.LengthStatus = this.DacEnable;

            if (this.lengthTimer == 0)
            {
                this.lengthTimer = 256;
            }

            // Per Binji: "The trick is to add a 6 cycle delay to the wave period whenever it is triggered." Dunno if this is actually needed.
            this.frequencyTimer = (2048 - this.Frequency) * 2; // + 6;

            this.wavePosition = 0;
        }

        public void ClockTick()
        {
            if (this.frequencyTimer > 0)
            {
                this.frequencyTimer--;
            }

            if (this.frequencyTimer == 0)
            {
                this.frequencyTimer = (2048 - this.Frequency) * 2;

                if (++this.wavePosition > 31)
                {
                    this.wavePosition = 0;
                }

                if ((this.wavePosition & 0b0000_0001) == 0)
                {
                    this.SampleBuffer = (this.WaveTable[this.wavePosition >> 1] >> 4) & 0b0000_1111;
                }
                else
                {
                    this.SampleBuffer = this.WaveTable[this.wavePosition >> 1] & 0b0000_1111;
                }
            }
        }

        public void LengthTimerTick()
        {
            if (this.LengthEnable)
            {
                if (this.lengthTimer > 0)
                {
                    this.lengthTimer--;
                }

                if (this.lengthTimer == 0)
                {
                    this.LengthStatus = false;
                }
            }
        }
    }
}
