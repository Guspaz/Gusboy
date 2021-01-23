// #define ENABLE_TRACING

namespace Gusboy
{
    /// <summary>
    /// Main CPU emulation.
    /// </summary>
    public partial class CPU
    {
        private readonly Gameboy gb;

        private Opcode instruction;

#if ENABLE_TRACING
            private System.IO.FileStream log;
            private System.IO.StreamWriter sw;
#endif

        public CPU(Gameboy gameBoy)
        {
            this.gb = gameBoy;
            this.opcodes = this.GenerateOpcodes();
            this.extendedOpcodes = this.GenerateExtendedOpcodes();

#if ENABLE_TRACING
                log = System.IO.File.Create("trace.log");
                sw = new System.IO.StreamWriter(log);
                sw.AutoFlush = true;
#endif
        }

        public long Ticks { get; set; } = 8;

        private RAM Ram => this.gb.Ram;

        public void Tick()
        {
#if ENABLE_TRACING
                sw.Write($"A:{rA:X2} F:{(fZ ? "Z" : "-")}{(fN ? "N" : "-")}{(fH ? "H" : "-")}{(fC ? "C" : "-")} BC:{rBC:X4} DE:{rDE:x4} HL:{rHL:x4} SP:{(int)rSP:x4} M: {(int)this.gb.Gpu.mode} IME: {(fInterruptMasterEnable ? 1 : 0)} IF: {rInterruptFlags:X2} IE: {rInterruptEnable:X2} BNK: {(this.gb.Rom.mapper as MBC5).GetBank():X2} PC:{rPC:x4}");
                //sw.Write($"{ram.rom.bankNumber:X2} A:{rA:X2} F:{(fZ ? "Z" : "-")}{(fN ? "N" : "-")}{(fH ? "H" : "-")}{(fC ? "C" : "-")} BC:{rBC:X4} DE:{rDE:x4} HL:{rHL:x4} SP:{rSP:x4} PC:{rPC:x4} ");
#endif

            // if ((this.gb.Rom.mapper as MBC5).GetBank() == 0x3E && rPC == 0x5A9D)
            // {
            // }
            this.instruction = this.opcodes[this.Ram[this.rPC]];

            if (this.fHaltBug)
            {
                this.rPC--;
                this.fHaltBug = false;
            }

            this.rPC++;

            Gshort operand = 0;

            if (this.instruction.OperandLength != 0)
            {
                if (this.instruction.OperandLength == 1)
                {
                    operand = this.Ram[this.rPC];
                }
                else
                {
                    // Assume operandLength == 2
                    operand = this.Ram.GetShort(this.rPC);
                }
            }

#if ENABLE_TRACING
                sw.Write($" OP: {System.String.Format(instruction.Mnemonic, (int)operand)}\n");
#endif

            this.rPC += this.instruction.OperandLength;
            this.Ticks += this.instruction.Func(operand);
        }

        public void FakeBootstrap()
        {
            if (this.gb.IsCgb)
            {
                this.rAF = 0x1180;
                this.rBC = 0x0000;
                this.rDE = 0xFF56;
                this.rHL = 0x000D;
                this.rSP = 0xFFFE;
                this.rPC = 0x0100;
                this.fInterruptMasterEnable = false;
                this.rInterruptEnable = 0x00;
                this.rInterruptFlags = 0x01;
                this.rDIV = 0x267C; // Actually for a CGB with a DMG game, not super important.
            }
            else
            {
                this.rAF = 0x01B0;
                this.rBC = 0x0013;
                this.rDE = 0x00D8;
                this.rHL = 0x014D;
                this.rSP = 0xFFFE;
                this.rPC = 0x0100;
                this.fInterruptMasterEnable = false;
                this.rInterruptEnable = 0x00;
                this.rInterruptFlags = 0x01;
                this.rDIV = 0xABCC;
            }
        }

        public void CheckOAMBug(byte value)
        {
            // This is just an extremely rough approximation. We don't simulate the corruption values or the numbers of read/writes.
            // Value of FEXX in a 16-bit register and in mode 2 and we also don't corrupt the first two rows
            // Some of this check is redundant with the RAM check. Don't check CGB, though, RAM class will.
            if (value == 0xFE && this.gb.Gpu.CurrentOam > 2)
            {
                this.Ram[0xFE42] = 0x42;
            }
        }
    }
}