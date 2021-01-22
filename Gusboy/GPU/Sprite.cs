namespace Gusboy
{
    public class Sprite
    {
        private readonly byte index;
        private readonly int baseAddress;
        private readonly Gameboy gb;
        private readonly int[] mappedPalette = new int[4];
        private readonly int[] palObj;

        public Sprite(byte index, Gameboy gb, int[] palObj)
        {
            this.index = index;
            this.baseAddress = 0xFE00 + (index << 2);
            this.gb = gb;
            this.palObj = palObj;
        }

        public byte X => (byte)(this.gb.Ram[this.baseAddress + 1, isDma: true] - 8); // Pre-offset the location

        public byte Y => (byte)(this.gb.Ram[this.baseAddress + 0, isDma: true] - 16); // Pre-offset the location

        public byte OamNum => this.index;

        public StaticSprite Static
        {
            get
            {
                int paletteIndex = this.gb.IsCgb ? this.gb.Ram[this.baseAddress + 3, isDma: true] & 0b0000_0111 : (this.gb.Ram[this.baseAddress + 3, isDma: true] & (1 << 4)) >> 4;

                if (this.gb.IsCgb)
                {
                    this.mappedPalette[0] = this.gb.Gpu.ColourCache[this.gb.Gpu.PalCgbSprites[paletteIndex, 0]];
                    this.mappedPalette[1] = this.gb.Gpu.ColourCache[this.gb.Gpu.PalCgbSprites[paletteIndex, 1]];
                    this.mappedPalette[2] = this.gb.Gpu.ColourCache[this.gb.Gpu.PalCgbSprites[paletteIndex, 2]];
                    this.mappedPalette[3] = this.gb.Gpu.ColourCache[this.gb.Gpu.PalCgbSprites[paletteIndex, 3]];
                }
                else
                {
                    this.mappedPalette[0] = this.palObj[this.gb.Gpu.PalObjMap[paletteIndex][0]];
                    this.mappedPalette[1] = this.palObj[this.gb.Gpu.PalObjMap[paletteIndex][1]];
                    this.mappedPalette[2] = this.palObj[this.gb.Gpu.PalObjMap[paletteIndex][2]];
                    this.mappedPalette[3] = this.palObj[this.gb.Gpu.PalObjMap[paletteIndex][3]];
                }

                return new StaticSprite
                {
                    X = this.X,
                    Y = this.Y,
                    TileNum = this.gb.Ram[this.baseAddress + 2, isDma: true],
                    OamNum = this.OamNum,
                    Priority = (this.gb.Ram[this.baseAddress + 3, isDma: true] & (1 << 7)) != 0,
                    YFlip = (this.gb.Ram[this.baseAddress + 3, isDma: true] & (1 << 6)) != 0, // Vertical
                    XFlip = (this.gb.Ram[this.baseAddress + 3, isDma: true] & (1 << 5)) != 0, // Horizontal
                    VramBank = (this.gb.Ram[this.baseAddress + 3, isDma: true] >> 3) & 1,
                    PaletteIndex = paletteIndex,
                    MappedPalette = this.mappedPalette,
                };
            }
        }

        public struct StaticSprite
        {
            public byte Y;
            public byte X;
            public byte TileNum;
            public byte OamNum;
            public bool Priority;
            public bool YFlip;
            public bool XFlip;
            public int VramBank;
            public int PaletteIndex;
            public int[] MappedPalette;
        }
    }
}