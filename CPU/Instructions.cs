namespace GusBoy
{
    public partial class CPU
    {
        private bool halfCarrySub(byte target, byte value, int carry = 0) => (((target & 0xF) - (value & 0xF) - carry) < 0);

        private bool halfCarryAdd(byte target, byte value, int carry = 0) => ((target & 0xF) + (value & 0xF) + carry) > 0xF;

        private bool halfCarryAdd(gshort target, gshort value) => ((target & 0xFFF) + (value & 0xFFF)) > 0xFFF;

        private int ext(gshort operand) => this.extendedOpcodes[operand.Lo].func();

        private int nop(gshort operand) => 4;

        private int jp_nn(gshort operand)
        {
            this.rPC = operand;
            return 16;
        }

        private int jp_nz_nn(gshort operand)
        {
            if ( !this.fZ )
            {
                this.rPC = operand;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private int jp_z_nn(gshort operand)
        {
            if ( this.fZ )
            {
                this.rPC = operand;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private int jp_nc_nn(gshort operand)
        {
            if ( !this.fC )
            {
                this.rPC = operand;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private int jp_c_nn(gshort operand)
        {
            if ( this.fC )
            {
                this.rPC = operand;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private int call_nn(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = operand;
            this.rSP -= 2;
            return 24;
        }

        private int call_nz_nn(gshort operand)
        {
            if ( !this.fZ )
            {
                this.ram.SetShort(this.rSP - 2, this.rPC);
                this.rPC = operand;
                this.rSP -= 2;
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private int call_z_nn(gshort operand)
        {
            if ( this.fZ )
            {
                this.ram.SetShort(this.rSP - 2, this.rPC);
                this.rPC = operand;
                this.rSP -= 2;
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private int call_nc_nn(gshort operand)
        {
            if ( !this.fC )
            {
                this.ram.SetShort(this.rSP - 2, this.rPC);
                this.rPC = operand;
                this.rSP -= 2;
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private int call_c_nn(gshort operand)
        {
            if ( this.fC )
            {
                this.ram.SetShort(this.rSP - 2, this.rPC);
                this.rPC = operand;
                this.rSP -= 2;
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private int rl_a(gshort operand)
        {
            bool oldFC = this.fC;

            this.fC = (this.rA & 0x80) == 0x80;
            this.rA = (byte)(this.rA << 1);
            if ( oldFC )
            {
                this.rA |= 0x01;
            }

            this.fZ = false;
            this.fN = false;
            this.fH = false;

            return 4;
        }

        private int rr_a(gshort operand)
        {
            bool oldFC = this.fC;

            this.fC = (this.rA & 0x01) == 0x01;
            this.rA = (byte)(this.rA >> 1);
            if ( oldFC )
            {
                this.rA |= 0x80;
            }

            this.fZ = false;
            this.fN = false;
            this.fH = false;

            return 4;
        }

        private int scf(gshort operand)
        {
            this.fN = false;
            this.fH = false;
            this.fC = true;

            return 4;
        }

        private int ccf(gshort operand)
        {
            this.fN = false;
            this.fH = false;
            this.fC = !this.fC;

            return 4;
        }

        private int ret(gshort operand)
        {
            this.rPC = this.ram.GetShort(this.rSP);
            this.rSP += 2;
            return 16;
        }

        private int reti(gshort operand)
        {
            this.rPC = this.ram.GetShort(this.rSP);
            this.rSP += 2;
            this.fInterruptMasterEnable = true;
            return 16;
        }

        private int ret_nz(gshort operand)
        {
            if ( !this.fZ )
            {
                this.rPC = this.ram.GetShort(this.rSP);
                this.rSP += 2;
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private int ret_z(gshort operand)
        {
            if ( this.fZ )
            {
                this.rPC = this.ram.GetShort(this.rSP);
                this.rSP += 2;
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private int ret_nc(gshort operand)
        {
            if ( !this.fC )
            {
                this.rPC = this.ram.GetShort(this.rSP);
                this.rSP += 2;
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private int ret_c(gshort operand)
        {
            if ( this.fC )
            {
                this.rPC = this.ram.GetShort(this.rSP);
                this.rSP += 2;
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private int xor_a(gshort operand)
        {
            this.rA ^= this.rA;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_b(gshort operand)
        {
            this.rA ^= this.rB;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_c(gshort operand)
        {
            this.rA ^= this.rC;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_d(gshort operand)
        {
            this.rA ^= this.rD;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_e(gshort operand)
        {
            this.rA ^= this.rE;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_h(gshort operand)
        {
            this.rA ^= this.rH;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_l(gshort operand)
        {
            this.rA ^= this.rL;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int xor_phl(gshort operand)
        {
            this.rA ^= this.ram[this.rHL];
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int ld_bc_nn(gshort operand)
        {
            this.rBC = (ushort)operand;
            return 12;
        }

        private int ld_de_nn(gshort operand)
        {
            this.rDE = (ushort)operand;
            return 12;
        }

        private int ld_hl_nn(gshort operand)
        {
            this.rHL = (ushort)operand;
            return 12;
        }

        private int ld_sp_nn(gshort operand)
        {
            this.rSP = (ushort)operand;
            return 12;
        }

        private int ld_pnn_sp(gshort operand)
        {
            this.ram.SetShort(operand, this.rSP); // TODO: Check that SetShort works here
            return 20;
        }

        private int ld_sp_hl(gshort operand)
        {
            this.rSP = (ushort)this.rHL;
            return 8;
        }

        private int ld_a_pnn(gshort operand)
        {
            this.rA = this.ram[operand];
            return 16;
        }

        private int ldd_phl_a(gshort operand)
        {
            this.ram[this.rHL] = this.rA;
            this.rHL--;
            return 8;
        }

        private int ldd_a_phl(gshort operand)
        {
            this.rA = this.ram[this.rHL];
            this.rHL--;
            return 8;
        }

        private int ld_pbc_a(gshort operand)
        {
            this.ram[this.rBC] = this.rA;
            return 8;
        }

        private int ld_pde_a(gshort operand)
        {
            this.ram[this.rDE] = this.rA;
            return 8;
        }

        private int ld_phl_a(gshort operand)
        {
            this.ram[this.rHL] = this.rA;
            return 8;
        }

        private int ld_phl_b(gshort operand)
        {
            this.ram[this.rHL] = this.rB;
            return 8;
        }

        private int ld_phl_c(gshort operand)
        {
            this.ram[this.rHL] = this.rC;
            return 8;
        }

        private int ld_phl_d(gshort operand)
        {
            this.ram[this.rHL] = this.rD;
            return 8;
        }

        private int ld_phl_e(gshort operand)
        {
            this.ram[this.rHL] = this.rE;
            return 8;
        }

        private int ld_phl_h(gshort operand)
        {
            this.ram[this.rHL] = this.rH;
            return 8;
        }

        private int ld_phl_l(gshort operand)
        {
            this.ram[this.rHL] = this.rL;
            return 8;
        }

        private int dec_a(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rA, 1);

            this.rA--;
            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int dec_b(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rB, 1);

            this.rB--;
            this.fZ = this.rB == 0;
            this.fN = true;

            return 4;
        }

        private int dec_c(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rC, 1);

            this.rC--;
            this.fZ = this.rC == 0;
            this.fN = true;

            return 4;
        }

        private int dec_d(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rD, 1);

            this.rD--;
            this.fZ = this.rD == 0;
            this.fN = true;

            return 4;
        }

        private int dec_e(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rE, 1);

            this.rE--;
            this.fZ = this.rE == 0;
            this.fN = true;

            return 4;
        }

        private int dec_h(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rH, 1);

            this.rH--;
            this.fZ = this.rH == 0;
            this.fN = true;

            return 4;
        }

        private int dec_l(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rL, 1);

            this.rL--;
            this.fZ = this.rL == 0;
            this.fN = true;

            return 4;
        }

        private int dec_phl(gshort operand)
        {
            this.fH = this.halfCarrySub(this.ram[this.rHL], 1);

            this.ram[this.rHL]--;
            this.fZ = this.ram[this.rHL] == 0;
            this.fN = true;

            return 12;
        }

        private int sub_a_a(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rA, this.rA);
            this.fC = this.rA > this.rA;

            this.rA -= this.rA;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_b(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rA, this.rB);
            this.fC = this.rB > this.rA;

            this.rA -= this.rB;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_c(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rA, this.rC);
            this.fC = this.rC > this.rA;

            this.rA -= this.rC;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_d(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rA, this.rD);
            this.fC = this.rD > this.rA;

            this.rA -= this.rD;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_e(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rA, this.rE);
            this.fC = this.rE > this.rA;

            this.rA -= this.rE;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_h(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rA, this.rH);
            this.fC = this.rH > this.rA;

            this.rA -= this.rH;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_l(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rA, this.rL);
            this.fC = this.rL > this.rA;

            this.rA -= this.rL;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sub_a_phl(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rA, this.ram[this.rHL]);
            this.fC = this.ram[this.rHL] > this.rA;

            this.rA -= this.ram[this.rHL];

            this.fZ = this.rA == 0;
            this.fN = true;

            return 8;
        }

        private int sub_a_n(gshort operand)
        {
            this.fH = this.halfCarrySub(this.rA, operand.Lo);
            this.fC = operand.Lo > this.rA;

            this.rA -= operand.Lo;

            this.fZ = this.rA == 0;
            this.fN = true;

            return 8;
        }

        private int sbc_a_a(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarrySub(this.rA, this.rA, carry);
            this.fC = this.rA - this.rA - carry < 0;

            this.rA -= (byte)(this.rA + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_b(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarrySub(this.rA, this.rB, carry);
            this.fC = this.rA - this.rB - carry < 0;

            this.rA -= (byte)(this.rB + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_c(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarrySub(this.rA, this.rC, carry);
            this.fC = this.rA - this.rC - carry < 0;

            this.rA -= (byte)(this.rC + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_d(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarrySub(this.rA, this.rD, carry);
            this.fC = this.rA - this.rD - carry < 0;

            this.rA -= (byte)(this.rD + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_e(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarrySub(this.rA, this.rE, carry);
            this.fC = this.rA - this.rE - carry < 0;

            this.rA -= (byte)(this.rE + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_h(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarrySub(this.rA, this.rH, carry);
            this.fC = this.rA - this.rH - carry < 0;

            this.rA -= (byte)(this.rH + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_l(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarrySub(this.rA, this.rL, carry);
            this.fC = this.rA - this.rL - carry < 0;

            this.rA -= (byte)(this.rL + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 4;
        }

        private int sbc_a_n(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarrySub(this.rA, operand.Lo, carry);
            this.fC = this.rA - operand.Lo - carry < 0;

            this.rA -= (byte)(operand.Lo + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 8; // WAS 4
        }

        private int sbc_a_phl(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarrySub(this.rA, this.ram[this.rHL], carry);
            this.fC = this.rA - this.ram[this.rHL] - carry < 0;

            this.rA -= (byte)(this.ram[this.rHL] + carry);

            this.fZ = this.rA == 0;
            this.fN = true;

            return 8; // WAS 4
        }

        private int cp_a(gshort operand)
        {
            this.fZ = this.rA == this.rA;
            this.fN = true;
            this.fH = this.halfCarrySub(this.rA, this.rA);
            this.fC = this.rA < this.rA;

            return 4;
        }

        private int cp_b(gshort operand)
        {
            this.fZ = this.rA == this.rB;
            this.fN = true;
            this.fH = this.halfCarrySub(this.rA, this.rB);
            this.fC = this.rA < this.rB;

            return 4;
        }

        private int cp_c(gshort operand)
        {
            this.fZ = this.rA == this.rC;
            this.fN = true;
            this.fH = this.halfCarrySub(this.rA, this.rC);
            this.fC = this.rA < this.rC;

            return 4;
        }

        private int cp_d(gshort operand)
        {
            this.fZ = this.rA == this.rD;
            this.fN = true;
            this.fH = this.halfCarrySub(this.rA, this.rD);
            this.fC = this.rA < this.rD;

            return 4;
        }

        private int cp_e(gshort operand)
        {
            this.fZ = this.rA == this.rE;
            this.fN = true;
            this.fH = this.halfCarrySub(this.rA, this.rE);
            this.fC = this.rA < this.rE;

            return 4;
        }

        private int cp_h(gshort operand)
        {
            this.fZ = this.rA == this.rH;
            this.fN = true;
            this.fH = this.halfCarrySub(this.rA, this.rH);
            this.fC = this.rA < this.rH;

            return 4;
        }

        private int cp_l(gshort operand)
        {
            this.fZ = this.rA == this.rL;
            this.fN = true;
            this.fH = this.halfCarrySub(this.rA, this.rL);
            this.fC = this.rA < this.rL;

            return 4;
        }

        private int cp_phl(gshort operand)
        {
            this.fZ = this.rA == this.ram[this.rHL];
            this.fN = true;
            this.fH = this.halfCarrySub(this.rA, this.ram[this.rHL]);
            this.fC = this.rA < this.ram[this.rHL];

            return 8;
        }

        private int cp_n(gshort operand)
        {
            this.fZ = this.rA == operand.Lo;
            this.fN = true;
            this.fH = this.halfCarrySub(this.rA, operand.Lo);
            this.fC = this.rA < operand.Lo;

            return 8;
        }

        private int dec_bc(gshort operand)
        {
            this.rBC--;
            return 8;
        }

        private int dec_de(gshort operand)
        {
            this.rDE--;
            return 8;
        }

        private int dec_hl(gshort operand)
        {
            this.rHL--;
            return 8;
        }

        private int dec_sp(gshort operand)
        {
            this.rSP--;
            return 8;
        }

        private int inc_a(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rA, 1);

            this.rA++;
            this.fZ = this.rA == 0;
            this.fN = false;

            return 4;
        }

        private int inc_b(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rB, 1);

            this.rB++;
            this.fZ = this.rB == 0;
            this.fN = false;

            return 4;
        }

        private int inc_c(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rC, 1);

            this.rC++;
            this.fZ = this.rC == 0;
            this.fN = false;

            return 4;
        }

        private int inc_d(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rD, 1);

            this.rD++;
            this.fZ = this.rD == 0;
            this.fN = false;

            return 4;
        }

        private int inc_e(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rE, 1);

            this.rE++;
            this.fZ = this.rE == 0;
            this.fN = false;

            return 4;
        }

        private int inc_h(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rH, 1);

            this.rH++;
            this.fZ = this.rH == 0;
            this.fN = false;

            return 4;
        }

        private int inc_l(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rL, 1);

            this.rL++;
            this.fZ = this.rL == 0;
            this.fN = false;

            return 4;
        }

        private int inc_phl(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.ram[this.rHL], 1);

            this.ram[this.rHL]++;
            this.fZ = this.ram[this.rHL] == 0;
            this.fN = false;

            return 12;
        }

        private int inc_bc(gshort operand)
        {
            this.rBC++;
            return 8;
        }
        private int inc_de(gshort operand)
        {
            this.rDE++;
            return 8;
        }
        private int inc_hl(gshort operand)
        {
            this.rHL++;
            return 8;
        }
        private int inc_sp(gshort operand)
        {
            this.rSP++;
            return 8;
        }

        private int jr_nz_dn(gshort operand)
        {
            if ( !this.fZ )
            {
                this.rPC += (sbyte)operand.Lo;
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int jr_z_dn(gshort operand)
        {
            if ( this.fZ )
            {
                this.rPC += (sbyte)operand.Lo;
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int jr_nc_dn(gshort operand)
        {
            if ( !this.fC )
            {
                this.rPC += (sbyte)operand.Lo;
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int jr_c_dn(gshort operand)
        {
            if ( this.fC )
            {
                this.rPC += (sbyte)operand.Lo;
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int jr_dn(gshort operand)
        {
            this.rPC += (sbyte)operand.Lo;
            return 12;
        }

        private int di(gshort operand)
        {
            this.fInterruptMasterEnable = false;
            return 4;
        }

        private int ei(gshort operand)
        {
            this.fInterruptMasterEnable = true;
            return 4;
        }

        private int ldh_pn_a(gshort operand)
        {
            this.ram[0xFF00 + operand] = this.rA;
            return 12;
        }

        private int ldh_a_pn(gshort operand)
        {
            this.rA = this.ram[0xFF00 + operand];
            return 12;
        }

        private int ld_a_n(gshort operand)
        {
            this.rA = operand.Lo;
            return 8;
        }

        private int ld_b_n(gshort operand)
        {
            this.rB = operand.Lo;
            return 8;
        }

        private int ld_c_n(gshort operand)
        {
            this.rC = operand.Lo;
            return 8;
        }

        private int ld_d_n(gshort operand)
        {
            this.rD = operand.Lo;
            return 8;
        }

        private int ld_e_n(gshort operand)
        {
            this.rE = operand.Lo;
            return 8;
        }

        private int ld_h_n(gshort operand)
        {
            this.rH = operand.Lo;
            return 8;
        }

        private int ld_l_n(gshort operand)
        {
            this.rL = operand.Lo;
            return 8;
        }

        private int ld_phl_n(gshort operand)
        {
            this.ram[this.rHL] = operand.Lo;

            return 12;
        }

        private int ld_pnn_a(gshort operand)
        {
            this.ram[operand] = this.rA;

            return 16;
        }

        private int ldi_a_phl(gshort operand)
        {
            this.rA = this.ram[this.rHL];
            this.rHL++;

            return 8;
        }

        private int ldi_phl_a(gshort operand)
        {
            this.ram[this.rHL] = this.rA;
            this.rHL++;

            return 8;
        }

        private int ld_pc_a(gshort operand)
        {
            this.ram[0xFF00 + this.rC] = this.rA;

            return 8; // TODO: Was 12? "ld   (FF00+C),A"
        }

        private int ld_a_pc(gshort operand)
        {
            this.rA = this.ram[0xFF00 + this.rC];

            return 8; // TODO: Was 12? "ld   A,(FF00+C)"
        }

        // Register copies
        private int ld_a_b(gshort operand)
        {
            this.rA = this.rB;
            return 4;
        }

        private int ld_a_c(gshort operand)
        {
            this.rA = this.rC;
            return 4;
        }

        private int ld_a_d(gshort operand)
        {
            this.rA = this.rD;
            return 4;
        }

        private int ld_a_e(gshort operand)
        {
            this.rA = this.rE;
            return 4;
        }

        private int ld_a_h(gshort operand)
        {
            this.rA = this.rH;
            return 4;
        }

        private int ld_a_l(gshort operand)
        {
            this.rA = this.rL;
            return 4;
        }

        private int ld_a_phl(gshort operand)
        {
            this.rA = this.ram[this.rHL];
            return 8;
        }

        private int ld_a_pbc(gshort operand)
        {
            this.rA = this.ram[this.rBC];
            return 8;
        }

        private int ld_a_pde(gshort operand)
        {
            this.rA = this.ram[this.rDE];
            return 8;
        }


        private int ld_b_a(gshort operand)
        {
            this.rB = this.rA;
            return 4;
        }

        private int ld_b_c(gshort operand)
        {
            this.rB = this.rC;
            return 4;
        }

        private int ld_b_d(gshort operand)
        {
            this.rB = this.rD;
            return 4;
        }

        private int ld_b_e(gshort operand)
        {
            this.rB = this.rE;
            return 4;
        }

        private int ld_b_h(gshort operand)
        {
            this.rB = this.rH;
            return 4;
        }

        private int ld_b_l(gshort operand)
        {
            this.rB = this.rL;
            return 4;
        }

        private int ld_b_phl(gshort operand)
        {
            this.rB = this.ram[this.rHL];
            return 8;
        }


        private int ld_c_a(gshort operand)
        {
            this.rC = this.rA;
            return 4;
        }

        private int ld_c_b(gshort operand)
        {
            this.rC = this.rB;
            return 4;
        }

        private int ld_c_d(gshort operand)
        {
            this.rC = this.rD;
            return 4;
        }

        private int ld_c_e(gshort operand)
        {
            this.rC = this.rE;
            return 4;
        }

        private int ld_c_h(gshort operand)
        {
            this.rC = this.rH;
            return 4;
        }

        private int ld_c_l(gshort operand)
        {
            this.rC = this.rL;
            return 4;
        }

        private int ld_c_phl(gshort operand)
        {
            this.rC = this.ram[this.rHL];
            return 8;
        }


        private int ld_d_a(gshort operand)
        {
            this.rD = this.rA;
            return 4;
        }

        private int ld_d_b(gshort operand)
        {
            this.rD = this.rB;
            return 4;
        }

        private int ld_d_c(gshort operand)
        {
            this.rD = this.rC;
            return 4;
        }

        private int ld_d_e(gshort operand)
        {
            this.rD = this.rE;
            return 4;
        }

        private int ld_d_h(gshort operand)
        {
            this.rD = this.rH;
            return 4;
        }

        private int ld_d_l(gshort operand)
        {
            this.rD = this.rL;
            return 4;
        }

        private int ld_d_phl(gshort operand)
        {
            this.rD = this.ram[this.rHL];
            return 8;
        }


        private int ld_e_a(gshort operand)
        {
            this.rE = this.rA;
            return 4;
        }

        private int ld_e_b(gshort operand)
        {
            this.rE = this.rB;
            return 4;
        }

        private int ld_e_c(gshort operand)
        {
            this.rE = this.rC;
            return 4;
        }

        private int ld_e_d(gshort operand)
        {
            this.rE = this.rD;
            return 4;
        }

        private int ld_e_h(gshort operand)
        {
            this.rE = this.rH;
            return 4;
        }

        private int ld_e_l(gshort operand)
        {
            this.rE = this.rL;
            return 4;
        }

        private int ld_e_phl(gshort operand)
        {
            this.rE = this.ram[this.rHL];
            return 8;
        }


        private int ld_h_a(gshort operand)
        {
            this.rH = this.rA;
            return 4;
        }

        private int ld_h_b(gshort operand)
        {
            this.rH = this.rB;
            return 4;
        }

        private int ld_h_c(gshort operand)
        {
            this.rH = this.rC;
            return 4;
        }

        private int ld_h_d(gshort operand)
        {
            this.rH = this.rD;
            return 4;
        }

        private int ld_h_e(gshort operand)
        {
            this.rH = this.rE;
            return 4;
        }

        private int ld_h_l(gshort operand)
        {
            this.rH = this.rL;
            return 4;
        }

        private int ld_h_phl(gshort operand)
        {
            this.rH = this.ram[this.rHL];
            return 8;
        }


        private int ld_l_a(gshort operand)
        {
            this.rL = this.rA;
            return 4;
        }

        private int ld_l_b(gshort operand)
        {
            this.rL = this.rB;
            return 4;
        }

        private int ld_l_c(gshort operand)
        {
            this.rL = this.rC;
            return 4;
        }

        private int ld_l_d(gshort operand)
        {
            this.rL = this.rD;
            return 4;
        }

        private int ld_l_e(gshort operand)
        {
            this.rL = this.rE;
            return 4;
        }

        private int ld_l_h(gshort operand)
        {
            this.rL = this.rH;
            return 4;
        }

        private int ld_l_phl(gshort operand)
        {
            this.rL = this.ram[this.rHL];
            return 8;
        }


        private int or_a(gshort operand)
        {
            this.rA |= this.rA;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_b(gshort operand)
        {
            this.rA |= this.rB;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_c(gshort operand)
        {
            this.rA |= this.rC;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_d(gshort operand)
        {
            this.rA |= this.rD;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_e(gshort operand)
        {
            this.rA |= this.rE;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_h(gshort operand)
        {
            this.rA |= this.rH;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_l(gshort operand)
        {
            this.rA |= this.rL;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 4;
        }

        private int or_phl(gshort operand)
        {
            this.rA |= this.ram[this.rHL];
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int cpl(gshort operand)
        {
            this.rA = (byte)(~this.rA);
            this.fN = true;
            this.fH = true;

            return 4;
        }

        private int add_sp_dn(gshort operand)
        {
            // Weird instruction. Flags based on *unsigned* math, results based on *signed* math.
            this.fH = this.halfCarryAdd(this.rSP.Lo, operand.Lo);
            this.fC = ((this.rSP.Lo + operand.Lo) & 0x100) == 0x100;

            this.rSP += (sbyte)operand.Lo;

            this.fZ = false;
            this.fN = false;

            return 16;
        }

        private int ldhl_spdn(gshort operand)
        {
            // Weird instruction. Flags based on *unsigned* math, results based on *signed* math.
            this.fH = this.halfCarryAdd(this.rSP.Lo, operand.Lo);
            this.fC = ((this.rSP.Lo + operand.Lo) & 0x100) == 0x100;

            this.rHL = this.rSP + (sbyte)operand.Lo;

            this.fZ = false;
            this.fN = false;

            return 12;
        }

        private int and_a(gshort operand)
        {
            this.rA &= this.rA;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_b(gshort operand)
        {
            this.rA &= this.rB;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_c(gshort operand)
        {
            this.rA &= this.rC;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_d(gshort operand)
        {
            this.rA &= this.rD;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_e(gshort operand)
        {
            this.rA &= this.rE;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_h(gshort operand)
        {
            this.rA &= this.rH;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_l(gshort operand)
        {
            this.rA &= this.rL;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 4;
        }

        private int and_phl(gshort operand)
        {
            this.rA &= this.ram[this.rHL];
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 8;
        }

        private int and_n(gshort operand)
        {
            this.rA &= operand.Lo;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = true;
            this.fC = false;

            return 8;
        }

        private int xor_n(gshort operand)
        {
            this.rA ^= operand.Lo;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int or_n(gshort operand)
        {
            this.rA |= operand.Lo;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int rst_0(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x00;
            this.rSP -= 2;
            return 16;
        }

        private int rst_8(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x08;
            this.rSP -= 2;
            return 16;
        }

        private int rst_10(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x10;
            this.rSP -= 2;
            return 16;
        }

        private int rst_18(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x18;
            this.rSP -= 2;
            return 16;
        }

        private int rst_20(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x20;
            this.rSP -= 2;
            return 16;
        }

        private int rst_28(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x28;
            this.rSP -= 2;
            return 16;
        }

        private int rst_30(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x30;
            this.rSP -= 2;
            return 16;
        }

        private int rst_38(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rPC);
            this.rPC = 0x38;
            this.rSP -= 2;
            return 16;
        }

        private int add_a_a(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rA, this.rA);

            int result = this.rA + this.rA;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_b(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rA, this.rB);

            int result = this.rA + this.rB;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_c(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rA, this.rC);

            int result = this.rA + this.rC;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_d(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rA, this.rD);

            int result = this.rA + this.rD;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_e(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rA, this.rE);

            int result = this.rA + this.rE;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_h(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rA, this.rH);

            int result = this.rA + this.rH;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_l(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rA, this.rL);

            int result = this.rA + this.rL;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 4;
        }

        private int add_a_phl(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rA, this.ram[this.rHL]);

            int result = this.rA + this.ram[this.rHL];
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 8;
        }

        private int add_a_n(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rA, operand.Lo);

            int result = this.rA + operand.Lo;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = (result & 0x100) == 0x100;

            return 8;
        }

        private int adc_a_a(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarryAdd(this.rA, this.rA, carry);

            int result = this.rA + this.rA + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_b(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarryAdd(this.rA, this.rB, carry);

            int result = this.rA + this.rB + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_c(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarryAdd(this.rA, this.rC, carry);

            int result = this.rA + this.rC + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_d(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarryAdd(this.rA, this.rD, carry);

            int result = this.rA + this.rD + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_e(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarryAdd(this.rA, this.rE, carry);

            int result = this.rA + this.rE + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_h(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarryAdd(this.rA, this.rH, carry);

            int result = this.rA + this.rH + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_l(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarryAdd(this.rA, this.rL, carry);

            int result = this.rA + this.rL + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 4;
        }

        private int adc_a_n(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarryAdd(this.rA, operand.Lo, carry);

            int result = this.rA + operand.Lo + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 8;
        }

        private int adc_a_phl(gshort operand)
        {
            int carry = this.fC ? 1 : 0;
            this.fH = this.halfCarryAdd(this.rA, this.ram[this.rHL], carry);

            int result = this.rA + this.ram[this.rHL] + carry;
            this.rA = (byte)result;
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fC = result > 0xFF;

            return 8;
        }

        private int push_bc(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rBC);
            this.rSP -= 2;
            return 16;
        }

        private int push_de(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rDE);
            this.rSP -= 2;
            return 16;
        }

        private int push_hl(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rHL);
            this.rSP -= 2;
            return 16;
        }

        private int push_af(gshort operand)
        {
            this.ram.SetShort(this.rSP - 2, this.rAF);
            this.rSP -= 2;
            return 16;
        }

        private int pop_bc(gshort operand)
        {
            this.rBC = (ushort)this.ram.GetShort(this.rSP);
            this.rSP += 2;
            return 12;
        }

        private int pop_de(gshort operand)
        {
            this.rDE = (ushort)this.ram.GetShort(this.rSP);
            this.rSP += 2;
            return 12;
        }

        private int pop_hl(gshort operand)
        {
            this.rHL = (ushort)this.ram.GetShort(this.rSP);
            this.rSP += 2;
            return 12;
        }

        private int pop_af(gshort operand)
        {
            // Special case, lower four bits of F are always 0
            this.rAF = this.ram.GetShort(this.rSP) & 0xFFF0;

            this.rSP += 2;
            return 12;
        }

        private int add_hl_bc(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rHL, this.rBC);

            int result = this.rHL + this.rBC;
            this.rHL = result;
            this.fN = false;
            this.fC = (result & 0x10000) == 0x10000;

            return 8;
        }

        private int add_hl_de(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rHL, this.rDE);

            int result = this.rHL + this.rDE;
            this.rHL = result;
            this.fN = false;
            this.fC = (result & 0x10000) == 0x10000;

            return 8;
        }

        private int add_hl_hl(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rHL, this.rHL);

            int result = this.rHL + this.rHL;
            this.rHL = result;
            this.fN = false;
            this.fC = (result & 0x10000) == 0x10000;

            return 8;
        }

        private int add_hl_sp(gshort operand)
        {
            this.fH = this.halfCarryAdd(this.rHL, this.rSP);
            //fH = ((rHL & 0xFFF) + (rSP & 0xFFF)) > 0xFFF;

            int result = this.rHL + this.rSP;
            this.rHL = result;
            this.fN = false;
            this.fC = result > 0xFFFF;

            return 8;
        }

        private int jp_hl(gshort operand)
        {
            this.rPC = (ushort)this.rHL;
            return 4;
        }

        private int rlc_a(gshort operand)
        {
            this.fC = (this.rA & 0x80) == 0x80;

            this.rA = (byte)(this.rA << 1);
            if ( this.fC )
            {
                this.rA |= 0x01;
            }

            this.fZ = false;
            this.fN = false;
            this.fH = false;

            return 4;
        }

        private int rrc_a(gshort operand)
        {
            this.fC = (this.rA & 0x01) == 0x01;

            this.rA = (byte)(this.rA >> 1);
            if ( this.fC )
            {
                this.rA |= 0x80;
            }

            this.fZ = false;
            this.fN = false;
            this.fH = false;

            return 4;
        }

        private int stop(gshort operand)
        {
            this.fStop = true;

            // Work around buggy software? ROMs will hard-lock the gameboy if they halt with interrupts disabled. Blargg's cpu_instrs test appears to do this right before test 3.
            if ( this.rInterruptEnable == 0 )
            {
                this.rInterruptEnable = 0x1f;
            }

            return 0; // WAS 4
        }

        private int halt(gshort operand)
        {
            this.fHalt = true;

            // Work around buggy software? ROMs will hard-lock the gameboy if they halt with interrupts disabled.
            if ( this.rInterruptEnable == 0 )
            {
                this.rInterruptEnable = 0x1f;
            }

            if ( !this.fInterruptMasterEnable && (this.rInterruptEnable & this.rInterruptFlags & 0x1f) != 0 )
            {
                // HALT bug. Just disabling halt isn't accurate, but is probably close enough
                this.fHalt = false;
            }

            return 0; // WAS 4
        }

        private int daa(gshort operand)
        {
            gshort a = this.rA;

            if ( !this.fN )
            {
                if ( this.fH || (a & 0x0F) > 9 )
                {
                    a += 6;
                }

                if ( this.fC || a > 0x9F )
                {
                    a += 0x60;
                }
            }
            else
            {
                if ( this.fH )
                {
                    a = (a - 6) & 0xFF;
                }

                if ( this.fC )
                {
                    a -= 0x60;
                }
            }

            this.rF = (byte)(this.rF & ~(0x20 | 0x80));
            if ( (a & 0x100) != 0 )
            {
                this.rF |= 0x10;
            }

            a &= 0xFF;
            if ( a == 0 )
            {
                this.rF |= 0x80;
            }

            this.rA = a.Lo;

            return 4;
        }

        private int crash(gshort operand) => throw new gbException($"Executed illegal instruction at address 0x{rPC:X4}");
    }
}