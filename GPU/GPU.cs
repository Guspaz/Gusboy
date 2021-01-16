using System;
using System.Linq;

namespace GusBoy
{
    public class GPU
    {
        const int TIME_HBLANK = 204;
        const int TIME_OAM = 80;
        const int TIME_VRAM = 172;
        const int TIME_VBLANK = TIME_OAM + TIME_VRAM + TIME_HBLANK; // Time per scanline, vblank will be 10 of these
        const int MAX_LINE = 143;
        const int VBLANK_LENGTH = 10; // Number of scanlines
        const int TIME_DMA = 648;
        const int TIME_DMA_DELAY = 8;

        const int TILE_MAP_WIDTH = 32;
        const int TILE_HEIGHT = 8;
        const int TILE_ROW_BYTES = 2;
        const int WINDOW_X_OFFSET = 7;
        const int WINDOW_MAX_X = 166;

        // Default
        //private int[] palBG = { 0xE0F8D0, 0x88C070, 0x346856, 0x08181A };
        //private int[] palOBJ = { 0xE0F8D0, 0x88C070, 0x346856, 0x08181A };

        // DMG
        //private int[] palBG = { 0x7b8210, 0x5a7942, 0x39594a, 0x294139 };
        //private int[] palOBJ = { 0x7b8210, 0x5a7942, 0x39594a, 0x294139 };

        // MGB
        private int[] palBG = { 0xc6cba5, 0x8c926b, 0x4a5139, 0x181818 };
        private int[] palOBJ = { 0xc6cba5, 0x8c926b, 0x4a5139, 0x181818 };

        // GBL
        //private int[] palBG = { 0x00b284, 0x009a73, 0x00694a, 0x005139 };
        //private int[] palOBJ = { 0x00b284, 0x009a73, 0x00694a, 0x005139 };


        //private int[] palOBJ = { 0xF8E0D0, 0xC08870, 0x683456, 0x18081A }; // red
        //private int[] palBG = { 0xFFFFFF, 0xAAAAAA, 0x555555, 0x000000 }; // grey
        //private int[] palOBJ = { 0xFFFFFF, 0xAAAAAA, 0x555555, 0x000000 }; // grey

        private const bool FRAME_LIMITER = false;
        private TimeSpan FRAME_DURATION = new TimeSpan((long)(10000000.0 / (4194304.0 / (TIME_VBLANK * (MAX_LINE + VBLANK_LENGTH + 1.0)))));
        private System.Diagnostics.Stopwatch frameLimiterClock = new System.Diagnostics.Stopwatch();

        // Colour transform for 5bpc GGB to 8bpc VGA
        // From https://byuu.net/video/color-emulation/
        int FilterCGB(int color)
        {
            return color;

            int r = (color & 0xFF0000) >> 16;
            int g = (color & 0x00FF00) >> 8;
            int b = (color & 0x0000FF);

            // 8bpc -> 5bpc for previewing
            //r >>= 3; g >>= 3; b >>= 3;

            int R = Math.Min(960, (r * 26 + g * 4 + b * 2)) >> 2;
            int G = Math.Min(960, (g * 24 + b * 8)) >> 2;
            int B = Math.Min(960, (r * 6 + g * 4 + b * 22)) >> 2;

            return (0xFF << 24) | (R << 16) | (G << 8) | (B);
        }

        public enum GPUMode
        {
            HBLANK = 0,
            VBLANK = 1,
            OAM = 2,
            VRAM = 3
        };

        [Flags]
        private enum LCDC
        {
            SpritesEnabled = (1 << 1),
            BGEnabled = (1 << 0),
            SpriteSize = (1 << 2), // 0 = 8x8, 1 = 8x16
            BGTileMap = (1 << 3),
            BGWindowTileset = (1 << 4),
            WindowEnable = (1 << 5),
            WindowTileMap = (1 << 6),
            LCDPower = (1 << 7)
        }

        private struct Sprite
        {
            public byte y;
            public byte x;
            public byte tileNum;
            public bool priority;
            public bool yFlip;
            public bool xFlip;
            public int palette;
        }

        private GPUMode mode = GPUMode.HBLANK;
        private GPUMode oldMode = GPUMode.HBLANK;
        private byte _stat = 0;
        public bool IsDmaActive = false;

        private int[] palBgMap = new int[4];
        private int[][] palObjMap = new int[4][];
        private long gpuTicks = 0;
        private long oldCpuTicks = 0;
        
