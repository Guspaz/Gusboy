﻿namespace Gusboy
{
    using System;
    using System.Linq;

    public class GPU
    {
        // private const int TIME_DMA = 648;
        // private const int TIME_DMA_DELAY = 8;
        // private const long FRAME_DURATION = (long)(10000000.0 / (4194304.0 / (TIME_VBLANK * (MAX_LINE + VBLANK_LENGTH + 1.0))));
        private const int TIME_HBLANK = 204;
        private const int TIME_OAM = 80;
        private const int TIME_VRAM = 172;
        private const int TIME_VBLANK = TIME_OAM + TIME_VRAM + TIME_HBLANK; // Time per scanline, vblank will be 10 of these
        private const int MAX_LINE = 143;
        private const int VBLANK_LENGTH = 10; // Number of scanlines
        private const int TILE_MAP_WIDTH = 32;
        private const int TILE_HEIGHT = 8;
        private const int TILE_ROW_BYTES = 2;
        private const int WINDOW_X_OFFSET = 7;
        private const int WINDOW_MAX_X = 166;

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

        private readonly Gameboy gb;
        private readonly Sprite[] sprites = new Sprite[40];
        private readonly Tile[] tiles = new Tile[32 * 32 * 2];
        private readonly int[] framebuffer;
        private readonly Func<bool> drawFramebuffer;
        private readonly int[][] palObjMap = new int[4][];
        private int[] palBgMap = new int[4];
        private byte stat = 0;
        private GPUMode mode = GPUMode.HBLANK;
        private GPUMode oldMode = GPUMode.HBLANK;
        private byte control = 0;
        private bool lcdWasOff = true; // LCD starts off
        private long gpuTicks = 0;
        private long oldCpuTicks = 0;
        private byte currentLine = 0;
        private byte startWinY;
        private byte currentWinY;
        private bool renderingWindow = false;
        private byte currentLineCompare;
        private byte hdmaControl;

        public GPU(Gameboy gameBoy, Func<bool> drawFramebuffer, int[] framebuffer)
        {
            this.gb = gameBoy;

            // Initialize sprite array
            for (int i = 0; i < this.sprites.Length; i++)
            {
                this.sprites[i] = default;
            }

            // Initialize background tile array
            for (int i = 0; i < this.tiles.Length; i++)
            {
                this.tiles[i] = default;
                this.tiles[i].MappedPalette = new int[] { 0xFFFFFF, 0xFFFFFF, 0xFFFFFF, 0xFFFFFF };
            }

            this.drawFramebuffer = drawFramebuffer;
            this.framebuffer = framebuffer;

            // Pre-set palette alpha
            for (int i = 0; i < 4; i++)
            {
                this.palBg[i] |= 0xFF << 24;
                this.palObj[i] |= 0xFF << 24;
            }

            // Init object palette maps so they're at least not undefined
            this.palObjMap[0] = new int[4];
            this.palObjMap[1] = new int[4];

            // Select palette
            this.palObj = this.palObjMGB;
            this.palBg = this.palBgMGB;
        }

        private enum GPUMode
        {
            HBLANK = 0,
            VBLANK = 1,
            OAM = 2,
            VRAM = 3,
        }

        [Flags]
        private enum LCDC
        {
            SpritesEnabled = 1 << 1,
            BGEnabled = 1 << 0,
            SpriteSize = 1 << 2, // 0 = 8x8, 1 = 8x16
            BGTileMap = 1 << 3,
            BGWindowTileset = 1 << 4,
            WindowEnable = 1 << 5,
            WindowTileMap = 1 << 6,
            LCDPower = 1 << 7,
        }

        public bool IsDmaActive { get; set; }

        public bool SpriteCacheDirty { get; set; }

        public bool BackgroundCacheDirty { get; set; }

        public int[,] PalCgbSprites { get; } = new int[8, 4];

        public int[,] PalCgbBackground { get; } = new int[8, 4];

        public byte Stat
        {
            get => (byte)((this.stat & 0xFC) | (byte)this.mode);

            set => this.stat = value;
        }

        public byte BgPal
        {
            get => PackByte(this.palBgMap[0], this.palBgMap[1], this.palBgMap[2], this.palBgMap[3]);

            set => this.palBgMap = UnpackByte(value);
        }

        public byte ObjPal0
        {
            get => PackByte(this.palObjMap[0][0], this.palObjMap[0][1], this.palObjMap[0][2], this.palObjMap[0][3]);

            set
            {
                this.palObjMap[0] = UnpackByte(value);
                this.SpriteCacheDirty = true;
            }
        }

        public byte ObjPal1
        {
            get => PackByte(this.palObjMap[1][0], this.palObjMap[1][1], this.palObjMap[1][2], this.palObjMap[1][3]);

            set
            {
                this.palObjMap[1] = UnpackByte(value);
                this.SpriteCacheDirty = true;
            }
        }

        public byte CurrentLine
        {
            get => this.currentLine;

            set
            {
                this.currentLine = value;

                this.CheckLYCInterrupt();
            }
        }

        public byte ScrollX { get; set; }

        public byte ScrollY { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Register")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Register")]
        public byte rLYC
        {
            get => this.currentLineCompare;
            set
            {
                this.currentLineCompare = value;
                this.CheckLYCInterrupt();
            }
        }

        public byte WinX { get; set; }

        public byte WinY { get; set; }

        public bool OAMPriorityMode { get; set; } // TODO: Implement me.

        public byte HDMA1SourceHi { get; set; }

        public byte HDMA2SourceLo { get; set; }

        public byte HDMA3DestHi { get; set; }

        public byte HDMA4DestLo { get; set; }

        // CGB DMA
        public byte HDMA5Control
        {
            get => this.hdmaControl;

            set
            {
                this.hdmaControl = value;

                int source = (this.HDMA1SourceHi << 8 | this.HDMA2SourceLo) & 0b1111_1111_1111_0000;
                int destination = (this.HDMA3DestHi << 8 | this.HDMA4DestLo) & 0b0001_1111_1111_0000;
                bool hdma = (value & 0b1000_0000) != 0;
                int size = ((value & 0b0111_1111) + 1) * 16;
                int transferTime = 4 + ((32 * (size / 16)) * (this.gb.Cpu.fSpeed ? 2 : 1));

                this.BackgroundCacheDirty = true;

                // TODO: Enforce invalid source address behaviour
                if (hdma)
                {
                    // TODO: Put a real implementation here, it can be changed in-flight and this is not going to be accurate
                    for (int i = 0; i < size; i++)
                    {
                        this.gb.Ram[source + i, isDma: true] = this.gb.Ram[destination + i, isDma: true];
                    }

                    this.gb.Cpu.Ticks += transferTime;
                }
                else
                {
                    // GDMA
                    for (int i = 0; i < size; i++)
                    {
                        this.gb.Ram[source + i, isDma: true] = this.gb.Ram[destination + i, isDma: true];
                    }

                    this.gb.Cpu.Ticks += transferTime;
                }
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

        // DMG DMA
        public void TriggerDMA(byte address)
        {
            this.IsDmaActive = true;

            int dmaAddress = address << 8;

            // In a real DMG this should happen bit by bit during normal execution, not all in one chunk
            for (int i = 0; i < 0xA0; i++)
            {
                this.gb.Ram[0xFE00 + i, isDma: true] = this.gb.Ram[dmaAddress + i, isDma: true];
            }

            // TODO: Ideally we should keep this set to true until a timer elapses for how long DMA is supposed to take, so that memory access can be restricted to HRAM (FF80-FFFE) during that period.
            this.IsDmaActive = false;

            // TODO: We should add the transfer time to the CPU clocks maybe? Not if it happens in the background though.
        }

        public void CacheSprites()
        {
            for (int n = 0; n < this.sprites.Length; n++)
            {
                int baseAddress = 0xFE00 + (n << 2);

                // TODO: Pull directly from VRAM to use the right bank? Gotta figure out GPU VRAM banking. I don't think OAM is banked.
                this.sprites[n].Y = (byte)(this.gb.Ram[baseAddress + 0] - 16); // Pre-offset the location

                this.sprites[n].X = (byte)(this.gb.Ram[baseAddress + 1] - 8); // Pre-offset the location

                this.sprites[n].TileNum = this.gb.Ram[baseAddress + 2];

                this.sprites[n].Priority = (this.gb.Ram[baseAddress + 3] & (1 << 7)) != 0;
                this.sprites[n].YFlip = (this.gb.Ram[baseAddress + 3] & (1 << 6)) != 0;
                this.sprites[n].XFlip = (this.gb.Ram[baseAddress + 3] & (1 << 5)) != 0;
                this.sprites[n].VramBank = (this.gb.Ram[baseAddress + 3] >> 3) & 1;

                if (this.gb.IsCgb)
                {
                    this.sprites[n].PaletteIndex = this.gb.Ram[baseAddress + 3] & 0b0000_0111;

                    this.sprites[n].MappedPalette = new[]
                    {
                        GPU.FilterCGB(this.PalCgbSprites[this.sprites[n].PaletteIndex, 0], this.gb.UseFilter),
                        GPU.FilterCGB(this.PalCgbSprites[this.sprites[n].PaletteIndex, 1], this.gb.UseFilter),
                        GPU.FilterCGB(this.PalCgbSprites[this.sprites[n].PaletteIndex, 2], this.gb.UseFilter),
                        GPU.FilterCGB(this.PalCgbSprites[this.sprites[n].PaletteIndex, 3], this.gb.UseFilter),
                    };
                }
                else
                {
                    this.sprites[n].PaletteIndex = (this.gb.Ram[baseAddress + 3] & (1 << 4)) >> 4;

                    this.sprites[n].MappedPalette = new[]
                    {
                        this.palObj[this.palObjMap[this.sprites[n].PaletteIndex][0]],
                        this.palObj[this.palObjMap[this.sprites[n].PaletteIndex][1]],
                        this.palObj[this.palObjMap[this.sprites[n].PaletteIndex][2]],
                        this.palObj[this.palObjMap[this.sprites[n].PaletteIndex][3]],
                    };
                }
            }

            this.SpriteCacheDirty = false;
        }

        public void CacheBackgrounds()
        {
            // TODO: This is super slow, need to only invalidate the specific tiles that use a given palette. Or do the palettes by reference and dirty this cache selectively when the tile attribute table is updated only.
            for (int n = 0; n < this.tiles.Length; n++)
            {
                int baseAddress = 0x1800 | n;

                this.tiles[n].OAMPriority = (this.gb.Ram.Vram[1, baseAddress] & (1 << 7)) != 0;
                this.tiles[n].YFlip = (this.gb.Ram.Vram[1, baseAddress] & (1 << 6)) != 0;
                this.tiles[n].XFlip = (this.gb.Ram.Vram[1, baseAddress] & (1 << 5)) != 0;
                this.tiles[n].VramBank = (this.gb.Ram.Vram[1, baseAddress] >> 4) & 1;
                this.tiles[n].PaletteIndex = this.gb.Ram.Vram[1, baseAddress] & 0b111;

                // TODO: Fill this in for both CGB and DMG
                // TODO: Do this by reference somehow? It's silly filtering it over and over again.
                this.tiles[n].MappedPalette = new[]
                {
                    GPU.FilterCGB(this.PalCgbBackground[this.tiles[n].PaletteIndex, 0], this.gb.UseFilter),
                    GPU.FilterCGB(this.PalCgbBackground[this.tiles[n].PaletteIndex, 1], this.gb.UseFilter),
                    GPU.FilterCGB(this.PalCgbBackground[this.tiles[n].PaletteIndex, 2], this.gb.UseFilter),
                    GPU.FilterCGB(this.PalCgbBackground[this.tiles[n].PaletteIndex, 3], this.gb.UseFilter),
                };

                this.BackgroundCacheDirty = false;
            }
        }

        public void CheckLYCInterrupt()
        {
            // TODO: This doesn't seem to always fire at the right time, sometimes it's of by one scanline...
            if (this.CurrentLine == this.rLYC)
            {
                // Set match bit
                this.stat |= 1 << 2;

                // LYC = LY status interrupt
                if ((this.stat & (1 << 6)) != 0)
                {
                    this.gb.Cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                }
            }
            else
            {
                // Clear match bit
                this.stat &= 0xFB;
            }
        }

        public byte GetLCDC() => this.control;

        public void SetLCDC(byte value)
        {
            this.control = value;

            // https://www.reddit.com/r/Gameboy/comments/a1c8h0/what_happens_when_a_gameboy_screen_is_disabled/eap4f8c/
            if (!this.LCDCFlag(LCDC.LCDPower))
            {
                this.currentLine = 0; // Reset rLY to 0
                this.gpuTicks = 0; // Reset GPU clock
                this.mode = GPUMode.HBLANK; // Reset mode to 0

                // Clear framebuffer
                if (!this.LCDCFlag(LCDC.LCDPower))
                {
                    this.ClearFramebuffer();
                    this.drawFramebuffer();

                    // Used to skip the first frame after re-enabling it
                    this.lcdWasOff = true;
                }
            }
        }

        // Always access when LCD power is off, but that forces mode 0 anyway. DMA period blocks access but the memory mapper handles that globally.
        // DMA itself gets to bypass the restriction, I think?
        // Disabled for now because my timing isn't accurate enough to enforce this properly.
        public bool CanAccessOAM(bool isDma) => this.mode == GPUMode.HBLANK || this.mode == GPUMode.VBLANK || isDma || true;

        // Always access when LCD power is off, but that forces mode 0 anyway. DMA periodblocks access but the memory mapper handles that globally.
        // DMA itself gets to bypass the restriction, I think?
        // Disabled for now because my timing isn't accurate enough to enforce this properly.
        public bool CanAccessVRAM(bool isDma) => this.mode == GPUMode.HBLANK || this.mode == GPUMode.VBLANK || this.mode == GPUMode.OAM || isDma || true;

        public void Tick()
        {
            if (!this.LCDCFlag(LCDC.LCDPower))
            {
                this.oldCpuTicks = this.gb.Cpu.Ticks;
                return;
            }

            this.gpuTicks += (this.gb.Cpu.Ticks - this.oldCpuTicks) >> (this.gb.Cpu.fSpeed ? 1 : 0);
            this.oldCpuTicks = this.gb.Cpu.Ticks;

            switch (this.mode)
            {
                case GPUMode.HBLANK:
                    if (this.oldMode != this.mode)
                    {
                        this.oldMode = this.mode;

                        if ((this.stat & (1 << 3)) != 0)
                        {
                            this.gb.Cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                        }
                    }

                    // Switch to VBLANK mode
                    if (this.gpuTicks >= TIME_HBLANK)
                    {
                        this.gpuTicks -= TIME_HBLANK;

                        this.RenderScanline();
                        this.CurrentLine++;

                        if (this.renderingWindow)
                        {
                            this.currentWinY++;
                        }

                        if (this.CurrentLine == MAX_LINE + 1)
                        {
                            this.mode = GPUMode.VBLANK;

                            // The first frame after turning the LCD on is skipped (some games show garbage on this frame, real hardware doesn't draw it)
                            if (this.lcdWasOff)
                            {
                                // Draw blank frame
                                this.ClearFramebuffer();
                                this.lcdWasOff = false;
                            }

                            this.drawFramebuffer();
                        }
                        else
                        {
                            this.mode = GPUMode.OAM;
                        }
                    }

                    break;
                case GPUMode.VBLANK:

                    if (this.oldMode != this.mode)
                    {
                        this.oldMode = this.mode;

                        this.gb.Cpu.TriggerInterrupt(CPU.INT_VBLANK);

                        if ((this.stat & (1 << 4)) != 0)
                        {
                            this.gb.Cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                        }
                    }

                    // Switch to OAM mode
                    if (this.gpuTicks >= TIME_VBLANK)
                    {
                        this.gpuTicks -= TIME_VBLANK;
                        this.CurrentLine++;

                        if (this.renderingWindow)
                        {
                            this.currentWinY++;
                        }

                        if (this.CurrentLine > MAX_LINE + VBLANK_LENGTH + 1)
                        {
                            this.renderingWindow = false;

                            this.mode = GPUMode.OAM;
                            this.CurrentLine = 0;
                            this.startWinY = this.WinY;
                            this.currentWinY = 0;
                        }
                    }

                    break;
                case GPUMode.OAM:
                    if (this.oldMode != this.mode)
                    {
                        this.oldMode = this.mode;

                        if ((this.stat & (1 << 5)) != 0)
                        {
                            this.gb.Cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                        }
                    }

                    // Switch to VRAM mode
                    if (this.gpuTicks >= TIME_OAM)
                    {
                        this.gpuTicks -= TIME_OAM;
                        this.mode = GPUMode.VRAM;
                    }

                    break;
                case GPUMode.VRAM:
                    if (this.oldMode != this.mode)
                    {
                        this.oldMode = this.mode;
                    }

                    // Switch to HBLANK mode
                    if (this.gpuTicks >= TIME_VRAM)
                    {
                        this.gpuTicks -= TIME_VRAM;
                        this.mode = GPUMode.HBLANK;
                    }

                    break;
            }
        }

        private static byte PackByte(int val0, int val1, int val2, int val3) => (byte)(((val0 & 3) << 0) | ((val1 & 3) << 2) | ((val2 & 3) << 4) | ((val3 & 3) << 6));

        private static int[] UnpackByte(byte value) => new int[] { (value >> 0) & 3, (value >> 2) & 3, (value >> 4) & 3, (value >> 6) & 3 };

        private void RenderScanline()
        {
            bool[] bgIsTransparent = new bool[256];

            // TODO: CGB handles BGEnabled differently

            // Tiles
            if (this.LCDCFlag(LCDC.BGEnabled) || this.renderingWindow)
            {
                // Regenerate the background cache if the tile maps have been touched since our last scanline
                if (this.BackgroundCacheDirty)
                {
                    this.CacheBackgrounds();
                }

                byte x;
                byte y;
                int offset;

                for (int i = 0; i < 160; i++)
                {
                    this.renderingWindow =
                        this.renderingWindow || (
                            this.LCDCFlag(LCDC.WindowEnable)
                            && i + WINDOW_X_OFFSET >= this.WinX
                            && this.CurrentLine >= this.startWinY
                            && this.WinX <= WINDOW_MAX_X);

                    if (this.renderingWindow)
                    {
                        x = (byte)(i + WINDOW_X_OFFSET - this.WinX);
                        y = this.currentWinY;
                        offset = !this.LCDCFlag(LCDC.WindowTileMap) ? 0x1800 : 0x1C00;
                    }
                    else
                    {
                        x = (byte)(this.ScrollX + i);
                        y = (byte)(this.ScrollY + this.CurrentLine);
                        offset = !this.LCDCFlag(LCDC.BGTileMap) ? 0x1800 : 0x1C00;
                    }

                    int framebufferLine = this.CurrentLine * 160;

                    int cachedTileNum = (offset == 0x1800 ? 0 : 1024) + ((y >> 3) * TILE_MAP_WIDTH) | (x >> 3);

                    int tileNum = this.gb.Ram.Vram[0, offset + ((y >> 3) * TILE_MAP_WIDTH | (x >> 3))];
                    if (!this.LCDCFlag(LCDC.BGWindowTileset))
                    {
                        tileNum = 256 + (sbyte)tileNum;
                    }

                    int tileAddress = ((tileNum * TILE_HEIGHT) + (y & 7)) * TILE_ROW_BYTES;
                    byte shift = (byte)(7 - (x & 7));

                    byte palIndex = (byte)((((this.gb.Ram.Vram[this.tiles[cachedTileNum].VramBank, tileAddress + 1] >> shift) & 1) << 1) | ((this.gb.Ram.Vram[this.tiles[cachedTileNum].VramBank, tileAddress] >> shift) & 1));

                    bgIsTransparent[i] = palIndex == 0;

                    if (this.gb.IsCgb)
                    {
                        this.framebuffer[framebufferLine + i] = this.tiles[cachedTileNum].MappedPalette[palIndex];
                    }
                    else
                    {
                        this.framebuffer[framebufferLine + i] = this.palBg[this.palBgMap[palIndex]];
                    }
                }
            }

            // Sprites
            if (this.LCDCFlag(LCDC.SpritesEnabled))
            {
                // Regenerate the sprite cache if OAM has been touched since our last scanline
                if (this.SpriteCacheDirty)
                {
                    this.CacheSprites();
                }

                byte spriteHeight = (byte)(this.LCDCFlag(LCDC.SpriteSize) ? 16 : 8);

                foreach (var currentSprite in this.sprites.Where(s => (byte)(this.CurrentLine - s.Y) < spriteHeight).Take(10).OrderBy(s => s.X).Reverse())
                {
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
                                tileNum += 1;
                                spriteRelY -= 8;
                            }
                        }

                        int tileAddress = ((tileNum * TILE_HEIGHT) + (spriteRelY & 7)) * TILE_ROW_BYTES;

                        byte shift = (byte)(7 - (spriteRelX & 7));

                        byte palIndex = (byte)((((this.gb.Ram.Vram[currentSprite.VramBank, tileAddress + 1] >> shift) & 1) << 1) | ((this.gb.Ram.Vram[currentSprite.VramBank, tileAddress] >> shift) & 1));

                        if (palIndex != 0 && (!currentSprite.Priority || bgIsTransparent[(byte)(currentSprite.X + i)]) && (byte)(currentSprite.X + i) < 160)
                        {
                            this.framebuffer[(this.CurrentLine * 160) + (byte)(currentSprite.X + i)] = currentSprite.MappedPalette[palIndex];
                        }
                    }
                }
            }
        }

        private void ClearFramebuffer()
        {
            if (this.gb.IsCgb)
            {
                Array.Copy(Enumerable.Repeat(FilterCGB(0xFFFFFF, gb.UseFilter), this.framebuffer.Length).ToArray(), this.framebuffer, this.framebuffer.Length);
            }
            else
            {
                Array.Copy(Enumerable.Repeat(this.palBg[0], this.framebuffer.Length).ToArray(), this.framebuffer, this.framebuffer.Length);
            }
        }

        private bool LCDCFlag(LCDC flag) => (this.control & (byte)flag) != 0;

        private struct Sprite
        {
            public byte Y;

            public byte X;

            public byte TileNum;

            public bool Priority;
            public bool YFlip; // Vertical
            public bool XFlip; // Horizontal
            public int VramBank; // TODO: Test me
            public int PaletteIndex;

            public int[] MappedPalette;
        }

        private struct Tile
        {
            public bool OAMPriority; // TODO: Implement me
            public bool YFlip; // Vertical // TODO: Implement me
            public bool XFlip; // Horizontal // TODO: Implement me
            public int VramBank; // TODO: Test me
            public int PaletteIndex;

            public int[] MappedPalette;
        }
    }
}