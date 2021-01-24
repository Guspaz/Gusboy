namespace Gusboy
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Background tile stuff.
    /// </summary>
    public partial class GPU
    {
        private const int TILE_MAP_WIDTH = 32;
        private const int TILE_HEIGHT = 8;
        private const int TILE_ROW_BYTES = 2;

        private readonly Tile[] tileCache = new Tile[32 * 32 * 2];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CacheTile(int n)
        {
            if (this.tileCache[n].Dirty)
            {
                this.tileCache[n].Dirty = false;

                int ramValue = this.gb.Ram.Vram[1, 0x1800 | n];
                int paletteIndex = ramValue & 0b111;

                this.tileCache[n].OAMPriority = (ramValue & (1 << 7)) != 0;
                this.tileCache[n].YFlip = (ramValue & (1 << 6)) != 0;
                this.tileCache[n].XFlip = (ramValue & (1 << 5)) != 0;
                this.tileCache[n].VramBank = (ramValue >> 3) & 1;
                this.tileCache[n].PaletteIndex = paletteIndex;

                // DMG doesn't use this cached palette
                this.tileCache[n].MappedPalette[0] = this.ColourCache[this.PalCgbBackground[paletteIndex, 0]];
                this.tileCache[n].MappedPalette[1] = this.ColourCache[this.PalCgbBackground[paletteIndex, 1]];
                this.tileCache[n].MappedPalette[2] = this.ColourCache[this.PalCgbBackground[paletteIndex, 2]];
                this.tileCache[n].MappedPalette[3] = this.ColourCache[this.PalCgbBackground[paletteIndex, 3]];
            }
        }

        private void RenderScanlineTiles(bool[] bgIsTransparent, bool[] bgPriority)
        {
            if (!this.gb.IsCgb && !this.LCDCFlag(LCDC.BGEnabled))
            {
                Array.Fill(this.framebuffer, this.palBg[this.palBgMap[0]], this.CurrentLine * 160, 160);
            }
            else if (this.gb.IsCgb || this.LCDCFlag(LCDC.BGEnabled) || this.renderingWindow)
            {
                byte x;
                byte y;
                int tileDataAddress;
                LCDC tilemapFlag;
                int tileNum;

                // Invalidate the tile cache if it's dirty
                if (this.BackgroundCacheDirty)
                {
                    for (int i = 0; i < this.tileCache.Length; i++)
                    {
                        this.tileCache[i].Dirty = true;
                    }

                    this.BackgroundCacheDirty = false;
                }

                // Delay HBLANK for the background scroll
                // TODO: Should this happen if we're in the window?
                this.delayTicks += this.ScrollX % 8;
                bool windowDelay = false;

                bool precomputeRenderingWindow = this.LCDCFlag(LCDC.WindowEnable)
                            && this.CurrentLine >= this.startWinY
                            && this.WinX <= WINDOW_MAX_X;

                int precomputeCurrentLineOffset = this.CurrentLine * 160;

                for (int i = 0; i < 160; i++)
                {
                    this.renderingWindow = precomputeRenderingWindow
                            && i + WINDOW_X_OFFSET >= this.WinX;

                    if (this.renderingWindow)
                    {
                        x = (byte)(i + WINDOW_X_OFFSET - this.WinX);
                        y = this.currentWinY;
                        tilemapFlag = LCDC.WindowTileMap;

                        // Delay HBLANK for the window
                        if (!windowDelay)
                        {
                            this.delayTicks += 6;
                            windowDelay = true;
                        }
                    }
                    else
                    {
                        x = (byte)(this.ScrollX + i);
                        y = (byte)(this.ScrollY + this.CurrentLine);
                        tilemapFlag = LCDC.BGTileMap;
                    }

                    // This is the number of the tile in the tile map (0-2047)
                    tileNum = ((y / 8) * TILE_MAP_WIDTH) | (x / 8);

                    if (this.LCDCFlag(tilemapFlag))
                    {
                        tileNum += 1024;
                    }

                    // Add this tile to the cache if it isn't already there
                    this.CacheTile(tileNum);

                    // The tilemap is always in vram bank 0 since bank 1 contains the CGB attributes
                    // This is the actual address of the tile data
                    byte tileLookupValue = this.gb.Ram.Vram[0, 0x1800 + tileNum];

                    // This is wrong because I don't take X/Y into account yet
                    if (this.LCDCFlag(LCDC.BGWindowTileset))
                    {
                        tileDataAddress = tileLookupValue * 16;
                    }
                    else
                    {
                        tileDataAddress = 0x1000 + ((sbyte)tileLookupValue * 16);
                    }

                    // CGB X Flip (will always be false on DMG)
                    if (this.tileCache[tileNum].XFlip)
                    {
                        x = (byte)(7 - x);
                    }

                    // CGB Y Flip (will always be false on DMG)
                    if (this.tileCache[tileNum].YFlip)
                    {
                        y = (byte)(7 - y);
                    }

                    // Get to the right row based on the y position
                    tileDataAddress += (y & 7) * 2;

                    // Used to get the specific pixel from the row based on X position
                    byte shift = (byte)(7 - (x & 7));

                    // Grab the specific pixel from the row
                    byte palIndex = (byte)((((this.gb.Ram.Vram[this.tileCache[tileNum].VramBank, tileDataAddress + 1] >> shift) & 1) << 1) | ((this.gb.Ram.Vram[this.tileCache[tileNum].VramBank, tileDataAddress] >> shift) & 1));

                    // Used  later for rendering sprites
                    bgIsTransparent[i] = palIndex == 0;
                    bgPriority[i] = this.gb.IsCgb && this.LCDCFlag(LCDC.BGEnabled) && this.tileCache[tileNum].OAMPriority;

                    if (this.gb.IsCgb)
                    {
                        this.framebuffer[precomputeCurrentLineOffset + i] = this.tileCache[tileNum].MappedPalette[palIndex];
                    }
                    else
                    {
                        this.framebuffer[precomputeCurrentLineOffset + i] = this.palBg[this.palBgMap[palIndex]];
                    }
                }
            }
        }

        private struct Tile
        {
            public bool Dirty;

            public bool OAMPriority;
            public bool YFlip; // Vertical
            public bool XFlip; // Horizontal
            public int VramBank;
            public int PaletteIndex;

            public int[] MappedPalette;
        }
    }
}
