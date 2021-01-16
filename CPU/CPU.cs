//#define ENABLE_TRACING

namespace GusBoy
{
    public partial class CPU
    {
        public long ticks = 8;

        private readonly Gameboy gb;

        // Accessors
        private gRAM ram => this.gb.ram;

        public opcode instruction;

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

        public void Tick()
        {
#if ENABLE_TRACING
                sw.Write($"A:{rA:X2} F:{(fZ ? "Z" : "-")}{(fN ? "N" : "-")}{(fH ? "H" : "-")}{(fC ? "C" : "-")} BC:{rBC:X4} DE:{rDE:x4} HL:{rHL:x4} SP:{rSP:x4} PC:{rPC:x4}\n");
                //sw.Write($"{ram.rom.bankNumber:X2} A:{rA:X2} F:{(fZ ? "Z" : "-")}{(fN ? "N" : "-")}{(fH ? "H" : "-")}{(fC ? "C" : "-")} BC:{rBC:X4} DE:{rDE:x4} HL:{rHL:x4} SP:{rSP:x4} PC:{rPC:x4} ");
#endif

            this.instruction = this.opcodes[this.ram[this.rPC++]];

            gshort operand = 0;

            if ( this.instruction.operandLength != 0 )
            {
                if ( this.instruction.operandLength == 1 )
                {
                    operand = this.ram[this.rPC];
                }
                else
                {
                    // Assume operandLength == 2
                    operand = this.ram.GetShort(this.rPC);
                }
            }

#if ENABLE_TRACING
                //sw.Write($" OP: {String.Format(instruction.mnemonic, operand)}\n");
#endif

            this.rPC += this.instruction.operandLength;
            this.ticks += this.instruction.func(operand);
        }
    }
}