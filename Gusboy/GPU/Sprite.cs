namespace Gusboy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Sprite stuff.
    /// </summary>
    public partial class GPU
    {
        private void RenderScanlineSprites()
        {
            // Sprites
            if (this.LCDCFlag(LCDC.SpritesEnabled))
            {
                byte spriteHeight = (byte)(this.LCDCFlag(LCDC.SpriteSize) ? 16 : 8);

                IEnumerable<Sprite.StaticSprite> prioritySprites;

                if (this.OAMPriorityMode)
                {
                    prioritySprites = this.spriteCache.Where(s => (byte)(this.CurrentLine - s.Y) < spriteHeight).Take(10).OrderByDescending(s => s.OamNum).Select(s => s.Static);
                }
                else
                {
                    prioritySprites = this.spriteCache.Where(s => (byte)(this.CurrentLine - s.Y) < spriteHeight).Take(10).OrderBy(s => s.X).Reverse().Select(s => s.Static);
                }

                foreach (var currentSprite in prioritySprites)
                {
                    // Delay HBLANK for sprites
                    // if (currentSprite.X >= this.WinX)
                    // {
                    //     this.delayTicks += 11 - Math.Min(5, (currentSprite.X + (255 - this.WinX)) % 8);
                    // }
                    // else
                    // {
                    //     this.delayTicks += 11 - Math.Min(5, (currentSprite.X + this.ScrollX) % 8);
                    // }

                    // Sprite appears on this scanline, draw it.
                    for (int i = 0; i < 8; i++)
                    {
                        byte spriteRelY = (byte)(this.CurrentLine - currentSprite.Y);
                        byte spriteRelX = (byte)i;

                        if (currentSprite.XFlip)
                        {
                            spriteRelX = (byte)((spriteRelX - 7) * -1);
                        }

                        if (currentSprite.YFlip)
                        {
                            spriteRelY = (byte)(spriteHeight - 1 - spriteRelY);
                        }

                        byte tileNum = currentSprite.TileNum;
                        if (spriteHeight == 16)
                        {
                            if (spriteRelY < 8)
                            {
                                tileNum &= 0xFE;
                            }
                            else
                            {
                                tileNum |= 1;
                                spriteRelY -= 8;
                            }
                        }

                        int tileAddress = ((tileNum * TILE_HEIGHT) + (spriteRelY & 7)) * TILE_ROW_BYTES;

                        byte shift = (byte)(7 - (spriteRelX & 7));

                        byte palIndex = (byte)((((this.gb.Ram.Vram[currentSprite.VramBank, tileAddress + 1] >> shift) & 1) << 1) | ((this.gb.Ram.Vram[currentSprite.VramBank, tileAddress] >> shift) & 1));

                        byte finalX = (byte)(currentSprite.X + i);

                        if (
                            palIndex != 0
                            && finalX < 160
                            && (!currentSprite.Priority || this.bgIsTransparent[finalX])
                            && (!this.bgPriority[finalX] || this.bgIsTransparent[finalX]))
                        {
                            this.framebuffer[(this.CurrentLine * 160) + (byte)(currentSprite.X + i)] = currentSprite.MappedPalette[palIndex];
                        }
                    }
                }
            }
        }

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
}