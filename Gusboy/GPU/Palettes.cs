namespace Gusboy
{
    using System;

    /// <summary>
    /// Contains palette and colour stuff.
    /// </summary>
    public partial class GPU
    {
        // private readonly int[] palBgGBL = { 0x00b284, 0x009a73, 0x00694a, 0x005139 };
        // private readonly int[] palOBgGBl = { 0x00b284, 0x009a73, 0x00694a, 0x005139 };
        // private readonly int[] palBgGrey = { 0xFFFFFF, 0xAAAAAA, 0x555555, 0x000000 };
        // private readonly int[] palObjGrey = { 0xFFFFFF, 0xAAAAAA, 0x555555, 0x000000 };
        // private readonly int[] palObjRed = { 0xF8E0D0, 0xC08870, 0x683456, 0x18081A };
        // private readonly int[] palBgDMG = { 0x7b8210, 0x5a7942, 0x39594a, 0x294139 };
        // private readonly int[] palObjDMG = { 0x7b8210, 0x5a7942, 0x39594a, 0x294139 };
        private readonly int[] palBg = { 0xE0F8D0, 0x88C070, 0x346856, 0x08181A };
        private readonly int[] palObj = { 0xE0F8D0, 0x88C070, 0x346856, 0x08181A };
        private readonly int[] palBgMGB = { 0xc6cba5, 0x8c926b, 0x4a5139, 0x181818 };
        private readonly int[] palObjMGB = { 0xc6cba5, 0x8c926b, 0x4a5139, 0x181818 };

        public int[] ColourCache { get; } = new int[32768];

        public int[,] PalCgbSprites { get; } = new int[8, 4];

        public int[,] PalCgbBackground { get; } = new int[8, 4];

        public byte BgPal
        {
            get => PackByte(this.palBgMap[0], this.palBgMap[1], this.palBgMap[2], this.palBgMap[3]);

            set => this.palBgMap = UnpackByte(value);
        }

        public byte ObjPal0
        {
            get => PackByte(this.PalObjMap[0][0], this.PalObjMap[0][1], this.PalObjMap[0][2], this.PalObjMap[0][3]);

            set
            {
                this.PalObjMap[0] = UnpackByte(value);
            }
        }

        public byte ObjPal1
        {
            get => PackByte(this.PalObjMap[1][0], this.PalObjMap[1][1], this.PalObjMap[1][2], this.PalObjMap[1][3]);

            set
            {
                this.PalObjMap[1] = UnpackByte(value);
            }
        }

        // Colour transform for 5bpc GGB to 8bpc VGA
        // From https://byuu.net/video/color-emulation/
        public static int FilterCGB(int color, bool useFilter)
        {
            int rIn = color & 0b0000_0000_0001_1111;
            int gIn = (color & 0b0000_0011_1110_0000) >> 5;
            int bIn = (color & 0b0111_1100_0000_0000) >> 10;

            if (useFilter)
            {
                int rOut = Math.Min(960, (rIn * 26) + (gIn * 4) + (bIn * 2)) >> 2;
                int gOut = Math.Min(960, (gIn * 24) + (bIn * 8)) >> 2;
                int bOut = Math.Min(960, (rIn * 6) + (gIn * 4) + (bIn * 22)) >> 2;

                return (0xFF << 24) | (rOut << 16) | (gOut << 8) | bOut;
            }
            else
            {
                int rOut = (rIn << 3) | (rIn >> 2);
                int gOut = (gIn << 3) | (gIn >> 2);
                int bOut = (bIn << 3) | (bIn >> 2);

                return (0xFF << 24) | (rOut << 16) | (gOut << 8) | bOut;
            }
        }

        private static byte PackByte(int val0, int val1, int val2, int val3) => (byte)(((val0 & 3) << 0) | ((val1 & 3) << 2) | ((val2 & 3) << 4) | ((val3 & 3) << 6));

        private static int[] UnpackByte(byte value) => new int[] { (value >> 0) & 3, (value >> 2) & 3, (value >> 4) & 3, (value >> 6) & 3 };

        private void BuildColourCache(bool useFilter)
        {
            for (int i = 0; i < 32768; i++)
            {
                this.ColourCache[i] = GPU.FilterCGB(i, useFilter);
            }
        }
    }
}
