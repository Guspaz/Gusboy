﻿using System;

namespace GusBoy
{
    public partial class CPU
    {
        public struct Opcode
        {
            public string mnemonic;
            public int operandLength;
            public Func<Gshort, int> func;

            public Opcode(string name, int paramLength, Func<Gshort, int> func)
            {
                this.mnemonic = name;
                this.operandLength = paramLength;
                this.func = func;
            }
        }

        public Opcode[] opcodes;

        private Opcode[] GenerateOpcodes() => new Opcode[]
            {
                /* 0x00 */ new Opcode("NOP", 0, this.nop),
                /* 0x01 */ new Opcode("LD BC, 0x{0:X4}", 2, this.ld_bc_nn),
                /* 0x02 */ new Opcode("LD (BC), A", 0, this.ld_pbc_a),
                /* 0x03 */ new Opcode("INC BC", 0, this.inc_bc),
                /* 0x04 */ new Opcode("INC B", 0, this.inc_b),
                /* 0x05 */ new Opcode("DEC B", 0, this.dec_b),
                /* 0x06 */ new Opcode("LD B, 0x{0:X2}", 1, this.ld_b_n),
                /* 0x07 */ new Opcode("RLC A", 0, this.rlc_a),
                /* 0x08 */ new Opcode("LD (0x{0:X4}), SP", 2, this.ld_pnn_sp),
                /* 0x09 */ new Opcode("ADD HL, BC", 0, this.add_hl_bc),
                /* 0x0A */ new Opcode("LD A, (BC)", 0, this.ld_a_pbc),
                /* 0x0B */ new Opcode("DEC BC", 0, this.dec_bc),
                /* 0x0C */ new Opcode("INC C", 0, this.inc_c),
                /* 0x0D */ new Opcode("DEC C", 0, this.dec_c),
                /* 0x0E */ new Opcode("LD C, 0x{0:X2}", 1, this.ld_c_n),
                /* 0x0F */ new Opcode("RRC A", 0, this.rrc_a),

                /* 0x10 */ new Opcode("STOP", 1, this.stop),
                /* 0x11 */ new Opcode("LD DE, 0x{0:X4}", 2, this.ld_de_nn),
                /* 0x12 */ new Opcode("LD (DE), A", 0, this.ld_pde_a),
                /* 0x13 */ new Opcode("INC DE", 0, this.inc_de),
                /* 0x14 */ new Opcode("INC D", 0, this.inc_d),
                /* 0x15 */ new Opcode("DEC D", 0, this.dec_d),
                /* 0x16 */ new Opcode("LD D, 0x{0:X2}", 1, this.ld_d_n),
                /* 0x17 */ new Opcode("RL A", 0, this.rl_a),
                /* 0x18 */ new Opcode("JR d0x{0:X2}", 1, this.jr_dn),
                /* 0x19 */ new Opcode("ADD HL, DE", 0, this.add_hl_de),
                /* 0x1A */ new Opcode("LD A, (DE)", 0, this.ld_a_pde),
                /* 0x1B */ new Opcode("DEC DE", 0, this.dec_de),
                /* 0x1C */ new Opcode("INC E", 0, this.inc_e),
                /* 0x1D */ new Opcode("DEC E", 0, this.dec_e),
                /* 0x1E */ new Opcode("LD E, 0x{0:X2}", 1, this.ld_e_n),
                /* 0x1F */ new Opcode("RR A", 0, this.rr_a),

                /* 0x20 */ new Opcode("JR NZ, d0x{0:X2}", 1, this.jr_nz_dn),
                /* 0x21 */ new Opcode("LD HL, 0x{0:X4}", 2, this.ld_hl_nn),
                /* 0x22 */ new Opcode("LDI (HL), A", 0, this.ldi_phl_a),
                /* 0x23 */ new Opcode("INC HL", 0, this.inc_hl),
                /* 0x24 */ new Opcode("INC H", 0, this.inc_h),
                /* 0x25 */ new Opcode("DEC H", 0, this.dec_h),
                /* 0x26 */ new Opcode("LD H, 0x{0:X2}", 1, this.ld_h_n),
                /* 0x27 */ new Opcode("DAA", 0, this.daa),
                /* 0x28 */ new Opcode("JR Z, d0x{0:X2}", 1, this.jr_z_dn),
                /* 0x29 */ new Opcode("ADD HL, HL", 0, this.add_hl_hl),
                /* 0x2A */ new Opcode("LDI A, (HL)", 0, this.ldi_a_phl),
                /* 0x2B */ new Opcode("DEC HL", 0, this.dec_hl),
                /* 0x2C */ new Opcode("INC L", 0, this.inc_l),
                /* 0x2D */ new Opcode("DEC L", 0, this.dec_l),
                /* 0x2E */ new Opcode("LD L, 0x{0:X2}", 1, this.ld_l_n),
                /* 0x2F */ new Opcode("CPL", 0, this.cpl),

                /* 0x30 */ new Opcode("JR NC, d0x{0:X2}", 1, this.jr_nc_dn),
                /* 0x31 */ new Opcode("LD SP, 0x{0:X4}", 2, this.ld_sp_nn),
                /* 0x32 */ new Opcode("LDD (HL), A", 0, this.ldd_phl_a),
                /* 0x33 */ new Opcode("INC SP", 0, this.inc_sp),
                /* 0x34 */ new Opcode("INC (HL)", 0, this.inc_phl),
                /* 0x35 */ new Opcode("DEC (HL)", 0, this.dec_phl),
                /* 0x36 */ new Opcode("LD (HL), 0x{0:X2}", 1, this.ld_phl_n),
                /* 0x37 */ new Opcode("SCF", 0, this.scf),
                /* 0x38 */ new Opcode("JR C, d0x{0:X2}", 1, this.jr_c_dn),
                /* 0x39 */ new Opcode("ADD HL, SP", 0, this.add_hl_sp),
                /* 0x3A */ new Opcode("LDD A, (HL)", 0, this.ldd_a_phl),
                /* 0x3B */ new Opcode("DEC SP", 0, this.dec_sp),
                /* 0x3C */ new Opcode("INC A", 0, this.inc_a),
                /* 0x3D */ new Opcode("DEC A", 0, this.dec_a),
                /* 0x3E */ new Opcode("LD A, 0x{0:X2}", 1, this.ld_a_n),
                /* 0x3F */ new Opcode("CCF", 0, this.ccf),

                /* 0x40 */ new Opcode("LD B, B", 0, this.nop), // Equivalent to nop
                /* 0x41 */ new Opcode("LD B, C", 0, this.ld_b_c),
                /* 0x42 */ new Opcode("LD B, D", 0, this.ld_b_d),
                /* 0x43 */ new Opcode("LD B, E", 0, this.ld_b_e),
                /* 0x44 */ new Opcode("LD B, H", 0, this.ld_b_h),
                /* 0x45 */ new Opcode("LD B, L", 0, this.ld_b_l),
                /* 0x46 */ new Opcode("LD B, (HL)", 0, this.ld_b_phl),
                /* 0x47 */ new Opcode("LD B, A", 0, this.ld_b_a),
                /* 0x48 */ new Opcode("LD C, B", 0, this.ld_c_b),
                /* 0x49 */ new Opcode("LD C, C", 0, this.nop), // Equivalent to nop
                /* 0x4A */ new Opcode("LD C, D", 0, this.ld_c_d),
                /* 0x4B */ new Opcode("LD C, E", 0, this.ld_c_e),
                /* 0x4C */ new Opcode("LD C, H", 0, this.ld_c_h),
                /* 0x4D */ new Opcode("LD C, L", 0, this.ld_c_l),
                /* 0x4E */ new Opcode("LD C, (HL)", 0, this.ld_c_phl),
                /* 0x4F */ new Opcode("LD C, A", 0, this.ld_c_a),

                /* 0x50 */ new Opcode("LD D, B", 0, this.ld_d_b),
                /* 0x51 */ new Opcode("LD D, C", 0, this.ld_d_c),
                /* 0x52 */ new Opcode("LD D, D", 0, this.nop), // Equivalent to nop
                /* 0x53 */ new Opcode("LD D, E", 0, this.ld_d_e),
                /* 0x54 */ new Opcode("LD D, H", 0, this.ld_d_h),
                /* 0x55 */ new Opcode("LD D, L", 0, this.ld_d_l),
                /* 0x56 */ new Opcode("LD D, (HL)", 0, this.ld_d_phl),
                /* 0x57 */ new Opcode("LD D, A", 0, this.ld_d_a),
                /* 0x58 */ new Opcode("LD E, B", 0, this.ld_e_b),
                /* 0x59 */ new Opcode("LD E, C", 0, this.ld_e_c),
                /* 0x5A */ new Opcode("LD E, D", 0, this.ld_e_d),
                /* 0x5B */ new Opcode("LD E, E", 0, this.nop), // Equivalent to nop
                /* 0x5C */ new Opcode("LD E, H", 0, this.ld_e_h),
                /* 0x5D */ new Opcode("LD E, L", 0, this.ld_e_l),
                /* 0x5E */ new Opcode("LD E, (HL)", 0, this.ld_e_phl),
                /* 0x5F */ new Opcode("LD E, A", 0, this.ld_e_a),

                /* 0x60 */ new Opcode("LD H, B", 0, this.ld_h_b),
                /* 0x61 */ new Opcode("LD H, C", 0, this.ld_h_c),
                /* 0x62 */ new Opcode("LD H, D", 0, this.ld_h_d),
                /* 0x63 */ new Opcode("LD H, E", 0, this.ld_h_e),
                /* 0x64 */ new Opcode("LD H, H", 0, this.nop), // Equivalent to nop
                /* 0x65 */ new Opcode("LD H, L", 0, this.ld_h_l),
                /* 0x66 */ new Opcode("LD H, (HL)", 0, this.ld_h_phl),
                /* 0x67 */ new Opcode("LD H, A", 0, this.ld_h_a),
                /* 0x68 */ new Opcode("LD L, B", 0, this.ld_l_b),
                /* 0x69 */ new Opcode("LD L, C", 0, this.ld_l_c),
                /* 0x6A */ new Opcode("LD L, D", 0, this.ld_l_d),
                /* 0x6B */ new Opcode("LD L, E", 0, this.ld_l_e),
                /* 0x6C */ new Opcode("LD L, H", 0, this.ld_l_h),
                /* 0x6D */ new Opcode("LD L, L", 0, this.nop), // Equivalent to nop
                /* 0x6E */ new Opcode("LD L, (HL)", 0, this.ld_l_phl),
                /* 0x6F */ new Opcode("LD L, A", 0, this.ld_l_a),

                /* 0x70 */ new Opcode("LD (HL), B", 0, this.ld_phl_b),
                /* 0x71 */ new Opcode("LD (HL), C", 0, this.ld_phl_c),
                /* 0x72 */ new Opcode("LD (HL), D", 0, this.ld_phl_d),
                /* 0x73 */ new Opcode("LD (HL), E", 0, this.ld_phl_e),
                /* 0x74 */ new Opcode("LD (HL), H", 0, this.ld_phl_h),
                /* 0x75 */ new Opcode("LD (HL), L", 0, this.ld_phl_l),
                /* 0x76 */ new Opcode("HALT", 0, this.halt),
                /* 0x77 */ new Opcode("LD (HL), A", 0, this.ld_phl_a),
                /* 0x78 */ new Opcode("LD A, B", 0, this.ld_a_b),
                /* 0x79 */ new Opcode("LD A, C", 0, this.ld_a_c),
                /* 0x7A */ new Opcode("LD A, D", 0, this.ld_a_d),
                /* 0x7B */ new Opcode("LD A, E", 0, this.ld_a_e),
                /* 0x7C */ new Opcode("LD A, H", 0, this.ld_a_h),
                /* 0x7D */ new Opcode("LD A, L", 0, this.ld_a_l),
                /* 0x7E */ new Opcode("LD A, (HL)", 0, this.ld_a_phl),
                /* 0x7F */ new Opcode("LD A, A", 0, this.nop), // Equivalent to nop

                /* 0x80 */ new Opcode("ADD A, B", 0, this.add_a_b),
                /* 0x81 */ new Opcode("ADD A, C", 0, this.add_a_c),
                /* 0x82 */ new Opcode("ADD A, D", 0, this.add_a_d),
                /* 0x83 */ new Opcode("ADD A, E", 0, this.add_a_e),
                /* 0x84 */ new Opcode("ADD A, H", 0, this.add_a_h),
                /* 0x85 */ new Opcode("ADD A, L", 0, this.add_a_l),
                /* 0x86 */ new Opcode("ADD A, (HL)", 0, this.add_a_phl),
                /* 0x87 */ new Opcode("ADD A, A", 0, this.add_a_a),
                /* 0x88 */ new Opcode("ADC A, B", 0, this.adc_a_b),
                /* 0x89 */ new Opcode("ADC A, C", 0, this.adc_a_c),
                /* 0x8A */ new Opcode("ADC A, D", 0, this.adc_a_d),
                /* 0x8B */ new Opcode("ADC A, E", 0, this.adc_a_e),
                /* 0x8C */ new Opcode("ADC A, H", 0, this.adc_a_h),
                /* 0x8D */ new Opcode("ADC A, L", 0, this.adc_a_l),
                /* 0x8E */ new Opcode("ADC A, (HL)", 0, this.adc_a_phl),
                /* 0x8F */ new Opcode("ADC A, A", 0, this.adc_a_a),

                /* 0x90 */ new Opcode("SUB A, B", 0, this.sub_a_b),
                /* 0x91 */ new Opcode("SUB A, C", 0, this.sub_a_c),
                /* 0x92 */ new Opcode("SUB A, D", 0, this.sub_a_d),
                /* 0x93 */ new Opcode("SUB A, E", 0, this.sub_a_e),
                /* 0x94 */ new Opcode("SUB A, H", 0, this.sub_a_h),
                /* 0x95 */ new Opcode("SUB A, L", 0, this.sub_a_l),
                /* 0x96 */ new Opcode("SUB A, (HL)", 0, this.sub_a_phl),
                /* 0x97 */ new Opcode("SUB A, A", 0, this.sub_a_a),
                /* 0x98 */ new Opcode("SBC A, B", 0, this.sbc_a_b),
                /* 0x99 */ new Opcode("SBC A, C", 0, this.sbc_a_c),
                /* 0x9A */ new Opcode("SBC A, D", 0, this.sbc_a_d),
                /* 0x9B */ new Opcode("SBC A, E", 0, this.sbc_a_e),
                /* 0x9C */ new Opcode("SBC A, H", 0, this.sbc_a_h),
                /* 0x9D */ new Opcode("SBC A, L", 0, this.sbc_a_l),
                /* 0x9E */ new Opcode("SBC A, (HL)", 0, this.sbc_a_phl),
                /* 0x9F */ new Opcode("SBC A, A", 0, this.sbc_a_a),

                /* 0xA0 */ new Opcode("AND B", 0, this.and_b),
                /* 0xA1 */ new Opcode("AND C", 0, this.and_c),
                /* 0xA2 */ new Opcode("AND D", 0, this.and_d),
                /* 0xA3 */ new Opcode("AND E", 0, this.and_e),
                /* 0xA4 */ new Opcode("AND H", 0, this.and_h),
                /* 0xA5 */ new Opcode("AND L", 0, this.and_l),
                /* 0xA6 */ new Opcode("AND (HL)", 0, this.and_phl),
                /* 0xA7 */ new Opcode("AND A", 0, this.and_a),
                /* 0xA8 */ new Opcode("XOR B", 0, this.xor_b),
                /* 0xA9 */ new Opcode("XOR C", 0, this.xor_c),
                /* 0xAA */ new Opcode("XOR D", 0, this.xor_d),
                /* 0xAB */ new Opcode("XOR E", 0, this.xor_e),
                /* 0xAC */ new Opcode("XOR H", 0, this.xor_h),
                /* 0xAD */ new Opcode("XOR L", 0, this.xor_l),
                /* 0xAE */ new Opcode("XOR (HL)", 0, this.xor_phl),
                /* 0xAF */ new Opcode("XOR A", 0, this.xor_a),

                /* 0xB0 */ new Opcode("OR B", 0, this.or_b),
                /* 0xB1 */ new Opcode("OR C", 0, this.or_c),
                /* 0xB2 */ new Opcode("OR D", 0, this.or_d),
                /* 0xB3 */ new Opcode("OR E", 0, this.or_e),
                /* 0xB4 */ new Opcode("OR H", 0, this.or_h),
                /* 0xB5 */ new Opcode("OR L", 0, this.or_l),
                /* 0xB6 */ new Opcode("OR (HL)", 0, this.or_phl),
                /* 0xB7 */ new Opcode("OR A", 0, this.or_a),
                /* 0xB8 */ new Opcode("CP B", 0, this.cp_b),
                /* 0xB9 */ new Opcode("CP C", 0, this.cp_c),
                /* 0xBA */ new Opcode("CP D", 0, this.cp_d),
                /* 0xBB */ new Opcode("CP E", 0, this.cp_e),
                /* 0xBC */ new Opcode("CP H", 0, this.cp_h),
                /* 0xBD */ new Opcode("CP L", 0, this.cp_l),
                /* 0xBE */ new Opcode("CP (HL)", 0, this.cp_phl),
                /* 0xBF */ new Opcode("CP A", 0, this.cp_a),

                /* 0xC0 */ new Opcode("RET NZ", 0, this.ret_nz),
                /* 0xC1 */ new Opcode("POP BC", 0, this.pop_bc),
                /* 0xC2 */ new Opcode("JP NZ, 0x{0:X4}", 2, this.jp_nz_nn),
                /* 0xC3 */ new Opcode("JP 0x{0:X4}", 2, this.jp_nn),
                /* 0xC4 */ new Opcode("CALL NZ, 0x{0:X4}", 2, this.call_nz_nn),
                /* 0xC5 */ new Opcode("PUSH BC", 0, this.push_bc),
                /* 0xC6 */ new Opcode("ADD A, 0x{0:X2}", 1, this.add_a_n),
                /* 0xC7 */ new Opcode("RST 0", 0, this.rst_0),
                /* 0xC8 */ new Opcode("RET Z", 0, this.ret_z),
                /* 0xC9 */ new Opcode("RET", 0, this.ret),
                /* 0xCA */ new Opcode("JP Z, 0x{0:X4}", 2, this.jp_z_nn),
                /* 0xCB */ new Opcode("EXT", 1, this.ext),
                /* 0xCC */ new Opcode("CALL Z, 0x{0:X4}", 2, this.call_z_nn),
                /* 0xCD */ new Opcode("CALL 0x{0:X4}", 2, this.call_nn),
                /* 0xCE */ new Opcode("ADC A, 0x{0:X2}", 1, this.adc_a_n),
                /* 0xCF */ new Opcode("RST 8", 0, this.rst_8),

                /* 0xD0 */ new Opcode("RET NC", 0, this.ret_nc),
                /* 0xD1 */ new Opcode("POP DE", 0, this.pop_de),
                /* 0xD2 */ new Opcode("JP NC, 0x{0:X4}", 2, this.jp_nc_nn),
                /* 0xD3 */ new Opcode("XX", 0, this.Crash),
                /* 0xD4 */ new Opcode("CALL NC, 0x{0:X4}", 2, this.call_nc_nn),
                /* 0xD5 */ new Opcode("PUSH DE", 0, this.push_de),
                /* 0xD6 */ new Opcode("SUB A, 0x{0:X2}", 1, this.sub_a_n),
                /* 0xD7 */ new Opcode("RST 10", 0, this.rst_10),
                /* 0xD8 */ new Opcode("RET C", 0, this.ret_c),
                /* 0xD9 */ new Opcode("RETI", 0, this.reti),
                /* 0xDA */ new Opcode("JP C, 0x{0:X4}", 2, this.jp_c_nn),
                /* 0xDB */ new Opcode("XX", 0, this.Crash),
                /* 0xDC */ new Opcode("CALL C, 0x{0:X4}", 2, this.call_c_nn),
                /* 0xDD */ new Opcode("XX", 0, this.Crash),
                /* 0xDE */ new Opcode("SBC A, 0x{0:X2}", 1, this.sbc_a_n),
                /* 0xDF */ new Opcode("RST 18", 0, this.rst_18),

                /* 0xE0 */ new Opcode("LDH (0x{0:X2}), A", 1, this.ldh_pn_a),
                /* 0xE1 */ new Opcode("POP HL", 0, this.pop_hl),
                /* 0xE2 */ new Opcode("LD (C), A", 0, this.ld_pc_a),
                /* 0xE3 */ new Opcode("XX", 0, this.Crash),
                /* 0xE4 */ new Opcode("XX", 0, this.Crash),
                /* 0xE5 */ new Opcode("PUSH HL", 0, this.push_hl),
                /* 0xE6 */ new Opcode("AND 0x{0:X2}", 1, this.and_n),
                /* 0xE7 */ new Opcode("RST 20", 0, this.rst_20),
                /* 0xE8 */ new Opcode("ADD SP, d0x{0:X2}", 1, this.add_sp_dn),
                /* 0xE9 */ new Opcode("JP (HL)", 0, this.jp_hl),
                /* 0xEA */ new Opcode("LD (0x{0:X4}), A", 2, this.ld_pnn_a),
                /* 0xEB */ new Opcode("XX", 0, this.Crash),
                /* 0xEC */ new Opcode("XX", 0, this.Crash),
                /* 0xED */ new Opcode("XX", 0, this.Crash),
                /* 0xEE */ new Opcode("XOR 0x{0:X2}", 1, this.xor_n),
                /* 0xEF */ new Opcode("RST 28", 0, this.rst_28),

                /* 0xF0 */ new Opcode("LDH A, (0x{0:X2})", 1, this.ldh_a_pn),
                /* 0xF1 */ new Opcode("POP AF", 0, this.pop_af),
                /* 0xF2 */ new Opcode("LDH A, (C)", 0, this.ld_a_pc),
                /* 0xF3 */ new Opcode("DI", 0, this.di),
                /* 0xF4 */ new Opcode("XX", 0, this.Crash),
                /* 0xF5 */ new Opcode("PUSH AF", 0, this.push_af),
                /* 0xF6 */ new Opcode("OR 0x{0:X2}", 1, this.or_n),
                /* 0xF7 */ new Opcode("RST 30", 0, this.rst_30),
                /* 0xF8 */ new Opcode("LDHL SP, d0x{0:X2}", 1, this.ldhl_spdn),
                /* 0xF9 */ new Opcode("LD SP, HL", 0, this.ld_sp_hl),
                /* 0xFA */ new Opcode("LD A, (0x{0:X4})", 2, this.ld_a_pnn),
                /* 0xFB */ new Opcode("EI", 0, this.ei),
                /* 0xFC */ new Opcode("XX", 0, this.Crash),
                /* 0xFD */ new Opcode("XX", 0, this.Crash),
                /* 0xFE */ new Opcode("CP 0x{0:X2}", 1, this.cp_n),
                /* 0xFF */ new Opcode("RST 38", 0, this.rst_38),
            };
    }
}