        private Gameboy gb;
        
        // Accessors
        private gRAM ram => gb.ram;
        private CPU cpu => gb.cpu;
        
        private byte _currentLine = 0;
        private Sprite[] sprites = new Sprite[40];
        public bool spriteCacheDirty = false;
        private int[] framebuffer;
        private Func<bool> drawFramebuffer;
        private bool renderingWindow = false;
                
        public byte stat
        {
            get => (byte)((_stat & 0xFC) | (byte)mode);

            set =>_stat = value;
        }

        public byte bgPal
        {
            get => PackByte(palBgMap[0], palBgMap[1], palBgMap[2], palBgMap[3]);

            set => palBgMap = UnpackByte(value);
        }

        public byte objPal0
        {
            get => PackByte(palObjMap[0][0], palObjMap[0][1], palObjMap[0][2], palObjMap[0][3]);

            set => palObjMap[0] = UnpackByte(value);
        }

        public byte objPal1
        {
            get => PackByte(palObjMap[1][0], palObjMap[1][1], palObjMap[1][2], palObjMap[1][3]);

            set => palObjMap[1] = UnpackByte(value);
        }

        public byte currentLine
        {
            get => _currentLine;
            
            set
            {
                if (value == rLYC)
                {
                    // Set match bit
                    _stat |= (1 << 2);

                    // LYC = LY status interrupt
                    if ((_stat & (1 << 6)) != 0)
                        gb.cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                }
                else
                {
                    // Clear match bit
                    _stat &= 0xFB;
                }

                _currentLine = value;
            }
        }

        private byte control = 0;
        private bool lcdWasOff = true; // LCD starts off
        public byte scrollX = 0;
        public byte scrollY = 0;
        public byte rLYC = 0;
        public byte winX = 0;
        public byte winY = 0;
        public byte startWinY = 0;
        public byte currentWinY = 0;

        public GPU(Gameboy gameBoy, Func<bool> drawFramebuffer, int[] framebuffer)
        {
            this.gb = gameBoy;

            // Initialize sprite array
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = new Sprite();
            }

            this.drawFramebuffer = drawFramebuffer;
            this.framebuffer = framebuffer;

            // Pre-set palette alpha
            for (int i = 0; i < 4; i++)
            {
                palBG[i] |= 0xFF << 24;
                palOBJ[i] |= 0xFF << 24;
            }

            // Init object palette maps so they're at least not undefined
            palObjMap[0] = new int[4];
            palObjMap[1] = new int[4];

            if (FRAME_LIMITER)
            {
                frameLimiterClock.Start();
            }
        }

        private void RenderScanline()
        {
            bool[] bgIsTransparent = new bool[256];

            // Tiles
            if (LCDCFlag(LCDC.BGEnabled) || renderingWindow)
            {
                byte x;
                byte y;
                int offset;
                
                for (int i = 0; i < 160; i++)
                {
                    renderingWindow =
                        renderingWindow || (
                            LCDCFlag(LCDC.WindowEnable)
                            && i + WINDOW_X_OFFSET >= winX
                            && currentLine >= startWinY
                            && winX <= WINDOW_MAX_X
                        );

                    if (renderingWindow)
                    {
                        x = (byte)(i + WINDOW_X_OFFSET - winX);
                        y = (byte)(currentWinY);
                        offset = !LCDCFlag(LCDC.WindowTileMap) ? 0x1800 : 0x1C00;
                    }
                    else
                    {
                        x = (byte)(scrollX + i);
                        y = (byte)(scrollY + currentLine);
                        offset = !LCDCFlag(LCDC.BGTileMap) ? 0x1800 : 0x1C00;
                    }

                    int framebufferLine = currentLine * 160;

                    int tileNum = ram.vram[offset + ((y >> 3) * TILE_MAP_WIDTH | (x >> 3))];
                    if (!LCDCFlag(LCDC.BGWindowTileset)) tileNum = 256 + (sbyte)tileNum;

                    var tileAddress = (tileNum * TILE_HEIGHT + (y & 7)) * TILE_ROW_BYTES;
                    byte shift = (byte)(7 - (x & 7));
                    byte palIndex = (byte)((((ram.vram[tileAddress + 1] >> shift) & 1) << 1) | ((ram.vram[tileAddress] >> shift) & 1));

                    bgIsTransparent[i] = palIndex == 0;

                    framebuffer[framebufferLine + i] = palBG[palBgMap[palIndex]];
                }
            }

            // Regenerate the sprite cache if OAM has been touched since our last scanline
            if (spriteCacheDirty)
            {
                CacheSprites();
            }

            // Sprites
            if (LCDCFlag(LCDC.SpritesEnabled))
            {
                byte spriteHeight = (byte)(LCDCFlag(LCDC.SpriteSize) ? 16 : 8);

                foreach (var currentSprite in sprites.Where(s => (byte)(currentLine - s.y) < spriteHeight).Take(10).OrderBy(s => s.x).Reverse())
                {
                    // Sprite appears on this scanline, draw it.
                    for (int i = 0; i < 8; i++)
                    {
                        byte spriteRelY = (byte)(currentLine - currentSprite.y);
                        byte spriteRelX = (byte)i;

                        if (currentSprite.xFlip)
                            spriteRelX = (byte)((spriteRelX - 7) * -1);

                        if (currentSprite.yFlip)
                            spriteRelY = (byte)(spriteHeight - 1 - spriteRelY);

                        var tileNum = currentSprite.tileNum;
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

                        var tileAddress = (tileNum * TILE_HEIGHT + (spriteRelY & 7)) * TILE_ROW_BYTES;

                        byte shift = (byte)(7 - (spriteRelX & 7));

                        byte palIndex = (byte)((((ram.vram[tileAddress + 1] >> shift) & 1) << 1) | ((ram.vram[tileAddress] >> shift) & 1));

                        if (palIndex != 0 && (!currentSprite.priority || bgIsTransparent[(byte)(currentSprite.x + i)]) && (byte)(currentSprite.x + i) < 160)
                            framebuffer[(currentLine * 160) + (byte)(currentSprite.x + i)] = palOBJ[palObjMap[currentSprite.palette][palIndex]];
                    }
                }
            }
        }

