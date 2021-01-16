namespace GusBoy
{
    public partial class CPU
    {
        private bool halfCarrySub(byte target, byte value, int carry = 0)
        {
            return (((target & 0xF) - (value & 0xF) - carry) < 0);
        }

        private bool halfCarryAdd(byte target, byte value, int carry = 0)
        {
            return ((target & 0xF) + (value & 0xF) + carry) > 0xF;
        }

        private bool halfCarryAdd(gshort target, gshort value)
        {
            return ((target & 0xFFF) + (value & 0xFFF)) > 0xFFF;
        }

        private int ext(gshort operand)
        {
            //extopcode instruction = extendedOpcodes[operand.Lo];

            /*if (instruction.func == null)
            {
                rPC -= 2;
                throw new gbException(String.Format($"Unsupported extended opcode 0x{operand:X2}: {instruction.mnemonic}"));
            }*/

            //return instruction.func();

            return extendedOpcodes[operand.Lo].func();
        }

        private int nop(gshort operand)
        {
            return 4;
        }

        private int jp_nn(gshort operand)
        {
            rPC = operand;
            return 16;
        }

        private int jp_nz_nn(gshort operand)
        {
            if (!fZ)
            {
                rPC = operand;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private int jp_z_nn(gshort operand)
        {
            if (fZ)
            {
                rPC = operand;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private int jp_nc_nn(gshort operand)
        {
            if (!fC)
            {
                rPC = operand;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private int jp_c_nn(gshort operand)
        {
            if (fC)
            {
                rPC = operand;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private int call_nn(gshort operand)
        {
            ram.SetShort(rSP - 2, rPC);
            rPC = operand;
            rSP -= 2;
            return 24;
        }

        private int call_nz_nn(gshort operand)
        {
            if (!fZ)
            {
                ram.SetShort(rSP - 2, rPC);
                rPC = operand;
                rSP -= 2;
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private int call_z_nn(gshort operand)
        {
            if (fZ)
            {
                ram.SetShort(rSP - 2, rPC);
                rPC = operand;
                rSP -= 2;
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private int call_nc_nn(gshort operand)
        {
            if (!fC)
            {
                ram.SetShort(rSP - 2, rPC);
                rPC = operand;
                rSP -= 2;
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private int call_c_nn(gshort operand)
        {
            if (fC)
            {
                ram.SetShort(rSP - 2, rPC);
                rPC = operand;
                rSP -= 2;
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private int rl_a(gshort operand)
        {
            bool oldFC = fC;

            fC = (rA & 0x80) == 0x80;
            rA = (byte)(rA << 1);
            if (oldFC) rA |= 0x01;

            fZ = false;
            fN = false;
            fH = false;

            return 4;
        }

        private int rr_a(gshort operand)
        {
            bool oldFC = fC;

            fC = (rA & 0x01) == 0x01;
            rA = (byte)(rA >> 1);
            if (oldFC) rA |= 0x80;

            fZ = false;
            fN = false;
            fH = false;

            return 4;
        }

        private int scf(gshort operand)
        {
            fN = false;
            fH = false;
            fC = true;

            return 4;
        }

        private int ccf(gshort operand)
        {
            fN = false;
            fH = false;
            fC = !fC;

            return 4;
        }

        private int ret(gshort operand)
        {
            rPC = ram.GetShort(rSP);
            rSP += 2;
            return 16;
        }

        private int reti(gshort operand)
        {
            rPC = ram.GetShort(rSP);
            rSP += 2;
            fInterruptMasterEnable = true;
            return 16;
        }

        private int ret_nz(gshort operand)
        {
            if (!fZ)
            {
                rPC = ram.GetShort(rSP);
                rSP += 2;
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private int ret_z(gshort operand)
        {
            if (fZ)
            {
                rPC = ram.GetShort(rSP);
                rSP += 2;
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private int ret_nc(gshort operand)
        {
            if (!fC)
            {
                rPC = ram.GetShort(rSP);
                rSP += 2;
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private int ret_c(gshort operand)
        {
            if (fC)
            {
                rPC = ram.GetShort(rSP);
                rSP += 2;
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private int xor_a(gshort operand)
        {
            rA ^= rA;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int xor_b(gshort operand)
        {
            rA ^= rB;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int xor_c(gshort operand)
        {
            rA ^= rC;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int xor_d(gshort operand)
        {
            rA ^= rD;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int xor_e(gshort operand)
        {
            rA ^= rE;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int xor_h(gshort operand)
        {
            rA ^= rH;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int xor_l(gshort operand)
        {
            rA ^= rL;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int xor_phl(gshort operand)
        {
            rA ^= ram[rHL];
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 8;
        }

        private int ld_bc_nn(gshort operand)
        {
            rBC = (ushort)operand;
            return 12;
        }

        private int ld_de_nn(gshort operand)
        {
            rDE = (ushort)operand;
            return 12;
        }

        private int ld_hl_nn(gshort operand)
        {
            rHL = (ushort)operand;
            return 12;
        }

        private int ld_sp_nn(gshort operand)
        {
            rSP = (ushort)operand;
            return 12;
        }

        private int ld_pnn_sp(gshort operand)
        {
            ram.SetShort(operand, rSP); // TODO: Check that SetShort works here
            return 20;
        }

        private int ld_sp_hl(gshort operand)
        {
            rSP = (ushort)rHL;
            return 8;
        }

        private int ld_a_pnn(gshort operand)
        {
            rA = ram[operand];
            return 16;
        }

        private int ldd_phl_a(gshort operand)
        {
            ram[rHL] = rA;
            rHL--;
            return 8;
        }

        private int ldd_a_phl(gshort operand)
        {
            rA = ram[rHL];
            rHL--;
            return 8;
        }

        private int ld_pbc_a(gshort operand)
        {
            ram[rBC] = rA;
            return 8;
        }

        private int ld_pde_a(gshort operand)
        {
            ram[rDE] = rA;
            return 8;
        }

        private int ld_phl_a(gshort operand)
        {
            ram[rHL] = rA;
            return 8;
        }

        private int ld_phl_b(gshort operand)
        {
            ram[rHL] = rB;
            return 8;
        }

        private int ld_phl_c(gshort operand)
        {
            ram[rHL] = rC;
            return 8;
        }

        private int ld_phl_d(gshort operand)
        {
            ram[rHL] = rD;
            return 8;
        }

        private int ld_phl_e(gshort operand)
        {
            ram[rHL] = rE;
            return 8;
        }

        private int ld_phl_h(gshort operand)
        {
            ram[rHL] = rH;
            return 8;
        }

        private int ld_phl_l(gshort operand)
        {
            ram[rHL] = rL;
            return 8;
        }

        private int dec_a(gshort operand)
        {
            fH = halfCarrySub(rA, 1);

            rA--;
            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int dec_b(gshort operand)
        {
            fH = halfCarrySub(rB, 1);

            rB--;
            fZ = rB == 0;
            fN = true;

            return 4;
        }

        private int dec_c(gshort operand)
        {
            fH = halfCarrySub(rC, 1);

            rC--;
            fZ = rC == 0;
            fN = true;

            return 4;
        }

        private int dec_d(gshort operand)
        {
            fH = halfCarrySub(rD, 1);

            rD--;
            fZ = rD == 0;
            fN = true;

            return 4;
        }

        private int dec_e(gshort operand)
        {
            fH = halfCarrySub(rE, 1);

            rE--;
            fZ = rE == 0;
            fN = true;

            return 4;
        }

        private int dec_h(gshort operand)
        {
            fH = halfCarrySub(rH, 1);

            rH--;
            fZ = rH == 0;
            fN = true;

            return 4;
        }

        private int dec_l(gshort operand)
        {
            fH = halfCarrySub(rL, 1);

            rL--;
            fZ = rL == 0;
            fN = true;

            return 4;
        }

        private int dec_phl(gshort operand)
        {
            fH = halfCarrySub(ram[rHL], 1);

            ram[rHL]--;
            fZ = ram[rHL] == 0;
            fN = true;

            return 12;
        }

        private int sub_a_a(gshort operand)
        {
            fH = halfCarrySub(rA, rA);
            fC = rA > rA;

            rA -= rA;

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sub_a_b(gshort operand)
        {
            fH = halfCarrySub(rA, rB);
            fC = rB > rA;

            rA -= rB;

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sub_a_c(gshort operand)
        {
            fH = halfCarrySub(rA, rC);
            fC = rC > rA;

            rA -= rC;

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sub_a_d(gshort operand)
        {
            fH = halfCarrySub(rA, rD);
            fC = rD > rA;

            rA -= rD;

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sub_a_e(gshort operand)
        {
            fH = halfCarrySub(rA, rE);
            fC = rE > rA;

            rA -= rE;

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sub_a_h(gshort operand)
        {
            fH = halfCarrySub(rA, rH);
            fC = rH > rA;

            rA -= rH;

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sub_a_l(gshort operand)
        {
            fH = halfCarrySub(rA, rL);
            fC = rL > rA;

            rA -= rL;

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sub_a_phl(gshort operand)
        {
            fH = halfCarrySub(rA, ram[rHL]);
            fC = ram[rHL] > rA;

            rA -= ram[rHL];

            fZ = rA == 0;
            fN = true;

            return 8;
        }

        private int sub_a_n(gshort operand)
        {
            fH = halfCarrySub(rA, operand.Lo);
            fC = operand.Lo > rA;

            rA -= operand.Lo;

            fZ = rA == 0;
            fN = true;

            return 8;
        }

        private int sbc_a_a(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarrySub(rA, rA, carry);
            fC = rA - rA - carry < 0;

            rA -= (byte)(rA + carry);

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sbc_a_b(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarrySub(rA, rB, carry);
            fC = rA - rB - carry < 0;

            rA -= (byte)(rB + carry);

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sbc_a_c(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarrySub(rA, rC, carry);
            fC = rA - rC - carry < 0;

            rA -= (byte)(rC + carry);

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sbc_a_d(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarrySub(rA, rD, carry);
            fC = rA - rD - carry < 0;

            rA -= (byte)(rD + carry);

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sbc_a_e(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarrySub(rA, rE, carry);
            fC = rA - rE - carry < 0;

            rA -= (byte)(rE + carry);

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sbc_a_h(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarrySub(rA, rH, carry);
            fC = rA - rH - carry < 0;

            rA -= (byte)(rH + carry);

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sbc_a_l(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarrySub(rA, rL, carry);
            fC = rA - rL - carry < 0;

            rA -= (byte)(rL + carry);

            fZ = rA == 0;
            fN = true;

            return 4;
        }

        private int sbc_a_n(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarrySub(rA, operand.Lo, carry);
            fC = rA - operand.Lo - carry < 0;

            rA -= (byte)(operand.Lo + carry);

            fZ = rA == 0;
            fN = true;

            return 8; // WAS 4
        }

        private int sbc_a_phl(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarrySub(rA, ram[rHL], carry);
            fC = rA - ram[rHL] - carry < 0;

            rA -= (byte)(ram[rHL] + carry);

            fZ = rA == 0;
            fN = true;

            return 8; // WAS 4
        }

        private int cp_a(gshort operand)
        {
            fZ = rA == rA;
            fN = true;
            fH = halfCarrySub(rA, rA);
            fC = rA < rA;

            return 4;
        }

        private int cp_b(gshort operand)
        {
            fZ = rA == rB;
            fN = true;
            fH = halfCarrySub(rA, rB);
            fC = rA < rB;

            return 4;
        }

        private int cp_c(gshort operand)
        {
            fZ = rA == rC;
            fN = true;
            fH = halfCarrySub(rA, rC);
            fC = rA < rC;

            return 4;
        }

        private int cp_d(gshort operand)
        {
            fZ = rA == rD;
            fN = true;
            fH = halfCarrySub(rA, rD);
            fC = rA < rD;

            return 4;
        }

        private int cp_e(gshort operand)
        {
            fZ = rA == rE;
            fN = true;
            fH = halfCarrySub(rA, rE);
            fC = rA < rE;

            return 4;
        }

        private int cp_h(gshort operand)
        {
            fZ = rA == rH;
            fN = true;
            fH = halfCarrySub(rA, rH);
            fC = rA < rH;

            return 4;
        }

        private int cp_l(gshort operand)
        {
            fZ = rA == rL;
            fN = true;
            fH = halfCarrySub(rA, rL);
            fC = rA < rL;

            return 4;
        }

        private int cp_phl(gshort operand)
        {
            fZ = rA == ram[rHL];
            fN = true;
            fH = halfCarrySub(rA, ram[rHL]);
            fC = rA < ram[rHL];

            return 8;
        }

        private int cp_n(gshort operand)
        {
            fZ = rA == operand.Lo;
            fN = true;
            fH = halfCarrySub(rA, operand.Lo);
            fC = rA < operand.Lo;

            return 8;
        }

        private int dec_bc(gshort operand)
        {
            rBC--;
            return 8;
        }

        private int dec_de(gshort operand)
        {
            rDE--;
            return 8;
        }

        private int dec_hl(gshort operand)
        {
            rHL--;
            return 8;
        }

        private int dec_sp(gshort operand)
        {
            rSP--;
            return 8;
        }

        private int inc_a(gshort operand)
        {
            fH = halfCarryAdd(rA, 1);

            rA++;
            fZ = rA == 0;
            fN = false;

            return 4;
        }

        private int inc_b(gshort operand)
        {
            fH = halfCarryAdd(rB, 1);

            rB++;
            fZ = rB == 0;
            fN = false;

            return 4;
        }

        private int inc_c(gshort operand)
        {
            fH = halfCarryAdd(rC, 1);

            rC++;
            fZ = rC == 0;
            fN = false;

            return 4;
        }

        private int inc_d(gshort operand)
        {
            fH = halfCarryAdd(rD, 1);

            rD++;
            fZ = rD == 0;
            fN = false;

            return 4;
        }

        private int inc_e(gshort operand)
        {
            fH = halfCarryAdd(rE, 1);

            rE++;
            fZ = rE == 0;
            fN = false;

            return 4;
        }

        private int inc_h(gshort operand)
        {
            fH = halfCarryAdd(rH, 1);

            rH++;
            fZ = rH == 0;
            fN = false;

            return 4;
        }

        private int inc_l(gshort operand)
        {
            fH = halfCarryAdd(rL, 1);

            rL++;
            fZ = rL == 0;
            fN = false;

            return 4;
        }

        private int inc_phl(gshort operand)
        {
            fH = halfCarryAdd(ram[rHL], 1);

            ram[rHL]++;
            fZ = ram[rHL] == 0;
            fN = false;

            return 12;
        }

        private int inc_bc(gshort operand)
        {
            rBC++;
            return 8;
        }
        private int inc_de(gshort operand)
        {
            rDE++;
            return 8;
        }
        private int inc_hl(gshort operand)
        {
            rHL++;
            return 8;
        }
        private int inc_sp(gshort operand)
        {
            rSP++;
            return 8;
        }

        private int jr_nz_dn(gshort operand)
        {            
            if (!fZ)
            {
                rPC += (sbyte)operand.Lo;
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int jr_z_dn(gshort operand)
        {
            if (fZ)
            {
                rPC += (sbyte)operand.Lo;
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int jr_nc_dn(gshort operand)
        {
            if (!fC)
            {
                rPC += (sbyte)operand.Lo;
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int jr_c_dn(gshort operand)
        {
            if (fC)
            {
                rPC += (sbyte)operand.Lo;
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int jr_dn(gshort operand)
        {
            rPC += (sbyte)operand.Lo;
            return 12;
        }

        private int di(gshort operand)
        {
            fInterruptMasterEnable = false;
            return 4;
        }

        private int ei(gshort operand)
        {
            fInterruptMasterEnable = true;
            return 4;
        }

        private int ldh_pn_a(gshort operand)
        {
            ram[0xFF00 + operand] = rA;
            return 12;
        }

        private int ldh_a_pn(gshort operand)
        {
            rA = ram[0xFF00 + operand];
            return 12;
        }

        private int ld_a_n(gshort operand)
        {
            rA = operand.Lo;
            return 8;
        }

        private int ld_b_n(gshort operand)
        {
            rB = operand.Lo;
            return 8;
        }

        private int ld_c_n(gshort operand)
        {
            rC = operand.Lo;
            return 8;
        }

        private int ld_d_n(gshort operand)
        {
            rD = operand.Lo;
            return 8;
        }

        private int ld_e_n(gshort operand)
        {
            rE = operand.Lo;
            return 8;
        }

        private int ld_h_n(gshort operand)
        {
            rH = operand.Lo;
            return 8;
        }

        private int ld_l_n(gshort operand)
        {
            rL = operand.Lo;
            return 8;
        }

        private int ld_phl_n(gshort operand)
        {
            ram[rHL] = operand.Lo;

            return 12;
        }

        private int ld_pnn_a(gshort operand)
        {
            ram[operand] = rA;

            return 16;
        }

        private int ldi_a_phl(gshort operand)
        {
            rA = ram[rHL];
            rHL++;

            return 8;
        }

        private int ldi_phl_a(gshort operand)
        {
            ram[rHL] = rA;
            rHL++;

            return 8;
        }

        private int ld_pc_a(gshort operand)
        {
            ram[0xFF00 + rC] = rA;

            return 8; // TODO: Was 12? "ld   (FF00+C),A"
        }

        private int ld_a_pc(gshort operand)
        {
            rA = ram[0xFF00 + rC];

            return 8; // TODO: Was 12? "ld   A,(FF00+C)"
        }

        // Register copies
        private int ld_a_b(gshort operand)
        {
            rA = rB;
            return 4;
        }

        private int ld_a_c(gshort operand)
        {
            rA = rC;
            return 4;
        }

        private int ld_a_d(gshort operand)
        {
            rA = rD;
            return 4;
        }

        private int ld_a_e(gshort operand)
        {
            rA = rE;
            return 4;
        }

        private int ld_a_h(gshort operand)
        {
            rA = rH;
            return 4;
        }

        private int ld_a_l(gshort operand)
        {
            rA = rL;
            return 4;
        }

        private int ld_a_phl(gshort operand)
        {
            rA = ram[rHL];
            return 8;
        }

        private int ld_a_pbc(gshort operand)
        {
            rA = ram[rBC];
            return 8;
        }

        private int ld_a_pde(gshort operand)
        {
            rA = ram[rDE];
            return 8;
        }


        private int ld_b_a(gshort operand)
        {
            rB = rA;
            return 4;
        }

        private int ld_b_c(gshort operand)
        {
            rB = rC;
            return 4;
        }

        private int ld_b_d(gshort operand)
        {
            rB = rD;
            return 4;
        }

        private int ld_b_e(gshort operand)
        {
            rB = rE;
            return 4;
        }

        private int ld_b_h(gshort operand)
        {
            rB = rH;
            return 4;
        }

        private int ld_b_l(gshort operand)
        {
            rB = rL;
            return 4;
        }

        private int ld_b_phl(gshort operand)
        {
            rB = ram[rHL];
            return 8;
        }


        private int ld_c_a(gshort operand)
        {
            rC = rA;
            return 4;
        }

        private int ld_c_b(gshort operand)
        {
            rC = rB;
            return 4;
        }

        private int ld_c_d(gshort operand)
        {
            rC = rD;
            return 4;
        }

        private int ld_c_e(gshort operand)
        {
            rC = rE;
            return 4;
        }

        private int ld_c_h(gshort operand)
        {
            rC = rH;
            return 4;
        }

        private int ld_c_l(gshort operand)
        {
            rC = rL;
            return 4;
        }

        private int ld_c_phl(gshort operand)
        {
            rC = ram[rHL];
            return 8;
        }


        private int ld_d_a(gshort operand)
        {
            rD = rA;
            return 4;
        }

        private int ld_d_b(gshort operand)
        {
            rD = rB;
            return 4;
        }

        private int ld_d_c(gshort operand)
        {
            rD = rC;
            return 4;
        }

        private int ld_d_e(gshort operand)
        {
            rD = rE;
            return 4;
        }

        private int ld_d_h(gshort operand)
        {
            rD = rH;
            return 4;
        }

        private int ld_d_l(gshort operand)
        {
            rD = rL;
            return 4;
        }

        private int ld_d_phl(gshort operand)
        {
            rD = ram[rHL];
            return 8;
        }


        private int ld_e_a(gshort operand)
        {
            rE = rA;
            return 4;
        }

        private int ld_e_b(gshort operand)
        {
            rE = rB;
            return 4;
        }

        private int ld_e_c(gshort operand)
        {
            rE = rC;
            return 4;
        }

        private int ld_e_d(gshort operand)
        {
            rE = rD;
            return 4;
        }

        private int ld_e_h(gshort operand)
        {
            rE = rH;
            return 4;
        }

        private int ld_e_l(gshort operand)
        {
            rE = rL;
            return 4;
        }

        private int ld_e_phl(gshort operand)
        {
            rE = ram[rHL];
            return 8;
        }


        private int ld_h_a(gshort operand)
        {
            rH = rA;
            return 4;
        }

        private int ld_h_b(gshort operand)
        {
            rH = rB;
            return 4;
        }

        private int ld_h_c(gshort operand)
        {
            rH = rC;
            return 4;
        }

        private int ld_h_d(gshort operand)
        {
            rH = rD;
            return 4;
        }

        private int ld_h_e(gshort operand)
        {
            rH = rE;
            return 4;
        }

        private int ld_h_l(gshort operand)
        {
            rH = rL;
            return 4;
        }

        private int ld_h_phl(gshort operand)
        {
            rH = ram[rHL];
            return 8;
        }


        private int ld_l_a(gshort operand)
        {
            rL = rA;
            return 4;
        }

        private int ld_l_b(gshort operand)
        {
            rL = rB;
            return 4;
        }

        private int ld_l_c(gshort operand)
        {
            rL = rC;
            return 4;
        }

        private int ld_l_d(gshort operand)
        {
            rL = rD;
            return 4;
        }

        private int ld_l_e(gshort operand)
        {
            rL = rE;
            return 4;
        }

        private int ld_l_h(gshort operand)
        {
            rL = rH;
            return 4;
        }

        private int ld_l_phl(gshort operand)
        {
            rL = ram[rHL];
            return 8;
        }


        private int or_a(gshort operand)
        {
            rA |= rA;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int or_b(gshort operand)
        {
            rA |= rB;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int or_c(gshort operand)
        {
            rA |= rC;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int or_d(gshort operand)
        {
            rA |= rD;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int or_e(gshort operand)
        {
            rA |= rE;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int or_h(gshort operand)
        {
            rA |= rH;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int or_l(gshort operand)
        {
            rA |= rL;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 4;
        }

        private int or_phl(gshort operand)
        {
            rA |= ram[rHL];
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 8;
        }

        private int cpl(gshort operand)
        {
            rA = (byte)(~rA);
            fN = true;
            fH = true;

            return 4;
        }

        private int add_sp_dn(gshort operand)
        {
            // Weird instruction. Flags based on *unsigned* math, results based on *signed* math.
            fH = halfCarryAdd(rSP.Lo, operand.Lo);
            fC = ((rSP.Lo + operand.Lo) & 0x100) == 0x100;

            rSP += (sbyte)operand.Lo;

            fZ = false;
            fN = false;

            return 16;
        }

        private int ldhl_spdn(gshort operand)
        {
            // Weird instruction. Flags based on *unsigned* math, results based on *signed* math.
            fH = halfCarryAdd(rSP.Lo, operand.Lo);
            fC = ((rSP.Lo + operand.Lo) & 0x100) == 0x100;

            rHL = rSP + (sbyte)operand.Lo;

            fZ = false;
            fN = false;

            return 12;
        }

        private int and_a(gshort operand)
        {
            rA &= rA;
            fZ = rA == 0;
            fN = false;
            fH = true;
            fC = false;

            return 4;
        }

        private int and_b(gshort operand)
        {
            rA &= rB;
            fZ = rA == 0;
            fN = false;
            fH = true;
            fC = false;

            return 4;
        }

        private int and_c(gshort operand)
        {
            rA &= rC;
            fZ = rA == 0;
            fN = false;
            fH = true;
            fC = false;

            return 4;
        }

        private int and_d(gshort operand)
        {
            rA &= rD;
            fZ = rA == 0;
            fN = false;
            fH = true;
            fC = false;

            return 4;
        }

        private int and_e(gshort operand)
        {
            rA &= rE;
            fZ = rA == 0;
            fN = false;
            fH = true;
            fC = false;

            return 4;
        }

        private int and_h(gshort operand)
        {
            rA &= rH;
            fZ = rA == 0;
            fN = false;
            fH = true;
            fC = false;

            return 4;
        }

        private int and_l(gshort operand)
        {
            rA &= rL;
            fZ = rA == 0;
            fN = false;
            fH = true;
            fC = false;

            return 4;
        }

        private int and_phl(gshort operand)
        {
            rA &= ram[rHL];
            fZ = rA == 0;
            fN = false;
            fH = true;
            fC = false;

            return 8;
        }

        private int and_n(gshort operand)
        {
            rA &= operand.Lo;
            fZ = rA == 0;
            fN = false;
            fH = true;
            fC = false;

            return 8;
        }

        private int xor_n(gshort operand)
        {
            rA ^= operand.Lo;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 8;
        }

        private int or_n(gshort operand)
        {
            rA |= operand.Lo;
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 8;
        }

        private int rst_0(gshort operand)
        {
            ram.SetShort(rSP - 2, rPC);
            rPC = 0x00;
            rSP -= 2;
            return 16;
        }

        private int rst_8(gshort operand)
        {
            ram.SetShort(rSP - 2, rPC);
            rPC = 0x08;
            rSP -= 2;
            return 16;
        }

        private int rst_10(gshort operand)
        {
            ram.SetShort(rSP - 2, rPC);
            rPC = 0x10;
            rSP -= 2;
            return 16;
        }

        private int rst_18(gshort operand)
        {
            ram.SetShort(rSP - 2, rPC);
            rPC = 0x18;
            rSP -= 2;
            return 16;
        }

        private int rst_20(gshort operand)
        {
            ram.SetShort(rSP - 2, rPC);
            rPC = 0x20;
            rSP -= 2;
            return 16;
        }

        private int rst_28(gshort operand)
        {
            ram.SetShort(rSP - 2, rPC);
            rPC = 0x28;
            rSP -= 2;
            return 16;
        }

        private int rst_30(gshort operand)
        {
            ram.SetShort(rSP - 2, rPC);
            rPC = 0x30;
            rSP -= 2;
            return 16;
        }

        private int rst_38(gshort operand)
        {
            ram.SetShort(rSP - 2, rPC);
            rPC = 0x38;
            rSP -= 2;
            return 16;
        }

        private int add_a_a(gshort operand)
        {
            fH = halfCarryAdd(rA, rA);

            int result = rA + rA;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_b(gshort operand)
        {
            fH = halfCarryAdd(rA, rB);

            int result = rA + rB;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_c(gshort operand)
        {
            fH = halfCarryAdd(rA, rC);

            int result = rA + rC;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_d(gshort operand)
        {
            fH = halfCarryAdd(rA, rD);

            int result = rA + rD;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_e(gshort operand)
        {
            fH = halfCarryAdd(rA, rE);

            int result = rA + rE;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_h(gshort operand)
        {
            fH = halfCarryAdd(rA, rH);

            int result = rA + rH;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_l(gshort operand)
        {
            fH = halfCarryAdd(rA, rL);

            int result = rA + rL;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_phl(gshort operand)
        {
            fH = halfCarryAdd(rA, ram[rHL]);

            int result = rA + ram[rHL];
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = (result & 0x100) == 0x100;

            return 8;
        }

        private int add_a_n(gshort operand)
        {
            fH = halfCarryAdd(rA, operand.Lo);

            int result = rA + operand.Lo;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = (result & 0x100) == 0x100;

            return 8;
        }

        private int adc_a_a(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarryAdd(rA, rA, carry);

            int result = rA + rA + carry;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = result > 0xFF;

            return 4;
        }

        private int adc_a_b(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarryAdd(rA, rB, carry);

            int result = rA + rB + carry;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = result > 0xFF;

            return 4;
        }

        private int adc_a_c(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarryAdd(rA, rC, carry);

            int result = rA + rC + carry;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = result > 0xFF;

            return 4;
        }

        private int adc_a_d(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarryAdd(rA, rD, carry);

            int result = rA + rD + carry;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = result > 0xFF;

            return 4;
        }

        private int adc_a_e(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarryAdd(rA, rE, carry);

            int result = rA + rE + carry;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = result > 0xFF;

            return 4;
        }

        private int adc_a_h(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarryAdd(rA, rH, carry);

            int result = rA + rH + carry;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = result > 0xFF;

            return 4;
        }

        private int adc_a_l(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarryAdd(rA, rL, carry);

            int result = rA + rL + carry;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = result > 0xFF;

            return 4;
        }

        private int adc_a_n(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarryAdd(rA, operand.Lo, carry);

            int result = rA + operand.Lo + carry;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = result > 0xFF;

            return 8;
        }

        private int adc_a_phl(gshort operand)
        {
            int carry = fC ? 1 : 0;
            fH = halfCarryAdd(rA, ram[rHL], carry);

            int result = rA + ram[rHL] + carry;
            rA = (byte)result;
            fZ = rA == 0;
            fN = false;
            fC = result > 0xFF;

            return 8;
        }

        private int push_bc(gshort operand)
        {
            ram.SetShort(rSP - 2, rBC);
            rSP -= 2;
            return 16;
        }

        private int push_de(gshort operand)
        {
            ram.SetShort(rSP - 2, rDE);
            rSP -= 2;
            return 16;
        }

        private int push_hl(gshort operand)
        {
            ram.SetShort(rSP - 2, rHL);
            rSP -= 2;
            return 16;
        }

        private int push_af(gshort operand)
        {
            ram.SetShort(rSP - 2, rAF);
            rSP -= 2;
            return 16;
        }

        private int pop_bc(gshort operand)
        {
            rBC = (ushort)ram.GetShort(rSP);
            rSP += 2;
            return 12;
        }

        private int pop_de(gshort operand)
        {
            rDE = (ushort)ram.GetShort(rSP);
            rSP += 2;
            return 12;
        }

        private int pop_hl(gshort operand)
        {
            rHL = (ushort)ram.GetShort(rSP);
            rSP += 2;
            return 12;
        }

        private int pop_af(gshort operand)
        {
            // Special case, lower four bits of F are always 0
            rAF = ram.GetShort(rSP) & 0xFFF0;

            rSP += 2;
            return 12;
        }

        private int add_hl_bc(gshort operand)
        {
            fH = halfCarryAdd(rHL, rBC);

            int result = rHL + rBC;
            rHL = result;
            fN = false;
            fC = (result & 0x10000) == 0x10000;

            return 8;
        }

        private int add_hl_de(gshort operand)
        {
            fH = halfCarryAdd(rHL, rDE);

            int result = rHL + rDE;
            rHL = result;
            fN = false;
            fC = (result & 0x10000) == 0x10000;

            return 8;
        }

        private int add_hl_hl(gshort operand)
        {
            fH = halfCarryAdd(rHL, rHL);

            int result = rHL + rHL;
            rHL = result;
            fN = false;
            fC = (result & 0x10000) == 0x10000;

            return 8;
        }

        private int add_hl_sp(gshort operand)
        {
            fH = halfCarryAdd(rHL, rSP);
            //fH = ((rHL & 0xFFF) + (rSP & 0xFFF)) > 0xFFF;

            int result = rHL + rSP;
            rHL = result;
            fN = false;
            fC = result > 0xFFFF;

            return 8;
        }

        private int jp_hl(gshort operand)
        {
            rPC = (ushort)rHL;
            return 4;
        }

        private int rlc_a(gshort operand)
        {
            fC = (rA & 0x80) == 0x80;

            rA = (byte)(rA << 1);
            if (fC) rA |= 0x01;

            fZ = false;
            fN = false;
            fH = false;

            return 4;
        }

        private int rrc_a(gshort operand)
        {
            fC = (rA & 0x01) == 0x01;

            rA = (byte)(rA >> 1);
            if (fC) rA |= 0x80;

            fZ = false;
            fN = false;
            fH = false;

            return 4;
        }

        private int stop(gshort operand)
        {
            fStop = true;

            // Work around buggy software? ROMs will hard-lock the gameboy if they halt with interrupts disabled. Blargg's cpu_instrs test appears to do this right before test 3.
            if (rInterruptEnable == 0)
            {
                rInterruptEnable = 0x1f;
            }

            return 0; // WAS 4
        }

        private int halt(gshort operand)
        {
            fHalt = true;

            // Work around buggy software? ROMs will hard-lock the gameboy if they halt with interrupts disabled.
            if (rInterruptEnable == 0)
            {
                rInterruptEnable = 0x1f;
            }

            if (!fInterruptMasterEnable && (rInterruptEnable & rInterruptFlags & 0x1f) != 0)
            {
                // HALT bug. Just disabling halt isn't accurate, but is probably close enough
                fHalt = false;
            }

            return 0; // WAS 4
        }

        private int daa(gshort operand)
        {
            gshort a = rA;

            if (!fN)
            {
                if (fH || (a & 0x0F) > 9)
                    a += 6;

                if (fC || a > 0x9F)
                    a += 0x60;
            }
            else
            {
                if (fH)
                    a = (a - 6) & 0xFF;

                if (fC)
                    a -= 0x60;
            }

            rF = (byte)(rF & ~(0x20 | 0x80));
            if ((a & 0x100) != 0)
                rF |= 0x10;

            a &= 0xFF;
            if (a == 0)
                rF |= 0x80;

            rA = a.Lo;

            return 4;
        }

        private int crash(gshort operand)
        {
            //throw new gbException($"Executed illegal instruction at address 0x{rPC:X4}");
            return 4;
        }
    }
}