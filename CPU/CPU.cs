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
            private FileStream log;
            private StreamWriter sw;
#endif

        public CPU(Gameboy gameBoy)
        {
            this.gb = gameBoy;
            this.opcodes = this.GenerateOpcodes();
            this.extendedOpcodes = this.GenerateExtendedOpcodes();

#if ENABLE_TRACING
                log = File.Create("trace.log");
                sw = new StreamWriter(log);
                sw.AutoFlush = true;
#endif
        }

        public long Ticks { get; set; } = 8;

        private RAM Ram => this.gb.Ram;

        public void Tick()
        {
#if ENABLE_TRACING
                sw.Write($"A:{rA:X2} F:{(fZ ? "Z" : "-")}{(fN ? "N" : "-")}{(fH ? "H" : "-")}{(fC ? "C" : "-")} BC:{rBC:X4} DE:{rDE:x4} HL:{rHL:x4} SP:{rSP:x4} PC:{rPC:x4}\n");
                //sw.Write($"{ram.rom.bankNumber:X2} A:{rA:X2} F:{(fZ ? "Z" : "-")}{(fN ? "N" : "-")}{(fH ? "H" : "-")}{(fC ? "C" : "-")} BC:{rBC:X4} DE:{rDE:x4} HL:{rHL:x4} SP:{rSP:x4} PC:{rPC:x4} ");
#endif

            this.instruction = this.opcodes[this.Ram[this.rPC++]];

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
                //sw.Write($" OP: {String.Format(instruction.mnemonic, operand)}\n");
#endif

            this.rPC += this.instruction.OperandLength;
            this.Ticks += this.instruction.Func(operand);
        }
    }
}