        public void CacheSprites()
        {
            for (int n = 0; n < sprites.Length; n++)
            {
                sprites[n].y = (byte)(ram[0xFE00 + (n << 2) + 0] - 16); // Pre-offset the location
                sprites[n].x = (byte)(ram[0xFE00 + (n << 2) + 1] - 8); // Pre-offset the location
                sprites[n].tileNum = ram[0xFE00 + (n << 2) + 2];
                sprites[n].priority = (ram[0xFE00 + (n << 2) + 3] & (1 << 7)) != 0;
                sprites[n].yFlip = (ram[0xFE00 + (n << 2) + 3] & (1 << 6)) != 0;
                sprites[n].xFlip = (ram[0xFE00 + (n << 2) + 3] & (1 << 5)) != 0;
                sprites[n].palette = (ram[0xFE00 + (n << 2) + 3] & (1 << 4)) != 0 ? 1 : 0;
            }

            spriteCacheDirty = false;
        }

        public byte GetLCDC() => control;

        public void SetLCDC(byte value)
        {
            control = value;

            // https://www.reddit.com/r/Gameboy/comments/a1c8h0/what_happens_when_a_gameboy_screen_is_disabled/eap4f8c/
            if (!LCDCFlag(LCDC.LCDPower))
            {
                _currentLine = 0; // Reset rLY to 0
                gpuTicks = 0; // Reset GPU clock
                mode = GPUMode.HBLANK; // Reset mode to 0                

                // Clear framebuffer
                if (!LCDCFlag(LCDC.LCDPower))
                {
                    ClearFramebuffer();
                    drawFramebuffer();

                    // Used to skip the first frame after re-enabling it
                    lcdWasOff = true;
                }

            }
        }

        private void ClearFramebuffer()
        {
            // This should be palBG[0] but for debugs, do white
            //Array.Copy(Enumerable.Repeat(0xFFFFFF, framebuffer.Length).ToArray(), framebuffer, framebuffer.Length);
            Array.Copy(Enumerable.Repeat(palBG[0], framebuffer.Length).ToArray(), framebuffer, framebuffer.Length);
        }

        public bool CanAccessOAM(bool isDma)
        {
            // Always access when LCD power is off, but that forces mode 0 anyway. DMA period blocks access but the memory mapper handles that globally.
            // DMA itself gets to bypass the restriction, I think?
            return mode == GPUMode.HBLANK || mode == GPUMode.VBLANK || isDma;
        }

