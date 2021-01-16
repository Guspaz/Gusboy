//#define ENABLE_TRACING

namespace GusBoy
{
    public partial class CPU
    {
        public long ticks = 8;

        private Gameboy gb;

        // Accessors
        private gRAM ram => gb.ram;

        public opcode instruction;

        #if ENABLE_TRACING
            private FileStream log;
            private StreamWriter sw;
        #endif

        public CPU(Gameboy gameBoy)
        {
            this.gb = gameBoy;
            opcodes = GenerateOpcodes();
            extendedOpcodes = GenerateExtendedOpcodes();
           
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

            instruction = opcodes[ram[rPC++]];

            gshort operand = 0;

            if (instruction.operandLength != 0)
            {
                if (instruction.operandLength == 1)
                {
                    operand = ram[rPC];
                }
                else
                {
                    // Assume operandLength == 2
                    operand = ram.GetShort(rPC);
                }
            }

            #if ENABLE_TRACING
                //sw.Write($" OP: {String.Format(instruction.mnemonic, operand)}\n");
            #endif

            rPC += instruction.operandLength;            
            ticks += instruction.func(operand);
        }
    }
}