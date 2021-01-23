namespace Gusboy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Main implementation.
    /// </summary>
    public partial class GPU
    {
        // private const int TIME_DMA = 648;
        // private const int TIME_DMA_DELAY = 8;
        // private const long FRAME_DURATION = (long)(10000000.0 / (4194304.0 / (TIME_VBLANK * (MAX_LINE + VBLANK_LENGTH + 1.0))));
        private const int TILE_MAP_WIDTH = 32;
        private const int TILE_HEIGHT = 8;
        private const int TILE_ROW_BYTES = 2;
        private const int WINDOW_X_OFFSET = 7;
        private const int WINDOW_MAX_X = 166;

        private readonly Gameboy gb;

        private readonly Sprite[] spriteCache = new Sprite[40];

        private readonly Tile[] tileCache = new Tile[32 * 32 * 2];
        private readonly HashSet<int> tileCacheList = new HashSet<int>();

        private readonly int[] framebuffer;
        private readonly Func<bool> drawFramebuffer;
        private int[] palBgMap = new int[4];
        private byte lcdcStat = 0;
        private GPUMode mode = GPUMode.HBLANK;
        private byte control = 0;
        private bool lcdWasOff = true; // LCD starts off
        private long gpuTicks = 0;
        private long oldCpuTicks = 0;
        private byte internalCurrentLine = 0;
        private byte startWinY;
        private byte currentWinY;
        private bool renderingWindow = false;
        private byte currentLineCompare;
        private byte hdmaControl;
        private int remainingCycles = 0;
        private int delayTicks = 0; // Some things while drawing the scanline stall the LCD clock and eats into HBLANK

        private int hdmaSource;
        private int hdmaDestination;
        private bool hdmaInProgress = false;

        public GPU(Gameboy gameBoy, Func<bool> drawFramebuffer, int[] framebuffer)
        {
            this.gb = gameBoy;

            // Select palette
            this.palObj = this.palObjMGB;
            this.palBg = this.palBgMGB;

            // Initialize sprite array
            for (int i = 0; i < this.spriteCache.Length; i++)
            {
                this.spriteCache[i] = new Sprite((byte)i, this.gb, this.palObj);
            }

            // Initialize background tile array
            for (int i = 0; i < this.tileCache.Length; i++)
            {
                this.tileCache[i] = default;
                this.tileCache[i].MappedPalette = new int[] { 0xFFFFFF, 0xFFFFFF, 0xFFFFFF, 0xFFFFFF };
            }

            this.drawFramebuffer = drawFramebuffer;

            // We might be loaded by a GBS player that cares not for the video output
            if (framebuffer == null)
            {
                framebuffer = new int[160 * 144];
            }

            this.framebuffer = framebuffer;

            // Pre-set palette alpha
            for (int i = 0; i < 4; i++)
            {
                this.palBg[i] |= 0xFF << 24;
                this.palObj[i] |= 0xFF << 24;
            }

            // Init object palette maps so they're at least not undefined
            this.PalObjMap[0] = new int[4];
            this.PalObjMap[1] = new int[4];

            this.BuildColourCache(this.gb.UseFilter);
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
            BGEnabled = 1 << 0,
            SpritesEnabled = 1 << 1,
            SpriteSize = 1 << 2, // 0 = 8x8, 1 = 8x16
            BGTileMap = 1 << 3,
            BGWindowTileset = 1 << 4,
            WindowEnable = 1 << 5,
            WindowTileMap = 1 << 6,
            LCDPower = 1 << 7,
        }

        // TODO: Replace jagged array with multi-dimensional array
        public int[][] PalObjMap { get; } = new int[4][];

        public byte CurrentOam { get; set; }

        public bool IsDmaActive { get; set; }

        public bool BackgroundCacheDirty { get; set; }

        public byte Stat
        {
            get => (byte)((this.lcdcStat & 0xFC) | (byte)this.mode);

            set => this.lcdcStat = value;
        }

        public byte CurrentLine
        {
            get => this.internalCurrentLine;

            set
            {
                this.internalCurrentLine = value;

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

        // This is supposed to be written to by the BIOS, but it isn't... So let's just link it to cgb mode for now.
        public bool OAMPriorityMode
        {
            get => this.gb.IsCgb;

            set { }
        }

        public byte HDMA1SourceHi
        {
            get
            {
                return (byte)(this.hdmaSource >> 8);
            }

            set
            {
                this.hdmaSource = (this.hdmaSource & 0b0000_0000_1111_1111) | (value << 8);
            }
        }

        public byte HDMA2SourceLo
        {
            get
            {
                return (byte)this.hdmaSource;
            }

            set
            {
                this.hdmaSource = (this.hdmaSource & 0b1111_1111_0000_0000) | value;
            }
        }

        public byte HDMA3DestHi
        {
            get
            {
                return (byte)(this.hdmaDestination >> 8);
            }

            set
            {
                this.hdmaDestination = (this.hdmaDestination & 0b0000_0000_1111_1111) | (value << 8);
            }
        }

        public byte HDMA4DestLo
        {
            get
            {
                return (byte)this.hdmaDestination;
            }

            set
            {
                this.hdmaDestination = (this.hdmaDestination & 0b1111_1111_0000_0000) | value;
            }
        }

        // CGB DMA
        public byte HDMA5Control
        {
            get => this.hdmaControl;

            set
            {
                this.hdmaControl = value;

                // We already masked the bits on write
                // TODO: Re-enable this when it's better tested and optimized, it's a dog right now.
                bool hdma = false; // (value & 0b1000_0000) != 0;

                if (this.hdmaInProgress && !hdma)
                {
                    this.hdmaInProgress = false;
                    return;
                }

                // TODO: Enforce invalid source address behaviour
                if (hdma)
                {
                    this.hdmaInProgress = true;
                    this.hdmaControl &= 0b0111_1111;
                }
                else
                {
                    int size = ((value & 0b0111_1111) + 1) * 16;
                    int transferTime = 4 + ((32 * (size / 16)) * (this.gb.Cpu.fSpeed ? 2 : 1));

                    // GDMA
                    for (int i = 0; i < size; i++)
                    {
                        this.gb.Ram[this.hdmaDestination + 0x8000 + i, isDma: true] = this.gb.Ram[this.hdmaSource + i, isDma: true];
                    }

                    this.gb.Cpu.Ticks += transferTime;

                    this.BackgroundCacheDirty = true;
                }
            }
        }

        private int TIME_HBLANK => this.gb.Cpu.fSpeed ? 400 : 200;

        private int TIME_VRAM => this.gb.Cpu.fSpeed ? 344 : 172;

        private int TIME_OAM => this.gb.Cpu.fSpeed ? 168 : 84;

        private int TIME_VBLANK => this.gb.Cpu.fSpeed ? 912 : 456; // Time per scanline, will be 10 of these

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

        public void CacheTile(int n)
        {
            if (!this.tileCacheList.Contains(n))
            {
                this.tileCacheList.Add(n);

                int baseAddress = 0x1800 | n;
                int paletteIndex = this.gb.Ram.Vram[1, baseAddress] & 0b111;

                this.tileCache[n].OAMPriority = (this.gb.Ram.Vram[1, baseAddress] & (1 << 7)) != 0;
                this.tileCache[n].YFlip = (this.gb.Ram.Vram[1, baseAddress] & (1 << 6)) != 0;
                this.tileCache[n].XFlip = (this.gb.Ram.Vram[1, baseAddress] & (1 << 5)) != 0;
                this.tileCache[n].VramBank = (this.gb.Ram.Vram[1, baseAddress] >> 3) & 1;
                this.tileCache[n].PaletteIndex = paletteIndex;

                // DMG doesn't use this cached palette
                this.tileCache[n].MappedPalette[0] = this.ColourCache[this.PalCgbBackground[paletteIndex, 0]];
                this.tileCache[n].MappedPalette[1] = this.ColourCache[this.PalCgbBackground[paletteIndex, 1]];
                this.tileCache[n].MappedPalette[2] = this.ColourCache[this.PalCgbBackground[paletteIndex, 2]];
                this.tileCache[n].MappedPalette[3] = this.ColourCache[this.PalCgbBackground[paletteIndex, 3]];
            }
        }

        public void CheckLYCInterrupt()
        {
            if (this.CurrentLine == this.rLYC)
            {
                // Set match bit
                this.lcdcStat |= 1 << 2;

                // LYC = LY status interrupt
                if ((this.lcdcStat & (1 << 6)) != 0)
                {
                    this.gb.Cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                }
            }
            else
            {
                // Clear match bit
                this.lcdcStat &= 0xFB;
            }
        }

        public byte GetLCDC() => this.control;

        public void SetLCDC(byte value)
        {
            this.control = value;

            // https://www.reddit.com/r/Gameboy/comments/a1c8h0/what_happens_when_a_gameboy_screen_is_disabled/eap4f8c/
            if (!this.LCDCFlag(LCDC.LCDPower))
            {
                this.internalCurrentLine = 0; // Reset rLY to 0 but bypass the coincidence check
                this.gpuTicks = 0; // Reset GPU clock
                this.mode = GPUMode.HBLANK; // Reset mode to 0

                // Clear framebuffer
                this.ClearFramebuffer();
                this.drawFramebuffer();

                // Used to skip the first frame after re-enabling it
                this.lcdWasOff = true;
            }
            else if (this.lcdWasOff)
            {
                // Turning on the screen sets the LY=LYC coincidence bit in BGB, but doesn't seem to fire an interrupt?
                if (this.CurrentLine == this.rLYC)
                {
                    // Set match bit
                    this.lcdcStat |= 0b0000_0100;
                }
                else
                {
                    // Clear match bit
                    this.lcdcStat &= 0xFB;
                }
            }
        }

        // Always access when LCD power is off, but that forces mode 0 anyway. DMA period blocks access but the memory mapper handles that globally.
        // DMA itself gets to bypass the restriction, I think?
        // Disabled for now because my timing isn't accurate enough to enforce this properly.
        public bool CanAccessOAM(bool isDMA) => this.mode == GPUMode.HBLANK || this.mode == GPUMode.VBLANK || isDMA;

        // Always access when LCD power is off, but that forces mode 0 anyway. DMA period blocks access but the memory mapper handles that globally.
        // DMA itself gets to bypass the restriction, I think?
        // Disabled for now because my timing isn't accurate enough to enforce this properly.
        public bool CanAccessVRAM(bool isDMA) => this.mode == GPUMode.HBLANK || this.mode == GPUMode.VBLANK || this.mode == GPUMode.OAM || isDMA;

        // This is just the same as the VRAM restriction for now.
        public bool CanAccessCGBPal(bool isDMA) => this.CanAccessVRAM(isDMA);

        public void Tick()
        {
            if (!this.LCDCFlag(LCDC.LCDPower))
            {
                this.oldCpuTicks = this.gb.Cpu.Ticks;
                return;
            }

            this.gpuTicks = (this.gb.Cpu.Ticks - this.oldCpuTicks) >> 2;
            this.oldCpuTicks = this.gb.Cpu.Ticks;

            for (int i = 0; i < this.gpuTicks; i++)
            {
                this.remainingCycles -= 4;

                if (this.remainingCycles <= 0)
                {
                    // Mode change required
                    if (this.mode == GPUMode.HBLANK)
                    {
                        // We're in HBLANK, next is either OAM or VBLANK
                        if (this.CurrentLine == 143)
                        {
                            // This was the last line of the screen, so enter VBLANK
                            this.mode = GPUMode.VBLANK;
                            this.remainingCycles = this.TIME_VBLANK;

                            // VBLANK gets its own dedicated interrupt on top of the flag in STAT
                            this.gb.Cpu.TriggerInterrupt(CPU.INT_VBLANK);

                            if (this.CheckModeFlag(GPUMode.VBLANK))
                            {
                                this.gb.Cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                            }

                            // The first frame after turning the LCD on is skipped (some games show garbage on this frame, real hardware doesn't draw it)
                            if (this.lcdWasOff)
                            {
                                // Draw blank frame
                                this.ClearFramebuffer();
                                this.lcdWasOff = false;
                            }

                            this.drawFramebuffer();

                            // Increment LY at the end of HBLANK
                            this.CurrentLine++;

                            // Reset window
                            this.renderingWindow = false;
                            this.startWinY = this.WinY;
                            this.currentWinY = 0;
                        }
                        else
                        {
                            // Not the last line, go to OAM on the next one
                            this.mode = GPUMode.OAM;
                            this.remainingCycles = this.TIME_OAM;

                            if (this.CheckModeFlag(GPUMode.OAM))
                            {
                                this.gb.Cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                            }

                            // Increment LY at the end of HBLANK
                            this.CurrentLine++;

                            // Increment window Y at the end of HBLANK
                            if (this.renderingWindow)
                            {
                                this.currentWinY++;
                            }
                        }
                    }
                    else if (this.mode == GPUMode.VBLANK)
                    {
                        // We're in VBLANK, next is either OAM or more VBLANK
                        if (this.CurrentLine == 152)
                        {
                            // The LY counter only spends 4 cycles set to 153
                            this.remainingCycles = 4;

                            this.CurrentLine++;
                        }
                        else if (this.CurrentLine == 153)
                        {
                            // We spend the rest of line 153 with LY=0
                            this.remainingCycles = this.TIME_VBLANK - 4;
                            this.CurrentLine = 0;
                        }
                        else if (this.CurrentLine == 0)
                        {
                            // End of the line, top of the screen OAM
                            this.mode = GPUMode.OAM;
                            this.remainingCycles = this.TIME_OAM;

                            if (this.CheckModeFlag(GPUMode.OAM))
                            {
                                this.gb.Cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                            }
                        }
                        else
                        {
                            // There's yet more VBLANK to come, keep going
                            this.remainingCycles = this.TIME_VBLANK;

                            // Increment LY at the end of VBLANK
                            this.CurrentLine++;
                        }
                    }
                    else if (this.mode == GPUMode.OAM)
                    {
                        // We're in OAM, next is VRAM
                        this.mode = GPUMode.VRAM;
                        this.remainingCycles = this.TIME_VRAM;

                        // In a real system this would start drawing bit-by-bit here, but we're just going to do it all in one shot up front.
                        this.RenderScanline();

                        // Extend VRAM cycle based on what we did rendering the scanline. We'll subtract it from HBLANK.
                        // TODO: Validate these exact timings somehow, pan docs isn't precise exacty
                        this.remainingCycles += this.delayTicks * (this.gb.Cpu.fSpeed ? 2 : 1);
                    }
                    else
                    {
                        // We're in VRAM, next is HBLANK
                        this.mode = GPUMode.HBLANK;
                        this.remainingCycles = this.TIME_HBLANK;

                        // We extended VBLANK to do rendering so we must steal from HBLANK.
                        this.remainingCycles -= this.delayTicks * (this.gb.Cpu.fSpeed ? 2 : 1);
                        this.delayTicks = 0;

                        // TODO: We should also be able to start this if it started during hblank.
                        if (this.hdmaInProgress)
                        {
                            this.HdmaTick();
                        }

                        if (this.CheckModeFlag(GPUMode.HBLANK))
                        {
                            this.gb.Cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                        }
                    }
                }

                if (this.mode == GPUMode.OAM)
                {
                    this.CurrentOam++;
                }
                else
                {
                    this.CurrentOam = 0;
                }
            }
        }

        private void RenderScanline()
        {
            bool[] bgIsTransparent = new bool[256];
            bool[] bgPriority = new bool[256];

            // TODO: CGB handles BGEnabled differently

            // Tiles
            if (this.gb.IsCgb || this.LCDCFlag(LCDC.BGEnabled) || this.renderingWindow)
            {
                byte x;
                byte y;
                int tileDataAddress;
                LCDC tilemapFlag;
                int tileNum;

                // Invalidate the tile cache if it's dirty
                if (this.BackgroundCacheDirty)
                {
                    this.tileCacheList.Clear();
                    this.BackgroundCacheDirty = false;
                }

                // Delay HBLANK for the background scroll
                // TODO: Should this happen if we're in the window?
                this.delayTicks += this.ScrollX % 8;
                bool windowDelay = false;

                for (int i = 0; i < 160; i++)
                {
                    this.renderingWindow =
                            this.LCDCFlag(LCDC.WindowEnable)
                            && i + WINDOW_X_OFFSET >= this.WinX
                            && this.CurrentLine >= this.startWinY
                            && this.WinX <= WINDOW_MAX_X;

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
                        this.framebuffer[(this.CurrentLine * 160) + i] = this.tileCache[tileNum].MappedPalette[palIndex];
                    }
                    else
                    {
                        this.framebuffer[(this.CurrentLine * 160) + i] = this.palBg[this.palBgMap[palIndex]];
                    }
                }
            }

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
                    if (currentSprite.X >= this.WinX)
                    {
                        this.delayTicks += 11 - Math.Min(5, (currentSprite.X + (255 - this.WinX)) % 8);
                    }
                    else
                    {
                        this.delayTicks += 11 - Math.Min(5, (currentSprite.X + this.ScrollX) % 8);
                    }

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
                            && (!currentSprite.Priority || bgIsTransparent[finalX])
                            && (!bgPriority[finalX] || bgIsTransparent[finalX]))
                        {
                            this.framebuffer[(this.CurrentLine * 160) + (byte)(currentSprite.X + i)] = currentSprite.MappedPalette[palIndex];
                        }
                    }
                }
            }
        }

        private void HdmaTick()
        {
            int size = ((this.hdmaControl & 0b0111_1111) + 1) * 16;
            int transferTime = 4 + (32 * (this.gb.Cpu.fSpeed ? 2 : 1));

            this.BackgroundCacheDirty = true;

            // Transfer 0x10 bytes and increment stuff
            if (size > 0)
            {
                for (int i = 0; i < 0x10; i++)
                {
                    this.gb.Ram[this.hdmaDestination + 0x8000 + i, isDma: true] = this.gb.Ram[this.hdmaSource + i, isDma: true];
                }

                this.gb.Cpu.Ticks += transferTime;
            }

            size -= 0x10;

            // TODO: Handle overflow of destination
            // TODO: Validate that this is the actual end of the transfer? We're not off-by-one?
            if (size == 0)
            {
                this.hdmaInProgress = false;
            }
            else
            {
                this.hdmaSource += 0x10;
                this.hdmaDestination += 0x10;
                this.hdmaControl = (byte)((size - 1) / 16);
            }
        }

        private void ClearFramebuffer()
        {
            if (this.gb.IsCgb)
            {
                Array.Fill(this.framebuffer, this.ColourCache[0x7FFF]);
            }
            else
            {
                Array.Fill(this.framebuffer, this.palBg[0]);
            }
        }

        private bool CheckModeFlag(GPUMode mode)
        {
            return ((this.lcdcStat & 0b0000_1000) != 0 && mode == GPUMode.HBLANK)
                || ((this.lcdcStat & 0b0001_0000) != 0 && mode == GPUMode.VBLANK)
                || ((this.lcdcStat & 0b0010_0000) != 0 && mode == GPUMode.OAM);
        }

        private bool LCDCFlag(LCDC flag) => (this.control & (byte)flag) != 0;

        private struct Tile
        {
            public bool OAMPriority;
            public bool YFlip; // Vertical
            public bool XFlip; // Horizontal
            public int VramBank;
            public int PaletteIndex;

            public int[] MappedPalette;
        }
    }
}