        public bool CanAccessVRAM(bool isDma)
        {
            // Always access when LCD power is off, but that forces mode 0 anyway. DMA periodblocks access but the memory mapper handles that globally.
            // DMA itself gets to bypass the restriction, I think?
            return mode == GPUMode.HBLANK || mode == GPUMode.VBLANK || mode == GPUMode.OAM || isDma;
        }

        public void TriggerDMA(byte address)
        {
            IsDmaActive = true;

            int dmaAddress = address << 8;

            // In a real DMG this should happen bit by bit during normal execution, not all in one chunk
            for ( int i = 0; i < 0xA0; i++)
            {
                ram[0xFE00 + i, isDma: true] = ram[dmaAddress + i, isDma: true];
            }

            // TODO: Ideally we should keep this set to true until a timer elapses for how long DMA is supposed to take, so that memory access can be restricted to HRAM (FF80-FFFE) during that period.
            IsDmaActive = false;
        }

        public void Tick()
        {
            if (!LCDCFlag(LCDC.LCDPower))
            {
                oldCpuTicks = cpu.ticks;
                return;
            }

            gpuTicks += (cpu.ticks - oldCpuTicks);
            oldCpuTicks = cpu.ticks;

            switch (mode)
            {
                case GPUMode.HBLANK:
                    if (oldMode != mode)
                    {
                        oldMode = mode;

                        if ((_stat & (1 << 3)) != 0)
                            gb.cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                    }

                    // Switch to VBLANK mode
                    if (gpuTicks >= TIME_HBLANK)
                    {
                        gpuTicks -= TIME_HBLANK;

                        RenderScanline();
                        currentLine++;
                        if (renderingWindow) currentWinY++;

                        if (currentLine == MAX_LINE+1)
                        {
                            mode = GPUMode.VBLANK;

                            // The first frame after turning the LCD on is skipped (some games show garbage on this frame, real hardware doesn't draw it)
                            if (lcdWasOff)
                            {
                                // Draw blank frame
                                ClearFramebuffer();
                                lcdWasOff = false;
                            }

                            drawFramebuffer();

                            //Sleepwait
                            if (FRAME_LIMITER)
                            {
                                while (frameLimiterClock.Elapsed < FRAME_DURATION)
                                {
                                }

                                frameLimiterClock.Restart();
                            }
                        }
                        else
                        {
                            mode = GPUMode.OAM;
                        }
                    }

                    break;
                case GPUMode.VBLANK:

                    if (oldMode != mode)
                    {
                        oldMode = mode;

                        gb.cpu.TriggerInterrupt(CPU.INT_VBLANK);

                        if ((_stat & (1 << 4)) != 0)
                        {
                            gb.cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                        }
                    }

                    // Switch to OAM mode
                    if (gpuTicks >= TIME_VBLANK)
                    {
                        gpuTicks -= TIME_VBLANK;
                        currentLine++;
                        if (renderingWindow) currentWinY++;

                        if (currentLine > MAX_LINE + VBLANK_LENGTH+1)
                        {
                            renderingWindow = false;

                            mode = GPUMode.OAM;
                            currentLine = 0;
                            startWinY = winY;
                            currentWinY = 0;
                        }
                    }

                    break;
                case GPUMode.OAM:
                    if (oldMode != mode)
                    {
                        oldMode = mode;

                        if ((_stat & (1 << 5)) != 0)
                            gb.cpu.TriggerInterrupt(CPU.INT_LCDSTAT);
                    }

                    // Switch to VRAM mode
                    if (gpuTicks >= TIME_OAM)
                    {
                        gpuTicks -= TIME_OAM;
                        mode = GPUMode.VRAM;
                    }

                    break;
                case GPUMode.VRAM:
                    if (oldMode != mode)
                    {
                        oldMode = mode;
                    }

                    // Switch to HBLANK mode
                    if (gpuTicks >= TIME_VRAM)
                    {
                        gpuTicks -= TIME_VRAM;
                        mode = GPUMode.HBLANK;
                    }

                    break;
            }
        }

        private bool LCDCFlag(LCDC flag)
        {
            return (control & (byte)flag) != 0;
        }

        private byte PackByte(int val0, int val1, int val2, int val3)
        {
            return (byte)(((val0 & 3) << 0) | ((val1 & 3) << 2) | ((val2 & 3) << 4) | ((val3 & 3) << 6));
        }

        private int[] UnpackByte(byte value)
        {
            return new int[] {
                (value >> 0) & 3,
                (value >> 2) & 3,
                (value >> 4) & 3,
                (value >> 6) & 3
            };
        }
    }
}