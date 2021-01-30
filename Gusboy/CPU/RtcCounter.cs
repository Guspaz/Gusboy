namespace Gusboy
{
    // Provided by calc84maniac as an alternative to my branched RTC implementation
    public class RtcCounter
    {
        private const byte SubSecondsShift = 6;
        private const byte SecondsShift = SubSecondsShift + 15;
        private const byte MinutesShift = SecondsShift + 6;
        private const byte HoursShift = MinutesShift + 6;
        private const byte DaysShift = HoursShift + 5;
        private const byte OverflowShift = DaysShift + 9;

        private const ulong SubSecondsMask = 32767;
        private const ulong SecondsMask = 63;
        private const ulong MinutesMask = 63;
        private const ulong HoursMask = 31;
        private const ulong DaysMask = 511;
        private const ulong OverflowMask = 1;

        private const ulong SecondsOffset = 64 - 60;
        private const ulong MinutesOffset = 64 - 60;
        private const ulong HoursOffset = 32 - 24;

        private const ulong CarryMask =
            (1UL << (OverflowShift + 1)) | // Carry from overflow bit
            (1UL << DaysShift) | // Carry from hours bit
            (1UL << HoursShift) | // Carry from minutes bit
            (1UL << MinutesShift); // Carry from seconds bit

        private const ulong CarryOffsets =
            (1UL << OverflowShift) | // Keep overflow set if it carried
            (HoursOffset << HoursShift) | // Add to hours if it carried
            (MinutesOffset << MinutesShift) | // Add to minutes if it carried
            (SecondsOffset << SecondsShift); // Add to seconds if it carried

        private ulong mCounter;

        public ushort SubSeconds
        {
            get => (ushort)((this.mCounter >> SubSecondsShift) & SubSecondsMask);

            set => this.mCounter = (this.mCounter & ~(SubSecondsMask << SubSecondsShift)) |
                        (value & SubSecondsMask) << SubSecondsShift;
        }

        public byte Seconds
        {
            get => (byte)(((this.mCounter >> SecondsShift) - SecondsOffset) & SecondsMask);

            set => this.mCounter = (this.mCounter & ~(SecondsMask << SecondsShift)) |
                        ((value + SecondsOffset) & SecondsMask) << SecondsShift;
        }

        public byte Minutes
        {
            get => (byte)(((this.mCounter >> MinutesShift) - MinutesOffset) & MinutesMask);

            set => this.mCounter = (this.mCounter & ~(MinutesMask << MinutesShift)) |
                        ((value + MinutesOffset) & MinutesMask) << MinutesShift;
        }

        public byte Hours
        {
            get => (byte)(((this.mCounter >> HoursShift) - HoursOffset) & HoursMask);

            set => this.mCounter = (this.mCounter & ~(HoursMask << HoursShift)) |
                        ((value + HoursOffset) & HoursMask) << HoursShift;
        }

        // TODO: Do the high/low order bit stuff here so that MBC3 doesn't have to
        public ushort Days
        {
            get => (ushort)((this.mCounter >> DaysShift) & DaysMask);

            set => this.mCounter = (this.mCounter & ~(DaysMask << DaysShift)) |
                        (value & DaysMask) << DaysShift;
        }

        public byte Overflow
        {
            get => (byte)((this.mCounter >> OverflowShift) & OverflowMask);

            set => this.mCounter = (this.mCounter & ~(OverflowMask << OverflowShift)) |
                        (value & OverflowMask) << OverflowShift;
        }

        // Tick count to add must be less than a second in length
        public void AddTicks(uint tickCount)
        {
            // Add the tick count to the counter
            ulong newCounter = this.mCounter + tickCount;

            // Check for relevant carried bits, simple XOR is enough due to tickCount constraint
            ulong carryMask = (this.mCounter ^ newCounter) & CarryMask;

            // Get a 5-bit mask below each carry (cannot be more or fields overlap)
            // This works because SecondsOffset and MinutesOffset are not odd
            carryMask -= carryMask >> 5;

            // Add offsets to any field that carried
            this.mCounter = newCounter + (carryMask & CarryOffsets);
        }
    }
}
