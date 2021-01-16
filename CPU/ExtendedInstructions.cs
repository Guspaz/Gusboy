namespace GusBoy
{
    public partial class CPU
    {
#pragma warning disable IDE1006 // Naming Styles
        private int swap_a()
        {
            this.rA = (byte)(((this.rA & 0xF0) >> 4) | ((this.rA & 0x0F) << 4));
            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int swap_b()
        {
            this.rB = (byte)(((this.rB & 0xF0) >> 4) | ((this.rB & 0x0F) << 4));
            this.fZ = this.rB == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int swap_c()
        {
            this.rC = (byte)(((this.rC & 0xF0) >> 4) | ((this.rC & 0x0F) << 4));
            this.fZ = this.rC == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int swap_d()
        {
            this.rD = (byte)(((this.rD & 0xF0) >> 4) | ((this.rD & 0x0F) << 4));
            this.fZ = this.rD == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int swap_e()
        {
            this.rE = (byte)(((this.rE & 0xF0) >> 4) | ((this.rE & 0x0F) << 4));
            this.fZ = this.rE == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int swap_h()
        {
            this.rH = (byte)(((this.rH & 0xF0) >> 4) | ((this.rH & 0x0F) << 4));
            this.fZ = this.rH == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int swap_l()
        {
            this.rL = (byte)(((this.rL & 0xF0) >> 4) | ((this.rL & 0x0F) << 4));
            this.fZ = this.rL == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 8;
        }

        private int swap_phl()
        {
            this.Ram[this.rHL] = (byte)(((this.Ram[this.rHL] & 0xF0) >> 4) | ((this.Ram[this.rHL] & 0x0F) << 4));
            this.fZ = this.Ram[this.rHL] == 0;
            this.fN = false;
            this.fH = false;
            this.fC = false;

            return 16;
        }

        private int res_0_a()
        {
            this.rA = (byte)(this.rA & ~(1 << 0));
            return 8;
        }

        private int res_0_b()
        {
            this.rB = (byte)(this.rB & ~(1 << 0));
            return 8;
        }

        private int res_0_c()
        {
            this.rC = (byte)(this.rC & ~(1 << 0));
            return 8;
        }

        private int res_0_d()
        {
            this.rD = (byte)(this.rD & ~(1 << 0));
            return 8;
        }

        private int res_0_e()
        {
            this.rE = (byte)(this.rE & ~(1 << 0));
            return 8;
        }

        private int res_0_h()
        {
            this.rH = (byte)(this.rH & ~(1 << 0));
            return 8;
        }

        private int res_0_l()
        {
            this.rL = (byte)(this.rL & ~(1 << 0));
            return 8;
        }

        private int res_0_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] & ~(1 << 0));
            return 16;
        }


        private int res_1_a()
        {
            this.rA = (byte)(this.rA & ~(1 << 1));
            return 8;
        }

        private int res_1_b()
        {
            this.rB = (byte)(this.rB & ~(1 << 1));
            return 8;
        }

        private int res_1_c()
        {
            this.rC = (byte)(this.rC & ~(1 << 1));
            return 8;
        }

        private int res_1_d()
        {
            this.rD = (byte)(this.rD & ~(1 << 1));
            return 8;
        }

        private int res_1_e()
        {
            this.rE = (byte)(this.rE & ~(1 << 1));
            return 8;
        }

        private int res_1_h()
        {
            this.rH = (byte)(this.rH & ~(1 << 1));
            return 8;
        }

        private int res_1_l()
        {
            this.rL = (byte)(this.rL & ~(1 << 1));
            return 8;
        }

        private int res_1_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] & ~(1 << 1));
            return 16;
        }


        private int res_2_a()
        {
            this.rA = (byte)(this.rA & ~(1 << 2));
            return 8;
        }

        private int res_2_b()
        {
            this.rB = (byte)(this.rB & ~(1 << 2));
            return 8;
        }

        private int res_2_c()
        {
            this.rC = (byte)(this.rC & ~(1 << 2));
            return 8;
        }

        private int res_2_d()
        {
            this.rD = (byte)(this.rD & ~(1 << 2));
            return 8;
        }

        private int res_2_e()
        {
            this.rE = (byte)(this.rE & ~(1 << 2));
            return 8;
        }

        private int res_2_h()
        {
            this.rH = (byte)(this.rH & ~(1 << 2));
            return 8;
        }

        private int res_2_l()
        {
            this.rL = (byte)(this.rL & ~(1 << 2));
            return 8;
        }

        private int res_2_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] & ~(1 << 2));
            return 16;
        }


        private int res_3_a()
        {
            this.rA = (byte)(this.rA & ~(1 << 3));
            return 8;
        }

        private int res_3_b()
        {
            this.rB = (byte)(this.rB & ~(1 << 3));
            return 8;
        }

        private int res_3_c()
        {
            this.rC = (byte)(this.rC & ~(1 << 3));
            return 8;
        }

        private int res_3_d()
        {
            this.rD = (byte)(this.rD & ~(1 << 3));
            return 8;
        }

        private int res_3_e()
        {
            this.rE = (byte)(this.rE & ~(1 << 3));
            return 8;
        }

        private int res_3_h()
        {
            this.rH = (byte)(this.rH & ~(1 << 3));
            return 8;
        }

        private int res_3_l()
        {
            this.rL = (byte)(this.rL & ~(1 << 3));
            return 8;
        }

        private int res_3_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] & ~(1 << 3));
            return 16;
        }


        private int res_4_a()
        {
            this.rA = (byte)(this.rA & ~(1 << 4));
            return 8;
        }

        private int res_4_b()
        {
            this.rB = (byte)(this.rB & ~(1 << 4));
            return 8;
        }

        private int res_4_c()
        {
            this.rC = (byte)(this.rC & ~(1 << 4));
            return 8;
        }

        private int res_4_d()
        {
            this.rD = (byte)(this.rD & ~(1 << 4));
            return 8;
        }

        private int res_4_e()
        {
            this.rE = (byte)(this.rE & ~(1 << 4));
            return 8;
        }

        private int res_4_h()
        {
            this.rH = (byte)(this.rH & ~(1 << 4));
            return 8;
        }

        private int res_4_l()
        {
            this.rL = (byte)(this.rL & ~(1 << 4));
            return 8;
        }

        private int res_4_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] & ~(1 << 4));
            return 16;
        }


        private int res_5_a()
        {
            this.rA = (byte)(this.rA & ~(1 << 5));
            return 8;
        }

        private int res_5_b()
        {
            this.rB = (byte)(this.rB & ~(1 << 5));
            return 8;
        }

        private int res_5_c()
        {
            this.rC = (byte)(this.rC & ~(1 << 5));
            return 8;
        }

        private int res_5_d()
        {
            this.rD = (byte)(this.rD & ~(1 << 5));
            return 8;
        }

        private int res_5_e()
        {
            this.rE = (byte)(this.rE & ~(1 << 5));
            return 8;
        }

        private int res_5_h()
        {
            this.rH = (byte)(this.rH & ~(1 << 5));
            return 8;
        }

        private int res_5_l()
        {
            this.rL = (byte)(this.rL & ~(1 << 5));
            return 8;
        }

        private int res_5_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] & ~(1 << 5));
            return 16;
        }


        private int res_6_a()
        {
            this.rA = (byte)(this.rA & ~(1 << 6));
            return 8;
        }

        private int res_6_b()
        {
            this.rB = (byte)(this.rB & ~(1 << 6));
            return 8;
        }

        private int res_6_c()
        {
            this.rC = (byte)(this.rC & ~(1 << 6));
            return 8;
        }

        private int res_6_d()
        {
            this.rD = (byte)(this.rD & ~(1 << 6));
            return 8;
        }

        private int res_6_e()
        {
            this.rE = (byte)(this.rE & ~(1 << 6));
            return 8;
        }

        private int res_6_h()
        {
            this.rH = (byte)(this.rH & ~(1 << 6));
            return 8;
        }

        private int res_6_l()
        {
            this.rL = (byte)(this.rL & ~(1 << 6));
            return 8;
        }

        private int res_6_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] & ~(1 << 6));
            return 16;
        }


        private int res_7_a()
        {
            this.rA = (byte)(this.rA & ~(1 << 7));
            return 8;
        }

        private int res_7_b()
        {
            this.rB = (byte)(this.rB & ~(1 << 7));
            return 8;
        }

        private int res_7_c()
        {
            this.rC = (byte)(this.rC & ~(1 << 7));
            return 8;
        }

        private int res_7_d()
        {
            this.rD = (byte)(this.rD & ~(1 << 7));
            return 8;
        }

        private int res_7_e()
        {
            this.rE = (byte)(this.rE & ~(1 << 7));
            return 8;
        }

        private int res_7_h()
        {
            this.rH = (byte)(this.rH & ~(1 << 7));
            return 8;
        }

        private int res_7_l()
        {
            this.rL = (byte)(this.rL & ~(1 << 7));
            return 8;
        }

        private int res_7_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] & ~(1 << 7));
            return 16;
        }



        private int set_0_a()
        {
            this.rA = (byte)(this.rA | (1 << 0));
            return 8;
        }

        private int set_0_b()
        {
            this.rB = (byte)(this.rB | (1 << 0));
            return 8;
        }

        private int set_0_c()
        {
            this.rC = (byte)(this.rC | (1 << 0));
            return 8;
        }

        private int set_0_d()
        {
            this.rD = (byte)(this.rD | (1 << 0));
            return 8;
        }

        private int set_0_e()
        {
            this.rE = (byte)(this.rE | (1 << 0));
            return 8;
        }

        private int set_0_h()
        {
            this.rH = (byte)(this.rH | (1 << 0));
            return 8;
        }

        private int set_0_l()
        {
            this.rL = (byte)(this.rL | (1 << 0));
            return 8;
        }

        private int set_0_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] | (1 << 0));
            return 16;
        }


        private int set_1_a()
        {
            this.rA = (byte)(this.rA | (1 << 1));
            return 8;
        }

        private int set_1_b()
        {
            this.rB = (byte)(this.rB | (1 << 1));
            return 8;
        }

        private int set_1_c()
        {
            this.rC = (byte)(this.rC | (1 << 1));
            return 8;
        }

        private int set_1_d()
        {
            this.rD = (byte)(this.rD | (1 << 1));
            return 8;
        }

        private int set_1_e()
        {
            this.rE = (byte)(this.rE | (1 << 1));
            return 8;
        }

        private int set_1_h()
        {
            this.rH = (byte)(this.rH | (1 << 1));
            return 8;
        }

        private int set_1_l()
        {
            this.rL = (byte)(this.rL | (1 << 1));
            return 8;
        }

        private int set_1_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] | (1 << 1));
            return 16;
        }


        private int set_2_a()
        {
            this.rA = (byte)(this.rA | (1 << 2));
            return 8;
        }

        private int set_2_b()
        {
            this.rB = (byte)(this.rB | (1 << 2));
            return 8;
        }

        private int set_2_c()
        {
            this.rC = (byte)(this.rC | (1 << 2));
            return 8;
        }

        private int set_2_d()
        {
            this.rD = (byte)(this.rD | (1 << 2));
            return 8;
        }

        private int set_2_e()
        {
            this.rE = (byte)(this.rE | (1 << 2));
            return 8;
        }

        private int set_2_h()
        {
            this.rH = (byte)(this.rH | (1 << 2));
            return 8;
        }

        private int set_2_l()
        {
            this.rL = (byte)(this.rL | (1 << 2));
            return 8;
        }

        private int set_2_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] | (1 << 2));
            return 16;
        }


        private int set_3_a()
        {
            this.rA = (byte)(this.rA | (1 << 3));
            return 8;
        }

        private int set_3_b()
        {
            this.rB = (byte)(this.rB | (1 << 3));
            return 8;
        }

        private int set_3_c()
        {
            this.rC = (byte)(this.rC | (1 << 3));
            return 8;
        }

        private int set_3_d()
        {
            this.rD = (byte)(this.rD | (1 << 3));
            return 8;
        }

        private int set_3_e()
        {
            this.rE = (byte)(this.rE | (1 << 3));
            return 8;
        }

        private int set_3_h()
        {
            this.rH = (byte)(this.rH | (1 << 3));
            return 8;
        }

        private int set_3_l()
        {
            this.rL = (byte)(this.rL | (1 << 3));
            return 8;
        }

        private int set_3_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] | (1 << 3));
            return 16;
        }


        private int set_4_a()
        {
            this.rA = (byte)(this.rA | (1 << 4));
            return 8;
        }

        private int set_4_b()
        {
            this.rB = (byte)(this.rB | (1 << 4));
            return 8;
        }

        private int set_4_c()
        {
            this.rC = (byte)(this.rC | (1 << 4));
            return 8;
        }

        private int set_4_d()
        {
            this.rD = (byte)(this.rD | (1 << 4));
            return 8;
        }

        private int set_4_e()
        {
            this.rE = (byte)(this.rE | (1 << 4));
            return 8;
        }

        private int set_4_h()
        {
            this.rH = (byte)(this.rH | (1 << 4));
            return 8;
        }

        private int set_4_l()
        {
            this.rL = (byte)(this.rL | (1 << 4));
            return 8;
        }

        private int set_4_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] | (1 << 4));
            return 16;
        }


        private int set_5_a()
        {
            this.rA = (byte)(this.rA | (1 << 5));
            return 8;
        }

        private int set_5_b()
        {
            this.rB = (byte)(this.rB | (1 << 5));
            return 8;
        }

        private int set_5_c()
        {
            this.rC = (byte)(this.rC | (1 << 5));
            return 8;
        }

        private int set_5_d()
        {
            this.rD = (byte)(this.rD | (1 << 5));
            return 8;
        }

        private int set_5_e()
        {
            this.rE = (byte)(this.rE | (1 << 5));
            return 8;
        }

        private int set_5_h()
        {
            this.rH = (byte)(this.rH | (1 << 5));
            return 8;
        }

        private int set_5_l()
        {
            this.rL = (byte)(this.rL | (1 << 5));
            return 8;
        }

        private int set_5_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] | (1 << 5));
            return 16;
        }


        private int set_6_a()
        {
            this.rA = (byte)(this.rA | (1 << 6));
            return 8;
        }

        private int set_6_b()
        {
            this.rB = (byte)(this.rB | (1 << 6));
            return 8;
        }

        private int set_6_c()
        {
            this.rC = (byte)(this.rC | (1 << 6));
            return 8;
        }

        private int set_6_d()
        {
            this.rD = (byte)(this.rD | (1 << 6));
            return 8;
        }

        private int set_6_e()
        {
            this.rE = (byte)(this.rE | (1 << 6));
            return 8;
        }

        private int set_6_h()
        {
            this.rH = (byte)(this.rH | (1 << 6));
            return 8;
        }

        private int set_6_l()
        {
            this.rL = (byte)(this.rL | (1 << 6));
            return 8;
        }

        private int set_6_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] | (1 << 6));
            return 16;
        }


        private int set_7_a()
        {
            this.rA = (byte)(this.rA | (1 << 7));
            return 8;
        }

        private int set_7_b()
        {
            this.rB = (byte)(this.rB | (1 << 7));
            return 8;
        }

        private int set_7_c()
        {
            this.rC = (byte)(this.rC | (1 << 7));
            return 8;
        }

        private int set_7_d()
        {
            this.rD = (byte)(this.rD | (1 << 7));
            return 8;
        }

        private int set_7_e()
        {
            this.rE = (byte)(this.rE | (1 << 7));
            return 8;
        }

        private int set_7_h()
        {
            this.rH = (byte)(this.rH | (1 << 7));
            return 8;
        }

        private int set_7_l()
        {
            this.rL = (byte)(this.rL | (1 << 7));
            return 8;
        }

        private int set_7_phl()
        {
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] | (1 << 7));
            return 16;
        }

        private int sla_a()
        {
            this.fC = (this.rA & 0x80) == 0x80;

            this.rA = (byte)(this.rA << 1);

            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sla_b()
        {
            this.fC = (this.rB & 0x80) == 0x80;

            this.rB = (byte)(this.rB << 1);

            this.fZ = this.rB == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sla_c()
        {
            this.fC = (this.rC & 0x80) == 0x80;

            this.rC = (byte)(this.rC << 1);

            this.fZ = this.rC == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sla_d()
        {
            this.fC = (this.rD & 0x80) == 0x80;

            this.rD = (byte)(this.rD << 1);

            this.fZ = this.rD == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sla_e()
        {
            this.fC = (this.rE & 0x80) == 0x80;

            this.rE = (byte)(this.rE << 1);

            this.fZ = this.rE == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sla_h()
        {
            this.fC = (this.rH & 0x80) == 0x80;

            this.rH = (byte)(this.rH << 1);

            this.fZ = this.rH == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sla_l()
        {
            this.fC = (this.rL & 0x80) == 0x80;

            this.rL = (byte)(this.rL << 1);

            this.fZ = this.rL == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sla_phl()
        {
            this.fC = (this.Ram[this.rHL] & 0x80) == 0x80;

            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] << 1);

            this.fZ = this.Ram[this.rHL] == 0;
            this.fN = false;
            this.fH = false;

            return 16;
        }

        private int sra_a()
        {
            this.fC = (this.rA & 0x01) == 0x01;

            this.rA = (byte)(this.rA >> 1);
            if ( (this.rA & 0x40) == 0x40 )
            {
                this.rA |= 0x80;
            }

            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sra_b()
        {
            this.fC = (this.rB & 0x01) == 0x01;

            this.rB = (byte)(this.rB >> 1);
            if ( (this.rB & 0x40) == 0x40 )
            {
                this.rB |= 0x80;
            }

            this.fZ = this.rB == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sra_c()
        {
            this.fC = (this.rC & 0x01) == 0x01;

            this.rC = (byte)(this.rC >> 1);
            if ( (this.rC & 0x40) == 0x40 )
            {
                this.rC |= 0x80;
            }

            this.fZ = this.rC == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sra_d()
        {
            this.fC = (this.rD & 0x01) == 0x01;

            this.rD = (byte)(this.rD >> 1);
            if ( (this.rD & 0x40) == 0x40 )
            {
                this.rD |= 0x80;
            }

            this.fZ = this.rD == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sra_e()
        {
            this.fC = (this.rE & 0x01) == 0x01;

            this.rE = (byte)(this.rE >> 1);
            if ( (this.rE & 0x40) == 0x40 )
            {
                this.rE |= 0x80;
            }

            this.fZ = this.rE == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sra_h()
        {
            this.fC = (this.rH & 0x01) == 0x01;

            this.rH = (byte)(this.rH >> 1);
            if ( (this.rH & 0x40) == 0x40 )
            {
                this.rH |= 0x80;
            }

            this.fZ = this.rH == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sra_l()
        {
            this.fC = (this.rL & 0x01) == 0x01;

            this.rL = (byte)(this.rL >> 1);
            if ( (this.rL & 0x40) == 0x40 )
            {
                this.rL |= 0x80;
            }

            this.fZ = this.rL == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int sra_phl()
        {
            this.fC = (this.Ram[this.rHL] & 0x01) == 0x01;

            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] >> 1);
            if ( (this.Ram[this.rHL] & 0x40) == 0x40 )
            {
                this.Ram[this.rHL] |= 0x80;
            }

            this.fZ = this.Ram[this.rHL] == 0;
            this.fN = false;
            this.fH = false;

            return 16;
        }

        private int err_a()
        {
            bool oldFC = this.fC;

            this.fC = (this.rA & 0x01) == 0x01;
            this.rA = (byte)(this.rA >> 1);
            if ( oldFC )
            {
                this.rA |= 0x80;
            }

            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rr_b()
        {
            bool oldFC = this.fC;

            this.fC = (this.rB & 0x01) == 0x01;
            this.rB = (byte)(this.rB >> 1);
            if ( oldFC )
            {
                this.rB |= 0x80;
            }

            this.fZ = this.rB == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rr_c()
        {
            bool oldFC = this.fC;

            this.fC = (this.rC & 0x01) == 0x01;
            this.rC = (byte)(this.rC >> 1);
            if ( oldFC )
            {
                this.rC |= 0x80;
            }

            this.fZ = this.rC == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rr_d()
        {
            bool oldFC = this.fC;

            this.fC = (this.rD & 0x01) == 0x01;
            this.rD = (byte)(this.rD >> 1);
            if ( oldFC )
            {
                this.rD |= 0x80;
            }

            this.fZ = this.rD == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rr_e()
        {
            bool oldFC = this.fC;

            this.fC = (this.rE & 0x01) == 0x01;
            this.rE = (byte)(this.rE >> 1);
            if ( oldFC )
            {
                this.rE |= 0x80;
            }

            this.fZ = this.rE == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rr_h()
        {
            bool oldFC = this.fC;

            this.fC = (this.rH & 0x01) == 0x01;
            this.rH = (byte)(this.rH >> 1);
            if ( oldFC )
            {
                this.rH |= 0x80;
            }

            this.fZ = this.rH == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rr_l()
        {
            bool oldFC = this.fC;

            this.fC = (this.rL & 0x01) == 0x01;
            this.rL = (byte)(this.rL >> 1);
            if ( oldFC )
            {
                this.rL |= 0x80;
            }

            this.fZ = this.rL == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rr_phl()
        {
            bool oldFC = this.fC;

            this.fC = (this.Ram[this.rHL] & 0x01) == 0x01;
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] >> 1);
            if ( oldFC )
            {
                this.Ram[this.rHL] |= 0x80;
            }

            this.fZ = this.Ram[this.rHL] == 0;
            this.fN = false;
            this.fH = false;

            return 16;
        }

        private int erl_a()
        {
            bool oldFC = this.fC;

            this.fC = (this.rA & 0x80) == 0x80;
            this.rA = (byte)(this.rA << 1);
            if ( oldFC )
            {
                this.rA |= 0x01;
            }

            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rl_b()
        {
            bool oldFC = this.fC;

            this.fC = (this.rB & 0x80) == 0x80;
            this.rB = (byte)(this.rB << 1);
            if ( oldFC )
            {
                this.rB |= 0x01;
            }

            this.fZ = this.rB == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rl_c()
        {
            bool oldFC = this.fC;

            this.fC = (this.rC & 0x80) == 0x80;
            this.rC = (byte)(this.rC << 1);
            if ( oldFC )
            {
                this.rC |= 0x01;
            }

            this.fZ = this.rC == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rl_d()
        {
            bool oldFC = this.fC;

            this.fC = (this.rD & 0x80) == 0x80;
            this.rD = (byte)(this.rD << 1);
            if ( oldFC )
            {
                this.rD |= 0x01;
            }

            this.fZ = this.rD == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rl_e()
        {
            bool oldFC = this.fC;

            this.fC = (this.rE & 0x80) == 0x80;
            this.rE = (byte)(this.rE << 1);
            if ( oldFC )
            {
                this.rE |= 0x01;
            }

            this.fZ = this.rE == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rl_h()
        {
            bool oldFC = this.fC;

            this.fC = (this.rH & 0x80) == 0x80;
            this.rH = (byte)(this.rH << 1);
            if ( oldFC )
            {
                this.rH |= 0x01;
            }

            this.fZ = this.rH == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rl_l()
        {
            bool oldFC = this.fC;

            this.fC = (this.rL & 0x80) == 0x80;
            this.rL = (byte)(this.rL << 1);
            if ( oldFC )
            {
                this.rL |= 0x01;
            }

            this.fZ = this.rL == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rl_phl()
        {
            bool oldFC = this.fC;

            this.fC = (this.Ram[this.rHL] & 0x80) == 0x80;
            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] << 1);
            if ( oldFC )
            {
                this.Ram[this.rHL] |= 0x01;
            }

            this.fZ = this.Ram[this.rHL] == 0;
            this.fN = false;
            this.fH = false;

            return 16;
        }

        private int srl_a()
        {
            this.fC = (this.rA & 0x01) == 0x01;

            this.rA = (byte)(this.rA >> 1);

            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int srl_b()
        {
            this.fC = (this.rB & 0x01) == 0x01;

            this.rB = (byte)(this.rB >> 1);

            this.fZ = this.rB == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int srl_c()
        {
            this.fC = (this.rC & 0x01) == 0x01;

            this.rC = (byte)(this.rC >> 1);

            this.fZ = this.rC == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int srl_d()
        {
            this.fC = (this.rD & 0x01) == 0x01;

            this.rD = (byte)(this.rD >> 1);

            this.fZ = this.rD == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int srl_e()
        {
            this.fC = (this.rE & 0x01) == 0x01;

            this.rE = (byte)(this.rE >> 1);

            this.fZ = this.rE == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int srl_h()
        {
            this.fC = (this.rH & 0x01) == 0x01;

            this.rH = (byte)(this.rH >> 1);

            this.fZ = this.rH == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int srl_l()
        {
            this.fC = (this.rL & 0x01) == 0x01;

            this.rL = (byte)(this.rL >> 1);

            this.fZ = this.rL == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int srl_phl()
        {
            this.fC = (this.Ram[this.rHL] & 0x01) == 0x01;

            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] >> 1);

            this.fZ = this.Ram[this.rHL] == 0;
            this.fN = false;
            this.fH = false;

            return 16;
        }

        private int erlc_a()
        {
            this.fC = (this.rA & 0x80) == 0x80;

            this.rA = (byte)(this.rA << 1);
            if ( this.fC )
            {
                this.rA |= 0x01;
            }

            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rlc_b()
        {
            this.fC = (this.rB & 0x80) == 0x80;

            this.rB = (byte)(this.rB << 1);
            if ( this.fC )
            {
                this.rB |= 0x01;
            }

            this.fZ = this.rB == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rlc_c()
        {
            this.fC = (this.rC & 0x80) == 0x80;

            this.rC = (byte)(this.rC << 1);
            if ( this.fC )
            {
                this.rC |= 0x01;
            }

            this.fZ = this.rC == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rlc_d()
        {
            this.fC = (this.rD & 0x80) == 0x80;

            this.rD = (byte)(this.rD << 1);
            if ( this.fC )
            {
                this.rD |= 0x01;
            }

            this.fZ = this.rD == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rlc_e()
        {
            this.fC = (this.rE & 0x80) == 0x80;

            this.rE = (byte)(this.rE << 1);
            if ( this.fC )
            {
                this.rE |= 0x01;
            }

            this.fZ = this.rE == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rlc_h()
        {
            this.fC = (this.rH & 0x80) == 0x80;

            this.rH = (byte)(this.rH << 1);
            if ( this.fC )
            {
                this.rH |= 0x01;
            }

            this.fZ = this.rH == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rlc_l()
        {
            this.fC = (this.rL & 0x80) == 0x80;

            this.rL = (byte)(this.rL << 1);
            if ( this.fC )
            {
                this.rL |= 0x01;
            }

            this.fZ = this.rL == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rlc_phl()
        {
            this.fC = (this.Ram[this.rHL] & 0x80) == 0x80;

            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] << 1);
            if ( this.fC )
            {
                this.Ram[this.rHL] |= 0x01;
            }

            this.fZ = this.Ram[this.rHL] == 0;
            this.fN = false;
            this.fH = false;

            return 16;
        }

        private int errc_a()
        {
            this.fC = (this.rA & 0x01) == 0x01;

            this.rA = (byte)(this.rA >> 1);
            if ( this.fC )
            {
                this.rA |= 0x80;
            }

            this.fZ = this.rA == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rrc_b()
        {
            this.fC = (this.rB & 0x01) == 0x01;

            this.rB = (byte)(this.rB >> 1);
            if ( this.fC )
            {
                this.rB |= 0x80;
            }

            this.fZ = this.rB == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rrc_c()
        {
            this.fC = (this.rC & 0x01) == 0x01;

            this.rC = (byte)(this.rC >> 1);
            if ( this.fC )
            {
                this.rC |= 0x80;
            }

            this.fZ = this.rC == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rrc_d()
        {
            this.fC = (this.rD & 0x01) == 0x01;

            this.rD = (byte)(this.rD >> 1);
            if ( this.fC )
            {
                this.rD |= 0x80;
            }

            this.fZ = this.rD == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rrc_e()
        {
            this.fC = (this.rE & 0x01) == 0x01;

            this.rE = (byte)(this.rE >> 1);
            if ( this.fC )
            {
                this.rE |= 0x80;
            }

            this.fZ = this.rE == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rrc_h()
        {
            this.fC = (this.rH & 0x01) == 0x01;

            this.rH = (byte)(this.rH >> 1);
            if ( this.fC )
            {
                this.rH |= 0x80;
            }

            this.fZ = this.rH == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rrc_l()
        {
            this.fC = (this.rL & 0x01) == 0x01;

            this.rL = (byte)(this.rL >> 1);
            if ( this.fC )
            {
                this.rL |= 0x80;
            }

            this.fZ = this.rL == 0;
            this.fN = false;
            this.fH = false;

            return 8;
        }

        private int rrc_phl()
        {
            this.fC = (this.Ram[this.rHL] & 0x01) == 0x01;

            this.Ram[this.rHL] = (byte)(this.Ram[this.rHL] >> 1);
            if ( this.fC )
            {
                this.Ram[this.rHL] |= 0x80;
            }

            this.fZ = this.Ram[this.rHL] == 0;
            this.fN = false;
            this.fH = false;

            return 16;
        }

        private int bit_0_a()
        {
            this.fZ = (this.rA & (1 << 0)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_0_b()
        {
            this.fZ = (this.rB & (1 << 0)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_0_c()
        {
            this.fZ = (this.rC & (1 << 0)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_0_d()
        {
            this.fZ = (this.rD & (1 << 0)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_0_e()
        {
            this.fZ = (this.rE & (1 << 0)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_0_h()
        {
            this.fZ = (this.rH & (1 << 0)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_0_l()
        {
            this.fZ = (this.rL & (1 << 0)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_0_phl()
        {
            this.fZ = (this.Ram[this.rHL] & (1 << 0)) == 0;
            this.fN = false;
            this.fH = true;
            return 12;
        }


        private int bit_1_a()
        {
            this.fZ = (this.rA & (1 << 1)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_1_b()
        {
            this.fZ = (this.rB & (1 << 1)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_1_c()
        {
            this.fZ = (this.rC & (1 << 1)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_1_d()
        {
            this.fZ = (this.rD & (1 << 1)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_1_e()
        {
            this.fZ = (this.rE & (1 << 1)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_1_h()
        {
            this.fZ = (this.rH & (1 << 1)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_1_l()
        {
            this.fZ = (this.rL & (1 << 1)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_1_phl()
        {
            this.fZ = (this.Ram[this.rHL] & (1 << 1)) == 0;
            this.fN = false;
            this.fH = true;
            return 12;
        }


        private int bit_2_a()
        {
            this.fZ = (this.rA & (1 << 2)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_2_b()
        {
            this.fZ = (this.rB & (1 << 2)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_2_c()
        {
            this.fZ = (this.rC & (1 << 2)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_2_d()
        {
            this.fZ = (this.rD & (1 << 2)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_2_e()
        {
            this.fZ = (this.rE & (1 << 2)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_2_h()
        {
            this.fZ = (this.rH & (1 << 2)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_2_l()
        {
            this.fZ = (this.rL & (1 << 2)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_2_phl()
        {
            this.fZ = (this.Ram[this.rHL] & (1 << 2)) == 0;
            this.fN = false;
            this.fH = true;
            return 12;
        }


        private int bit_3_a()
        {
            this.fZ = (this.rA & (1 << 3)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_3_b()
        {
            this.fZ = (this.rB & (1 << 3)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_3_c()
        {
            this.fZ = (this.rC & (1 << 3)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_3_d()
        {
            this.fZ = (this.rD & (1 << 3)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_3_e()
        {
            this.fZ = (this.rE & (1 << 3)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_3_h()
        {
            this.fZ = (this.rH & (1 << 3)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_3_l()
        {
            this.fZ = (this.rL & (1 << 3)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_3_phl()
        {
            this.fZ = (this.Ram[this.rHL] & (1 << 3)) == 0;
            this.fN = false;
            this.fH = true;
            return 12;
        }


        private int bit_4_a()
        {
            this.fZ = (this.rA & (1 << 4)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_4_b()
        {
            this.fZ = (this.rB & (1 << 4)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_4_c()
        {
            this.fZ = (this.rC & (1 << 4)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_4_d()
        {
            this.fZ = (this.rD & (1 << 4)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_4_e()
        {
            this.fZ = (this.rE & (1 << 4)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_4_h()
        {
            this.fZ = (this.rH & (1 << 4)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_4_l()
        {
            this.fZ = (this.rL & (1 << 4)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_4_phl()
        {
            this.fZ = (this.Ram[this.rHL] & (1 << 4)) == 0;
            this.fN = false;
            this.fH = true;
            return 12;
        }


        private int bit_5_a()
        {
            this.fZ = (this.rA & (1 << 5)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_5_b()
        {
            this.fZ = (this.rB & (1 << 5)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_5_c()
        {
            this.fZ = (this.rC & (1 << 5)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_5_d()
        {
            this.fZ = (this.rD & (1 << 5)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_5_e()
        {
            this.fZ = (this.rE & (1 << 5)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_5_h()
        {
            this.fZ = (this.rH & (1 << 5)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_5_l()
        {
            this.fZ = (this.rL & (1 << 5)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_5_phl()
        {
            this.fZ = (this.Ram[this.rHL] & (1 << 5)) == 0;
            this.fN = false;
            this.fH = true;
            return 12;
        }


        private int bit_6_a()
        {
            this.fZ = (this.rA & (1 << 6)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_6_b()
        {
            this.fZ = (this.rB & (1 << 6)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_6_c()
        {
            this.fZ = (this.rC & (1 << 6)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_6_d()
        {
            this.fZ = (this.rD & (1 << 6)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_6_e()
        {
            this.fZ = (this.rE & (1 << 6)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_6_h()
        {
            this.fZ = (this.rH & (1 << 6)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_6_l()
        {
            this.fZ = (this.rL & (1 << 6)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_6_phl()
        {
            this.fZ = (this.Ram[this.rHL] & (1 << 6)) == 0;
            this.fN = false;
            this.fH = true;
            return 12;
        }


        private int bit_7_a()
        {
            this.fZ = (this.rA & (1 << 7)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_7_b()
        {
            this.fZ = (this.rB & (1 << 7)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_7_c()
        {
            this.fZ = (this.rC & (1 << 7)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_7_d()
        {
            this.fZ = (this.rD & (1 << 7)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_7_e()
        {
            this.fZ = (this.rE & (1 << 7)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_7_h()
        {
            this.fZ = (this.rH & (1 << 7)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_7_l()
        {
            this.fZ = (this.rL & (1 << 7)) == 0;
            this.fN = false;
            this.fH = true;
            return 8;
        }

        private int bit_7_phl()
        {
            this.fZ = (this.Ram[this.rHL] & (1 << 7)) == 0;
            this.fN = false;
            this.fH = true;
            return 12;
        }
#pragma warning restore IDE1006 // Naming Styles
    }
}