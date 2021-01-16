namespace NAudio.Wave
{
    using System;
    using NAudio.Utils;

    /// <summary>
    /// Provides a very stripped-down version of NAudio's BufferedWaveProvider that uses a callback to populate the buffer to service reads.
    /// </summary>
    public class BufferedCallbackWaveProvider : IWaveProvider
    {
        private readonly WaveFormat waveFormat;
        private readonly CircularBuffer circularBuffer;
        private readonly Func<BufferedCallbackWaveProvider, object, bool> callback;
        private readonly object callbackParam;

        public BufferedCallbackWaveProvider(WaveFormat waveFormat, Func<BufferedCallbackWaveProvider, object, bool> callback, object callbackParam)
        {
            this.waveFormat = waveFormat;
            this.callback = callback;
            this.callbackParam = callbackParam;
            this.circularBuffer = new CircularBuffer(waveFormat.AverageBytesPerSecond);
        }

        public WaveFormat WaveFormat
        {
            get { return this.waveFormat; }
        }

        public void AddSamples(byte[] buffer, int offset, int count)
        {
            this.circularBuffer.Write(buffer, offset, count);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            while (this.circularBuffer.Count < count)
            {
                this.callback(this, this.callbackParam);
            }

            return this.circularBuffer.Read(buffer, offset, count);
        }
    }
}