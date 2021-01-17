namespace Gusboy
{
    public class WaveChannel : Channel
    {
        private readonly int[] volumeFactor = { 4, 0, 1, 2 };

        private int wavePosition;

        public int Volume { get; set; }

        public byte[] WaveTable { get; set; } = new byte[16];

        public int SampleBuffer { get; set; }

        public int Frequency { get; set; }

        protected override int DigitalOutput => this.SampleBuffer >> this.volumeFactor[this.Volume];

        protected override int LengthWidth => 256;

        public void Trigger()
        {
            this.ChannelEnable = this.DacEnable;

            if (this.LengthTimer == 0)
            {
                this.LengthTimer = this.LengthWidth;
            }

            // Per Binji: "The trick is to add a 6 cycle delay to the wave period whenever it is triggered." Dunno if this is actually needed.
            this.FrequencyTimer = (2048 - this.Frequency) * 2; // + 6;

            this.wavePosition = 0;
        }

        protected override int FrequencyTimerFire()
        {
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

            return (2048 - this.Frequency) * 2;
        }
    }
}
