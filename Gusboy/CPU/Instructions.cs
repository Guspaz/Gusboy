namespace Gusboy
{
    using System;

    /// <summary>
    /// Implementations of all single-byte opcodes.
    /// </summary>
    public partial class CPU
    {
        private static bool HalfCarrySub(byte target, byte value, int carry = 0) => ((target & 0xF) - (value & 0xF) - carry) < 0;

        private static bool HalfCarryAdd(byte target, byte value, int carry = 0) => ((target & 0xF) + (value & 0xF) + carry) > 0xF;

        private static bool HalfCarryAdd(ushort target, ushort value) => ((target & 0xFFF) + (value & 0xFFF)) > 0xFFF;

        private int Crash(ushort operand) => throw new Exception($"Executed illegal instruction at address 0x{this.rPC:X4}");

#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable IDE1006 // Naming Styles
        private int ext(ushort operand) => this.extendedOpcodes[(byte)operand].Func();

        private int nop(ushort operand) => 4;

        private int jp_nn(ushort operand)
        {
            this.rPC = operand;
            return 16;
        }

        private int jp_nz_nn(ushort operand)
        {
            if (!this.fZ)
            {
                this.rPC = operand;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private int jp_z_nn(ushort operand)
        {
            if (this.fZ)
            {
                this.rPC = operand;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private int jp_nc_nn(ushort operand)
        {
            if (!this.fC)
            {
                this.rPC = operand;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private int jp_c_nn(ushort operand)
        {
            if (this.fC)
            {
                this.rPC = operand;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private int call_nn(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = operand;
            this.rSP -= 2;
            return 24;
        }

        private int call_nz_nn(ushort operand)
        {
            if (!this.fZ)
            {
                this.Ram.SetShort(this.rSP - 2, this.rPC);
                this.rPC = operand;
                this.rSP -= 2;
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private int call_z_nn(ushort operand)
        {
            if (this.fZ)
            {
                this.Ram.SetShort(this.rSP - 2, this.rPC);
                this.rPC = operand;
                this.rSP -= 2;
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private int call_nc_nn(ushort operand)
        {
            if (!this.fC)
            {
                this.Ram.SetShort(this.rSP - 2, this.rPC);
                this.rPC = operand;
                this.rSP -= 2;
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private int call_c_nn(ushort operand)
        {
            if (this.fC)
            {
                this.Ram.SetShort(this.rSP - 2, this.rPC);
                this.rPC = operand;
                this.rSP -= 2;
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private int rl_a(ushort operand)
        {
            bool oldFC = this.fC;

            this.fC = (this.rA & 0x80) == 0x80;
            this.rA = (byte)(this.rA << 1);
            if (oldFC)
            {
                this.rA |= 0x01;
            }

            this.fZ = false;
            this.fN = false;
            this.fH = false;

            return 4;
        }

        private int rr_a(ushort operand)
        {
            bool oldFC = this.fC;

            this.fC = (this.rA & 0x01) == 0x01;
            this.rA = (byte)(this.rA >> 1);
            if (oldFC)
            {
                this.rA |= 0x80;
            }

            this.fZ = false;
            this.fN = false;
            this.fH = false;

            return 4;
        }

        private int scf(ushort operand)
        {
            this.fN = false;
            this.fH = false;
            this.fC = true;

            return 4;
        }

        private int ccf(ushort operand)
        {
            this.fN = false;
            this.fH = false;
            this.fC = !this.fC;

            return 4;
        }

        private int ret(ushort operand)
        {
            this.rPC = this.Ram.GetShort(this.rSP);
            this.rSP += 2;
            return 16;
        }

        private int reti(ushort operand)
        {
            this.rPC = this.Ram.GetShort(this.rSP);
            this.rSP += 2;
            this.fInterruptMasterEnable = true;
            return 16;
        }

        private int ret_nz(ushort operand)
        {
            if (!this.fZ)
            {
                this.rPC = this.Ram.GetShort(this.rSP);
                this.rSP += 2;
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private int ret_z(ushort operand)
        {
            if (this.fZ)
            {
                this.rPC = this.Ram.GetShort(this.rSP);
                this.rSP += 2;
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private int ret_nc(ushort operand)
        {
            if (!this.fC)
            {
                this.rPC = this.Ram.GetShort(this.rSP);
                this.rSP += 2;
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private int ret_c(ushort operand)
        {
            if (this.fC)
            {
                this.rPC = this.Ram.GetShort(this.rSP);
                this.rSP += 2;
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private int xor_a(ushort operand)
        {
            this.rA ^= this.rA;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_b(ushort operand)
        {
            this.rA ^= this.rB;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_c(ushort operand)
        {
            this.rA ^= this.rC;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_d(ushort operand)
        {
            this.rA ^= this.rD;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_e(ushort operand)
        {
            this.rA ^= this.rE;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_h(ushort operand)
        {
            this.rA ^= this.rH;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_l(ushort operand)
        {
            this.rA ^= this.rL;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_phl(ushort operand)
        {
            this.rA ^= this.Ram[this.rHL];
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int ld_bc_nn(ushort operand)
        {
            this.rBC = (ushort)operand;
            return 12;
        }

        private int ld_de_nn(ushort operand)
        {
            this.rDE = (ushort)operand;
            return 12;
        }

        private int ld_hl_nn(ushort operand)
        {
            this.rHL = (ushort)operand;
            return 12;
        }

        private int ld_sp_nn(ushort operand)
        {
            this.rSP = (ushort)operand;
            return 12;
        }

        private int ld_pnn_sp(ushort operand)
        {
            this.Ram.SetShort(operand, this.rSP); // TODO: Check that SetShort works here
            return 20;
        }

        private int ld_sp_hl(ushort operand)
        {
            this.rSP = (ushort)this.rHL;
            return 8;
        }

        private int ld_a_pnn(ushort operand)
        {
            this.rA = this.Ram[operand];
            return 16;
        }

        private int ldd_phl_a(ushort operand)
        {
            this.Ram[this.rHL] = this.rA;
            this.rHL--;
            return 8;
        }

        private int ldd_a_phl(ushort operand)
        {
            this.rA = this.Ram[this.rHL];
            this.rHL--;
            return 8;
        }

        private int ld_pbc_a(ushort operand)
        {
            this.Ram[this.rBC] = this.rA;
            return 8;
        }

        private int ld_pde_a(ushort operand)
        {
            this.Ram[this.rDE] = this.rA;
            return 8;
        }

        private int ld_phl_a(ushort operand)
        {
            this.Ram[this.rHL] = this.rA;
            return 8;
        }

        private int ld_phl_b(ushort operand)
        {
            this.Ram[this.rHL] = this.rB;
            return 8;
        }

        private int ld_phl_c(ushort operand)
        {
            this.Ram[this.rHL] = this.rC;
            return 8;
        }

        private int ld_phl_d(ushort operand)
        {
            this.Ram[this.rHL] = this.rD;
            return 8;
        }

        private int ld_phl_e(ushort operand)
        {
            this.Ram[this.rHL] = this.rE;
            return 8;
        }

        private int ld_phl_h(ushort operand)
        {
            this.Ram[this.rHL] = this.rH;
            return 8;
        }

        private int ld_phl_l(ushort operand)
        {
            this.Ram[this.rHL] = this.rL;
            return 8;
        }

        private int dec_a(ushort operand)
        {
            this.fH = HalfCarrySub(this.rA, 1);

            this.rA--;
            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int dec_b(ushort operand)
        {
            this.fH = HalfCarrySub(this.rB, 1);

            this.rB--;
            this.fZ = this.rB == 0;
            this.fN = true;

            return 4;
        }

        private int dec_c(ushort operand)
        {
            this.fH = HalfCarrySub(this.rC, 1);

            this.rC--;
            this.fZ = this.rC == 0;
            this.fN = true;

            return 4;
        }

        private int dec_d(ushort operand)
        {
            this.fH = HalfCarrySub(this.rD, 1);

            this.rD--;
            this.fZ = this.rD == 0;
            this.fN = true;

            return 4;
        }

        private int dec_e(ushort operand)
        {
            this.fH = HalfCarrySub(this.rE, 1);

            this.rE--;
            this.fZ = this.rE == 0;
            this.fN = true;

            return 4;
        }

        private int dec_h(ushort operand)
        {
            this.fH = HalfCarrySub(this.rH, 1);

            this.rH--;
            this.fZ = this.rH == 0;
            this.fN = true;

            return 4;
        }

        private int dec_l(ushort operand)
        {
            this.fH = HalfCarrySub(this.rL, 1);

            this.rL--;
            this.fZ = this.rL == 0;
            this.fN = true;

            return 4;
        }

        private int dec_phl(ushort operand)
        {
            this.fH = HalfCarrySub(this.Ram[this.rHL], 1);

            this.Ram[this.rHL]--;
            this.fZ = this.Ram[this.rHL] == 0;
            this.fN = true;

            return 12;
        }

        private int sub_a_a(ushort operand)
        {
            this.fH = HalfCarrySub(this.rA, this.rA);
            this.fC = false; // this.rA > this.rA;

            this.rA = 0; // -= this.rA;

            this.fZ = true; // this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_b(ushort operand)
        {
            this.fH = HalfCarrySub(this.rA, this.rB);
            this.fC = this.rB > this.rA;

            this.rA -= this.rB;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_c(ushort operand)
        {
            this.fH = HalfCarrySub(this.rA, this.rC);
            this.fC = this.rC > this.rA;

            this.rA -= this.rC;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_d(ushort operand)
        {
            this.fH = HalfCarrySub(this.rA, this.rD);
            this.fC = this.rD > this.rA;

            this.rA -= this.rD;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_e(ushort operand)
        {
            this.fH = HalfCarrySub(this.rA, this.rE);
            this.fC = this.rE > this.rA;

            this.rA -= this.rE;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_h(ushort operand)
        {
            this.fH = HalfCarrySub(this.rA, this.rH);
            this.fC = this.rH > this.rA;

            this.rA -= this.rH;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_l(ushort operand)
        {
            this.fH = HalfCarrySub(this.rA, this.rL);
            this.fC = this.rL > this.rA;

            this.rA -= this.rL;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_phl(ushort operand)
        {
            this.fH = HalfCarrySub(this.rA, this.Ram[this.rHL]);
            this.fC = this.Ram[this.rHL] > this.rA;

            this.rA -= this.Ram[this.rHL];

            this.fZ = this.rA == 0;
            this.fN = true;

            return 8;
        }

        private int sub_a_n(ushort operand)
        {
            this.fH = HalfCarrySub(this.rA, (byte)operand);
            this.fC = (byte)operand > this.rA;

            this.rA -= (byte)operand;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 8;
        }

        private int sbc_a_a(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarrySub(this.rA, this.rA, carry);
            this.fC = this.rA - this.rA - carry < 0;

            this.rA -= (byte)(this.rA + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_b(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarrySub(this.rA, this.rB, carry);
            this.fC = this.rA - this.rB - carry < 0;

            this.rA -= (byte)(this.rB + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_c(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarrySub(this.rA, this.rC, carry);
            this.fC = this.rA - this.rC - carry < 0;

            this.rA -= (byte)(this.rC + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_d(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarrySub(this.rA, this.rD, carry);
            this.fC = this.rA - this.rD - carry < 0;

            this.rA -= (byte)(this.rD + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_e(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarrySub(this.rA, this.rE, carry);
            this.fC = this.rA - this.rE - carry < 0;

            this.rA -= (byte)(this.rE + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_h(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarrySub(this.rA, this.rH, carry);
            this.fC = this.rA - this.rH - carry < 0;

            this.rA -= (byte)(this.rH + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_l(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarrySub(this.rA, this.rL, carry);
            this.fC = this.rA - this.rL - carry < 0;

            this.rA -= (byte)(this.rL + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_n(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarrySub(this.rA, (byte)operand, carry);
            this.fC = this.rA - (byte)operand - carry < 0;

            this.rA -= (byte)((byte)operand + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 8; // WAS 4
        }

        private int sbc_a_phl(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarrySub(this.rA, this.Ram[this.rHL], carry);
            this.fC = this.rA - this.Ram[this.rHL] - carry < 0;

            this.rA -= (byte)(this.Ram[this.rHL] + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 8; // WAS 4
        }

        private int cp_a(ushort operand)
        {
            this.fZ = true; // this.rA == this.rA;
            this.fN = true;
            this.fH = HalfCarrySub(this.rA, this.rA);
            this.fC = false; // this.rA < this.rA;

            return 4;
        }

        private int cp_b(ushort operand)
        {
            this.fZ = this.rA == this.rB;
            this.fN = true;
            this.fH = HalfCarrySub(this.rA, this.rB);
            this.fC = this.rA < this.rB;

            return 4;
        }

        private int cp_c(ushort operand)
        {
            this.fZ = this.rA == this.rC;
            this.fN = true;
            this.fH = HalfCarrySub(this.rA, this.rC);
            this.fC = this.rA < this.rC;

            return 4;
        }

        private int cp_d(ushort operand)
        {
            this.fZ = this.rA == this.rD;
            this.fN = true;
            this.fH = HalfCarrySub(this.rA, this.rD);
            this.fC = this.rA < this.rD;

            return 4;
        }

        private int cp_e(ushort operand)
        {
            this.fZ = this.rA == this.rE;
            this.fN = true;
            this.fH = HalfCarrySub(this.rA, this.rE);
            this.fC = this.rA < this.rE;

            return 4;
        }

        private int cp_h(ushort operand)
        {
            this.fZ = this.rA == this.rH;
            this.fN = true;
            this.fH = HalfCarrySub(this.rA, this.rH);
            this.fC = this.rA < this.rH;

            return 4;
        }

        private int cp_l(ushort operand)
        {
            this.fZ = this.rA == this.rL;
            this.fN = true;
            this.fH = HalfCarrySub(this.rA, this.rL);
            this.fC = this.rA < this.rL;

            return 4;
        }

        private int cp_phl(ushort operand)
        {
            this.fZ = this.rA == this.Ram[this.rHL];
            this.fN = true;
            this.fH = HalfCarrySub(this.rA, this.Ram[this.rHL]);
            this.fC = this.rA < this.Ram[this.rHL];

            return 8;
        }

        private int cp_n(ushort operand)
        {
            this.fZ = this.rA == (byte)operand;
            this.fN = true;
            this.fH = HalfCarrySub(this.rA, (byte)operand);
            this.fC = this.rA < (byte)operand;

            return 8;
        }

        private int dec_bc(ushort operand)
        {
            this.rBC--;
            return 8;
        }

        private int dec_de(ushort operand)
        {
            this.rDE--;
            return 8;
        }

        private int dec_hl(ushort operand)
        {
            this.rHL--;
            return 8;
        }

        private int dec_sp(ushort operand)
        {
            this.rSP--;
            return 8;
        }

        private int inc_a(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rA, 1);

            this.rA++;
            this.fZ = this.rA == 0;
            this.fN = false;

            return 4;
        }

        private int inc_b(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rB, 1);

            this.rB++;
            this.fZ = this.rB == 0;
            this.fN = false;

            return 4;
        }

        private int inc_c(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rC, 1);

            this.rC++;
            this.fZ = this.rC == 0;
            this.fN = false;

            return 4;
        }

        private int inc_d(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rD, 1);

            this.rD++;
            this.fZ = this.rD == 0;
            this.fN = false;

            return 4;
        }

        private int inc_e(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rE, 1);

            this.rE++;
            this.fZ = this.rE == 0;
            this.fN = false;

            return 4;
        }

        private int inc_h(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rH, 1);

            this.rH++;
            this.fZ = this.rH == 0;
            this.fN = false;

            return 4;
        }

        private int inc_l(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rL, 1);

            this.rL++;
            this.fZ = this.rL == 0;
            this.fN = false;

            return 4;
        }

        private int inc_phl(ushort operand)
        {
            this.fH = HalfCarryAdd(this.Ram[this.rHL], 1);

            this.Ram[this.rHL]++;
            this.fZ = this.Ram[this.rHL] == 0;
            this.fN = false;

            return 12;
        }

        private int inc_bc(ushort operand)
        {
            this.CheckOAMBug(this.rB);

            this.rBC++;
            return 8;
        }

        private int inc_de(ushort operand)
        {
            this.CheckOAMBug(this.rD);

            this.rDE++;
            return 8;
        }

        private int inc_hl(ushort operand)
        {
            this.CheckOAMBug(this.rH);

            this.rHL++;
            return 8;
        }

        private int inc_sp(ushort operand)
        {
            // Doesn't count for OAM bug for some reason?
            // this.CheckOAMBug((byte)(this.rSP >> 8));
            this.rSP++;
            return 8;
        }

        private int jr_nz_dn(ushort operand)
        {
            if (!this.fZ)
            {
                this.rPC += (ushort)(sbyte)(byte)operand;
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int jr_z_dn(ushort operand)
        {
            if (this.fZ)
            {
                this.rPC += (ushort)(sbyte)(byte)operand;
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int jr_nc_dn(ushort operand)
        {
            if (!this.fC)
            {
                this.rPC += (ushort)(sbyte)(byte)operand;
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int jr_c_dn(ushort operand)
        {
            if (this.fC)
            {
                this.rPC += (ushort)(sbyte)(byte)operand;
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int jr_dn(ushort operand)
        {
            this.rPC += (ushort)(sbyte)(byte)operand;
            return 12;
        }

        private int di(ushort operand)
        {
            this.fInterruptMasterEnable = false;
            return 4;
        }

        private int ei(ushort operand)
        {
            this.fInterruptMasterEnable = true;
            return 4;
        }

        private int ldh_pn_a(ushort operand)
        {
            this.Ram[operand + 0xFF00] = this.rA;
            return 12;
        }

        private int ldh_a_pn(ushort operand)
        {
            this.rA = this.Ram[operand + 0xFF00];
            return 12;
        }

        private int ld_a_n(ushort operand)
        {
            this.rA = (byte)operand;
            return 8;
        }

        private int ld_b_n(ushort operand)
        {
            this.rB = (byte)operand;
            return 8;
        }

        private int ld_c_n(ushort operand)
        {
            this.rC = (byte)operand;
            return 8;
        }

        private int ld_d_n(ushort operand)
        {
            this.rD = (byte)operand;
            return 8;
        }

        private int ld_e_n(ushort operand)
        {
            this.rE = (byte)operand;
            return 8;
        }

        private int ld_h_n(ushort operand)
        {
            this.rH = (byte)operand;
            return 8;
        }

        private int ld_l_n(ushort operand)
        {
            this.rL = (byte)operand;
            return 8;
        }

        private int ld_phl_n(ushort operand)
        {
            this.Ram[this.rHL] = (byte)operand;

            return 12;
        }

        private int ld_pnn_a(ushort operand)
        {
            this.Ram[operand] = this.rA;

            return 16;
        }

        private int ldi_a_phl(ushort operand)
        {
            this.rA = this.Ram[this.rHL];
            this.rHL++;

            return 8;
        }

        private int ldi_phl_a(ushort operand)
        {
            this.Ram[this.rHL] = this.rA;
            this.rHL++;

            return 8;
        }

        private int ld_pc_a(ushort operand)
        {
            this.Ram[0xFF00 + this.rC] = this.rA;

            return 8; // TODO: Was 12? "ld   (FF00+C),A"
        }

        private int ld_a_pc(ushort operand)
        {
            this.rA = this.Ram[0xFF00 + this.rC];

            return 8; // TODO: Was 12? "ld   A,(FF00+C)"
        }

        // Register copies
        private int ld_a_b(ushort operand)
        {
            this.rA = this.rB;
            return 4;
        }

        private int ld_a_c(ushort operand)
        {
            this.rA = this.rC;
            return 4;
        }

        private int ld_a_d(ushort operand)
        {
            this.rA = this.rD;
            return 4;
        }

        private int ld_a_e(ushort operand)
        {
            this.rA = this.rE;
            return 4;
        }

        private int ld_a_h(ushort operand)
        {
            this.rA = this.rH;
            return 4;
        }

        private int ld_a_l(ushort operand)
        {
            this.rA = this.rL;
            return 4;
        }

        private int ld_a_phl(ushort operand)
        {
            this.rA = this.Ram[this.rHL];
            return 8;
        }

        private int ld_a_pbc(ushort operand)
        {
            this.rA = this.Ram[this.rBC];
            return 8;
        }

        private int ld_a_pde(ushort operand)
        {
            this.rA = this.Ram[this.rDE];
            return 8;
        }

        private int ld_b_a(ushort operand)
        {
            this.rB = this.rA;
            return 4;
        }

        private int ld_b_c(ushort operand)
        {
            this.rB = this.rC;
            return 4;
        }

        private int ld_b_d(ushort operand)
        {
            this.rB = this.rD;
            return 4;
        }

        private int ld_b_e(ushort operand)
        {
            this.rB = this.rE;
            return 4;
        }

        private int ld_b_h(ushort operand)
        {
            this.rB = this.rH;
            return 4;
        }

        private int ld_b_l(ushort operand)
        {
            this.rB = this.rL;
            return 4;
        }

        private int ld_b_phl(ushort operand)
        {
            this.rB = this.Ram[this.rHL];
            return 8;
        }

        private int ld_c_a(ushort operand)
        {
            this.rC = this.rA;
            return 4;
        }

        private int ld_c_b(ushort operand)
        {
            this.rC = this.rB;
            return 4;
        }

        private int ld_c_d(ushort operand)
        {
            this.rC = this.rD;
            return 4;
        }

        private int ld_c_e(ushort operand)
        {
            this.rC = this.rE;
            return 4;
        }

        private int ld_c_h(ushort operand)
        {
            this.rC = this.rH;
            return 4;
        }

        private int ld_c_l(ushort operand)
        {
            this.rC = this.rL;
            return 4;
        }

        private int ld_c_phl(ushort operand)
        {
            this.rC = this.Ram[this.rHL];
            return 8;
        }

        private int ld_d_a(ushort operand)
        {
            this.rD = this.rA;
            return 4;
        }

        private int ld_d_b(ushort operand)
        {
            this.rD = this.rB;
            return 4;
        }

        private int ld_d_c(ushort operand)
        {
            this.rD = this.rC;
            return 4;
        }

        private int ld_d_e(ushort operand)
        {
            this.rD = this.rE;
            return 4;
        }

        private int ld_d_h(ushort operand)
        {
            this.rD = this.rH;
            return 4;
        }

        private int ld_d_l(ushort operand)
        {
            this.rD = this.rL;
            return 4;
        }

        private int ld_d_phl(ushort operand)
        {
            this.rD = this.Ram[this.rHL];
            return 8;
        }

        private int ld_e_a(ushort operand)
        {
            this.rE = this.rA;
            return 4;
        }

        private int ld_e_b(ushort operand)
        {
            this.rE = this.rB;
            return 4;
        }

        private int ld_e_c(ushort operand)
        {
            this.rE = this.rC;
            return 4;
        }

        private int ld_e_d(ushort operand)
        {
            this.rE = this.rD;
            return 4;
        }

        private int ld_e_h(ushort operand)
        {
            this.rE = this.rH;
            return 4;
        }

        private int ld_e_l(ushort operand)
        {
            this.rE = this.rL;
            return 4;
        }

        private int ld_e_phl(ushort operand)
        {
            this.rE = this.Ram[this.rHL];
            return 8;
        }

        private int ld_h_a(ushort operand)
        {
            this.rH = this.rA;
            return 4;
        }

        private int ld_h_b(ushort operand)
        {
            this.rH = this.rB;
            return 4;
        }

        private int ld_h_c(ushort operand)
        {
            this.rH = this.rC;
            return 4;
        }

        private int ld_h_d(ushort operand)
        {
            this.rH = this.rD;
            return 4;
        }

        private int ld_h_e(ushort operand)
        {
            this.rH = this.rE;
            return 4;
        }

        private int ld_h_l(ushort operand)
        {
            this.rH = this.rL;
            return 4;
        }

        private int ld_h_phl(ushort operand)
        {
            this.rH = this.Ram[this.rHL];
            return 8;
        }

        private int ld_l_a(ushort operand)
        {
            this.rL = this.rA;
            return 4;
        }

        private int ld_l_b(ushort operand)
        {
            this.rL = this.rB;
            return 4;
        }

        private int ld_l_c(ushort operand)
        {
            this.rL = this.rC;
            return 4;
        }

        private int ld_l_d(ushort operand)
        {
            this.rL = this.rD;
            return 4;
        }

        private int ld_l_e(ushort operand)
        {
            this.rL = this.rE;
            return 4;
        }

        private int ld_l_h(ushort operand)
        {
            this.rL = this.rH;
            return 4;
        }

        private int ld_l_phl(ushort operand)
        {
            this.rL = this.Ram[this.rHL];
            return 8;
        }

        private int or_a(ushort operand)
        {
            this.rA |= this.rA;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_b(ushort operand)
        {
            this.rA |= this.rB;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_c(ushort operand)
        {
            this.rA |= this.rC;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_d(ushort operand)
        {
            this.rA |= this.rD;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_e(ushort operand)
        {
            this.rA |= this.rE;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_h(ushort operand)
        {
            this.rA |= this.rH;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_l(ushort operand)
        {
            this.rA |= this.rL;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_phl(ushort operand)
        {
            this.rA |= this.Ram[this.rHL];
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int cpl(ushort operand)
        {
            this.rA = (byte)(~this.rA);
            this.fN = true;
            this.fH = true;

            return 4;
        }

        private int add_sp_dn(ushort operand)
        {
            // Weird instruction. Flags based on *unsigned* math, results based on *signed* math.
            this.fH = HalfCarryAdd((byte)this.rSP, (byte)operand);
            this.fC = (((byte)this.rSP + (byte)operand) & 0x100) == 0x100;

            // TODO: This is, uh, roundabout? Figure out a better way.
            this.rSP = (ushort)(this.rSP + (sbyte)(byte)operand);

            this.fZ = false;
            this.fN = false;

            return 16;
        }

        private int ldhl_spdn(ushort operand)
        {
            // Weird instruction. Flags based on *unsigned* math, results based on *signed* math.
            this.fH = HalfCarryAdd((byte)this.rSP, (byte)operand);
            this.fC = (((byte)this.rSP + (byte)operand) & 0x100) == 0x100;

            this.rHL = (ushort)(this.rSP + (sbyte)(byte)operand);

            this.fZ = false;
            this.fN = false;

            return 12;
        }

        private int and_a(ushort operand)
        {
            this.rA &= this.rA;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_b(ushort operand)
        {
            this.rA &= this.rB;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_c(ushort operand)
        {
            this.rA &= this.rC;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_d(ushort operand)
        {
            this.rA &= this.rD;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_e(ushort operand)
        {
            this.rA &= this.rE;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_h(ushort operand)
        {
            this.rA &= this.rH;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_l(ushort operand)
        {
            this.rA &= this.rL;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_phl(ushort operand)
        {
            this.rA &= this.Ram[this.rHL];
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 8;
        }

        private int and_n(ushort operand)
        {
            this.rA &= (byte)operand;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 8;
        }

        private int xor_n(ushort operand)
        {
            this.rA ^= (byte)operand;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int or_n(ushort operand)
        {
            this.rA |= (byte)operand;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int rst_0(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x00;
            this.rSP -= 2;
            return 16;
        }

        private int rst_8(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x08;
            this.rSP -= 2;
            return 16;
        }

        private int rst_10(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x10;
            this.rSP -= 2;
            return 16;
        }

        private int rst_18(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x18;
            this.rSP -= 2;
            return 16;
        }

        private int rst_20(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x20;
            this.rSP -= 2;
            return 16;
        }

        private int rst_28(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x28;
            this.rSP -= 2;
            return 16;
        }

        private int rst_30(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x30;
            this.rSP -= 2;
            return 16;
        }

        private int rst_38(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x38;
            this.rSP -= 2;
            return 16;
        }

        private int add_a_a(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rA, this.rA);

            int result = this.rA + this.rA;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_b(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rA, this.rB);

            int result = this.rA + this.rB;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_c(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rA, this.rC);

            int result = this.rA + this.rC;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_d(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rA, this.rD);

            int result = this.rA + this.rD;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_e(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rA, this.rE);

            int result = this.rA + this.rE;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_h(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rA, this.rH);

            int result = this.rA + this.rH;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_l(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rA, this.rL);

            int result = this.rA + this.rL;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_phl(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rA, this.Ram[this.rHL]);

            int result = this.rA + this.Ram[this.rHL];
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 8;
        }

        private int add_a_n(ushort operand)
        {
            this.fH = HalfCarryAdd(this.rA, (byte)operand);

            int result = this.rA + (byte)operand;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 8;
        }

        private int adc_a_a(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarryAdd(this.rA, this.rA, carry);

            int result = this.rA + this.rA + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_b(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarryAdd(this.rA, this.rB, carry);

            int result = this.rA + this.rB + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_c(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarryAdd(this.rA, this.rC, carry);

            int result = this.rA + this.rC + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_d(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarryAdd(this.rA, this.rD, carry);

            int result = this.rA + this.rD + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_e(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarryAdd(this.rA, this.rE, carry);

            int result = this.rA + this.rE + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_h(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarryAdd(this.rA, this.rH, carry);

            int result = this.rA + this.rH + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_l(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarryAdd(this.rA, this.rL, carry);

            int result = this.rA + this.rL + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_n(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarryAdd(this.rA, (byte)operand, carry);

            int result = this.rA + (byte)operand + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 8;
        }

        private int adc_a_phl(ushort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = HalfCarryAdd(this.rA, this.Ram[this.rHL], carry);

            int result = this.rA + this.Ram[this.rHL] + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 8;
        }

        private int push_bc(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rBC);
            this.rSP -= 2;
            return 16;
        }

        private int push_de(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rDE);
            this.rSP -= 2;
            return 16;
        }

        private int push_hl(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rHL);
            this.rSP -= 2;
            return 16;
        }

        private int push_af(ushort operand)
        {
            this.Ram.SetShort(this.rSP - 2, this.rAF);
            this.rSP -= 2;
            return 16;
        }

        private int pop_bc(ushort operand)
        {
            this.rBC = (ushort)this.Ram.GetShort(this.rSP);
            this.rSP += 2;
            return 12;
        }

        private int pop_de(ushort operand)
        {
            this.rDE = (ushort)this.Ram.GetShort(this.rSP);
            this.rSP += 2;
            return 12;
        }

        private int pop_hl(ushort operand)
        {
            this.rHL = (ushort)this.Ram.GetShort(this.rSP);
            this.rSP += 2;
            return 12;
        }

        private int pop_af(ushort operand)
        {
            // Special case, lower four bits of F are always 0
            this.rAF = (ushort)(this.Ram.GetShort(this.rSP) & 0xFFF0);

            this.rSP += 2;
            return 12;
        }

        private int add_hl_bc(ushort operand)
        {
            this.fH = HalfCarryAdd((ushort)this.rHL, (ushort)this.rBC);

            int result = this.rHL + this.rBC;
            this.rHL = result;
            this.fN = false;
            this.fC = (result & 0x10000) == 0x10000;

            return 8;
        }

        private int add_hl_de(ushort operand)
        {
            this.fH = HalfCarryAdd((ushort)this.rHL, (ushort)this.rDE);

            int result = this.rHL + this.rDE;
            this.rHL = result;
            this.fN = false;
            this.fC = (result & 0x10000) == 0x10000;

            return 8;
        }

        private int add_hl_hl(ushort operand)
        {
            this.fH = HalfCarryAdd((ushort)this.rHL, (ushort)this.rHL);

            int result = this.rHL + this.rHL;
            this.rHL = result;
            this.fN = false;
            this.fC = (result & 0x10000) == 0x10000;

            return 8;
        }

        private int add_hl_sp(ushort operand)
        {
            this.fH = HalfCarryAdd((ushort)this.rHL, (ushort)this.rSP);

            // fH = ((rHL & 0xFFF) + (rSP & 0xFFF)) > 0xFFF;
            int result = this.rHL + this.rSP;
            this.rHL = result;
            this.fN = false;
            this.fC = result > 0xFFFF;

            return 8;
        }

        private int jp_hl(ushort operand)
        {
            this.rPC = (ushort)this.rHL;
            return 4;
        }

        private int rlc_a(ushort operand)
        {
            this.fC = (this.rA & 0x80) == 0x80;

            this.rA = (byte)(this.rA << 1);
            if (this.fC)
            {
                this.rA |= 0x01;
            }

            this.fZ = false;
            this.fN = false;
            this.fH = false;

            return 4;
        }

        private int rrc_a(ushort operand)
        {
            this.fC = (this.rA & 0x01) == 0x01;

            this.rA = (byte)(this.rA >> 1);
            if (this.fC)
            {
                this.rA |= 0x80;
            }

            this.fZ = false;
            this.fN = false;
            this.fH = false;

            return 4;
        }

        private int stop(ushort operand)
        {
            this.fStop = true;

            // Work around buggy software? ROMs will hard-lock the gameboy if they halt with interrupts disabled. Blargg's cpu_instrs test appears to do this right before test 3.
            if (this.rInterruptEnable == 0)
            {
                this.rInterruptEnable = 0x1f;
            }

            if (this.fPrepareSwitch)
            {
                // TODO: How many clocks to add here?
                this.fSpeed = !this.fSpeed;
                this.fPrepareSwitch = false;
                this.fStop = false;
            }

            return 0; // WAS 4
        }

        private int halt(ushort operand)
        {
            this.fHalt = true;

            // Work around buggy software? ROMs will hard-lock the gameboy if they halt with interrupts disabled.
            /*if (this.rInterruptEnable == 0)
            {
                this.rInterruptEnable = 0x1f;
            }*/

            if (!this.fInterruptMasterEnable && (this.rInterruptEnable & this.rInterruptFlags & 0x1f) != 0)
            {
                // HALT bug. Just disabling halt isn't accurate, but is probably close enough
                this.fHaltBug = true;
            }

            return 0; // WAS 4
        }

        private int daa(ushort operand)
        {
            ushort a = this.rA;

            if (!this.fN)
            {
                if (this.fH || (a & 0x0F) > 9)
                {
                    a += 6;
                }

                if (this.fC || a > 0x9F)
                {
                    a += 0x60;
                }
            }
            else
            {
                if (this.fH)
                {
                    a = (ushort)((a - 6) & 0xFF);
                }

                if (this.fC)
                {
                    a -= 0x60;
                }
            }

            this.rF = (byte)(this.rF & ~(0x20 | 0x80));
            if ((a & 0x100) != 0)
            {
                this.rF |= 0x10;
            }

            a &= 0xFF;
            if (a == 0)
            {
                this.rF |= 0x80;
            }

            this.rA = (byte)a;

            return 4;
        }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1300 // Element should begin with upper-case letter
    }
}