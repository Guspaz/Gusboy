namespace GusBoy
{
    public partial class CPU
    {
        private int swap_a()
        {
            rA = (byte)(((rA & 0xF0) >> 4) | ((rA & 0x0F) << 4));
            fZ = rA == 0;
            fN = false;
            fH = false;
            fC = false;

            return 8;
        }

        private int swap_b()
        {
            rB = (byte)(((rB & 0xF0) >> 4) | ((rB & 0x0F) << 4));
            fZ = rB == 0;
            fN = false;
            fH = false;
            fC = false;

            return 8;
        }

        private int swap_c()
        {
            rC = (byte)(((rC & 0xF0) >> 4) | ((rC & 0x0F) << 4));
            fZ = rC == 0;
            fN = false;
            fH = false;
            fC = false;

            return 8;
        }

        private int swap_d()
        {
            rD = (byte)(((rD & 0xF0) >> 4) | ((rD & 0x0F) << 4));
            fZ = rD == 0;
            fN = false;
            fH = false;
            fC = false;

            return 8;
        }

        private int swap_e()
        {
            rE = (byte)(((rE & 0xF0) >> 4) | ((rE & 0x0F) << 4));
            fZ = rE == 0;
            fN = false;
            fH = false;
            fC = false;

            return 8;
        }

        private int swap_h()
        {
            rH = (byte)(((rH & 0xF0) >> 4) | ((rH & 0x0F) << 4));
            fZ = rH == 0;
            fN = false;
            fH = false;
            fC = false;

            return 8;
        }

        private int swap_l()
        {
            rL = (byte)(((rL & 0xF0) >> 4) | ((rL & 0x0F) << 4));
            fZ = rL == 0;
            fN = false;
            fH = false;
            fC = false;

            return 8;
        }

        private int swap_phl()
        {
            ram[rHL] = (byte)(((ram[rHL] & 0xF0) >> 4) | ((ram[rHL] & 0x0F) << 4));
            fZ = ram[rHL] == 0;
            fN = false;
            fH = false;
            fC = false;

            return 16;
        }

        private int res_0_a()
        {
            rA = (byte)(rA & ~(1 << 0));
            return 8;
        }

        private int res_0_b()
        {
            rB = (byte)(rB & ~(1 << 0));
            return 8;
        }

        private int res_0_c()
        {
            rC = (byte)(rC & ~(1 << 0));
            return 8;
        }

        private int res_0_d()
        {
            rD = (byte)(rD & ~(1 << 0));
            return 8;
        }

        private int res_0_e()
        {
            rE = (byte)(rE & ~(1 << 0));
            return 8;
        }

        private int res_0_h()
        {
            rH = (byte)(rH & ~(1 << 0));
            return 8;
        }

        private int res_0_l()
        {
            rL = (byte)(rL & ~(1 << 0));
            return 8;
        }

        private int res_0_phl()
        {
            ram[rHL] = (byte)(ram[rHL] & ~(1 << 0));
            return 16;
        }


        private int res_1_a()
        {
            rA = (byte)(rA & ~(1 << 1));
            return 8;
        }

        private int res_1_b()
        {
            rB = (byte)(rB & ~(1 << 1));
            return 8;
        }

        private int res_1_c()
        {
            rC = (byte)(rC & ~(1 << 1));
            return 8;
        }

        private int res_1_d()
        {
            rD = (byte)(rD & ~(1 << 1));
            return 8;
        }

        private int res_1_e()
        {
            rE = (byte)(rE & ~(1 << 1));
            return 8;
        }

        private int res_1_h()
        {
            rH = (byte)(rH & ~(1 << 1));
            return 8;
        }

        private int res_1_l()
        {
            rL = (byte)(rL & ~(1 << 1));
            return 8;
        }

        private int res_1_phl()
        {
            ram[rHL] = (byte)(ram[rHL] & ~(1 << 1));
            return 16;
        }


        private int res_2_a()
        {
            rA = (byte)(rA & ~(1 << 2));
            return 8;
        }

        private int res_2_b()
        {
            rB = (byte)(rB & ~(1 << 2));
            return 8;
        }

        private int res_2_c()
        {
            rC = (byte)(rC & ~(1 << 2));
            return 8;
        }

        private int res_2_d()
        {
            rD = (byte)(rD & ~(1 << 2));
            return 8;
        }

        private int res_2_e()
        {
            rE = (byte)(rE & ~(1 << 2));
            return 8;
        }

        private int res_2_h()
        {
            rH = (byte)(rH & ~(1 << 2));
            return 8;
        }

        private int res_2_l()
        {
            rL = (byte)(rL & ~(1 << 2));
            return 8;
        }

        private int res_2_phl()
        {
            ram[rHL] = (byte)(ram[rHL] & ~(1 << 2));
            return 16;
        }


        private int res_3_a()
        {
            rA = (byte)(rA & ~(1 << 3));
            return 8;
        }

        private int res_3_b()
        {
            rB = (byte)(rB & ~(1 << 3));
            return 8;
        }

        private int res_3_c()
        {
            rC = (byte)(rC & ~(1 << 3));
            return 8;
        }

        private int res_3_d()
        {
            rD = (byte)(rD & ~(1 << 3));
            return 8;
        }

        private int res_3_e()
        {
            rE = (byte)(rE & ~(1 << 3));
            return 8;
        }

        private int res_3_h()
        {
            rH = (byte)(rH & ~(1 << 3));
            return 8;
        }

        private int res_3_l()
        {
            rL = (byte)(rL & ~(1 << 3));
            return 8;
        }

        private int res_3_phl()
        {
            ram[rHL] = (byte)(ram[rHL] & ~(1 << 3));
            return 16;
        }


        private int res_4_a()
        {
            rA = (byte)(rA & ~(1 << 4));
            return 8;
        }

        private int res_4_b()
        {
            rB = (byte)(rB & ~(1 << 4));
            return 8;
        }

        private int res_4_c()
        {
            rC = (byte)(rC & ~(1 << 4));
            return 8;
        }

        private int res_4_d()
        {
            rD = (byte)(rD & ~(1 << 4));
            return 8;
        }

        private int res_4_e()
        {
            rE = (byte)(rE & ~(1 << 4));
            return 8;
        }

        private int res_4_h()
        {
            rH = (byte)(rH & ~(1 << 4));
            return 8;
        }

        private int res_4_l()
        {
            rL = (byte)(rL & ~(1 << 4));
            return 8;
        }

        private int res_4_phl()
        {
            ram[rHL] = (byte)(ram[rHL] & ~(1 << 4));
            return 16;
        }


        private int res_5_a()
        {
            rA = (byte)(rA & ~(1 << 5));
            return 8;
        }

        private int res_5_b()
        {
            rB = (byte)(rB & ~(1 << 5));
            return 8;
        }

        private int res_5_c()
        {
            rC = (byte)(rC & ~(1 << 5));
            return 8;
        }

        private int res_5_d()
        {
            rD = (byte)(rD & ~(1 << 5));
            return 8;
        }

        private int res_5_e()
        {
            rE = (byte)(rE & ~(1 << 5));
            return 8;
        }

        private int res_5_h()
        {
            rH = (byte)(rH & ~(1 << 5));
            return 8;
        }

        private int res_5_l()
        {
            rL = (byte)(rL & ~(1 << 5));
            return 8;
        }

        private int res_5_phl()
        {
            ram[rHL] = (byte)(ram[rHL] & ~(1 << 5));
            return 16;
        }


        private int res_6_a()
        {
            rA = (byte)(rA & ~(1 << 6));
            return 8;
        }

        private int res_6_b()
        {
            rB = (byte)(rB & ~(1 << 6));
            return 8;
        }

        private int res_6_c()
        {
            rC = (byte)(rC & ~(1 << 6));
            return 8;
        }

        private int res_6_d()
        {
            rD = (byte)(rD & ~(1 << 6));
            return 8;
        }

        private int res_6_e()
        {
            rE = (byte)(rE & ~(1 << 6));
            return 8;
        }

        private int res_6_h()
        {
            rH = (byte)(rH & ~(1 << 6));
            return 8;
        }

        private int res_6_l()
        {
            rL = (byte)(rL & ~(1 << 6));
            return 8;
        }

        private int res_6_phl()
        {
            ram[rHL] = (byte)(ram[rHL] & ~(1 << 6));
            return 16;
        }


        private int res_7_a()
        {
            rA = (byte)(rA & ~(1 << 7));
            return 8;
        }

        private int res_7_b()
        {
            rB = (byte)(rB & ~(1 << 7));
            return 8;
        }

        private int res_7_c()
        {
            rC = (byte)(rC & ~(1 << 7));
            return 8;
        }

        private int res_7_d()
        {
            rD = (byte)(rD & ~(1 << 7));
            return 8;
        }

        private int res_7_e()
        {
            rE = (byte)(rE & ~(1 << 7));
            return 8;
        }

        private int res_7_h()
        {
            rH = (byte)(rH & ~(1 << 7));
            return 8;
        }

        private int res_7_l()
        {
            rL = (byte)(rL & ~(1 << 7));
            return 8;
        }

        private int res_7_phl()
        {
            ram[rHL] = (byte)(ram[rHL] & ~(1 << 7));
            return 16;
        }



        private int set_0_a()
        {
            rA = (byte)(rA | (1 << 0));
            return 8;
        }

        private int set_0_b()
        {
            rB = (byte)(rB | (1 << 0));
            return 8;
        }

        private int set_0_c()
        {
            rC = (byte)(rC | (1 << 0));
            return 8;
        }

        private int set_0_d()
        {
            rD = (byte)(rD | (1 << 0));
            return 8;
        }

        private int set_0_e()
        {
            rE = (byte)(rE | (1 << 0));
            return 8;
        }

        private int set_0_h()
        {
            rH = (byte)(rH | (1 << 0));
            return 8;
        }

        private int set_0_l()
        {
            rL = (byte)(rL | (1 << 0));
            return 8;
        }

        private int set_0_phl()
        {
            ram[rHL] = (byte)(ram[rHL] | (1 << 0));
            return 16;
        }


        private int set_1_a()
        {
            rA = (byte)(rA | (1 << 1));
            return 8;
        }

        private int set_1_b()
        {
            rB = (byte)(rB | (1 << 1));
            return 8;
        }

        private int set_1_c()
        {
            rC = (byte)(rC | (1 << 1));
            return 8;
        }

        private int set_1_d()
        {
            rD = (byte)(rD | (1 << 1));
            return 8;
        }

        private int set_1_e()
        {
            rE = (byte)(rE | (1 << 1));
            return 8;
        }

        private int set_1_h()
        {
            rH = (byte)(rH | (1 << 1));
            return 8;
        }

        private int set_1_l()
        {
            rL = (byte)(rL | (1 << 1));
            return 8;
        }

        private int set_1_phl()
        {
            ram[rHL] = (byte)(ram[rHL] | (1 << 1));
            return 16;
        }


        private int set_2_a()
        {
            rA = (byte)(rA | (1 << 2));
            return 8;
        }

        private int set_2_b()
        {
            rB = (byte)(rB | (1 << 2));
            return 8;
        }

        private int set_2_c()
        {
            rC = (byte)(rC | (1 << 2));
            return 8;
        }

        private int set_2_d()
        {
            rD = (byte)(rD | (1 << 2));
            return 8;
        }

        private int set_2_e()
        {
            rE = (byte)(rE | (1 << 2));
            return 8;
        }

        private int set_2_h()
        {
            rH = (byte)(rH | (1 << 2));
            return 8;
        }

        private int set_2_l()
        {
            rL = (byte)(rL | (1 << 2));
            return 8;
        }

        private int set_2_phl()
        {
            ram[rHL] = (byte)(ram[rHL] | (1 << 2));
            return 16;
        }


        private int set_3_a()
        {
            rA = (byte)(rA | (1 << 3));
            return 8;
        }

        private int set_3_b()
        {
            rB = (byte)(rB | (1 << 3));
            return 8;
        }

        private int set_3_c()
        {
            rC = (byte)(rC | (1 << 3));
            return 8;
        }

        private int set_3_d()
        {
            rD = (byte)(rD | (1 << 3));
            return 8;
        }

        private int set_3_e()
        {
            rE = (byte)(rE | (1 << 3));
            return 8;
        }

        private int set_3_h()
        {
            rH = (byte)(rH | (1 << 3));
            return 8;
        }

        private int set_3_l()
        {
            rL = (byte)(rL | (1 << 3));
            return 8;
        }

        private int set_3_phl()
        {
            ram[rHL] = (byte)(ram[rHL] | (1 << 3));
            return 16;
        }


        private int set_4_a()
        {
            rA = (byte)(rA | (1 << 4));
            return 8;
        }

        private int set_4_b()
        {
            rB = (byte)(rB | (1 << 4));
            return 8;
        }

        private int set_4_c()
        {
            rC = (byte)(rC | (1 << 4));
            return 8;
        }

        private int set_4_d()
        {
            rD = (byte)(rD | (1 << 4));
            return 8;
        }

        private int set_4_e()
        {
            rE = (byte)(rE | (1 << 4));
            return 8;
        }

        private int set_4_h()
        {
            rH = (byte)(rH | (1 << 4));
            return 8;
        }

        private int set_4_l()
        {
            rL = (byte)(rL | (1 << 4));
            return 8;
        }

        private int set_4_phl()
        {
            ram[rHL] = (byte)(ram[rHL] | (1 << 4));
            return 16;
        }


        private int set_5_a()
        {
            rA = (byte)(rA | (1 << 5));
            return 8;
        }

        private int set_5_b()
        {
            rB = (byte)(rB | (1 << 5));
            return 8;
        }

        private int set_5_c()
        {
            rC = (byte)(rC | (1 << 5));
            return 8;
        }

        private int set_5_d()
        {
            rD = (byte)(rD | (1 << 5));
            return 8;
        }

        private int set_5_e()
        {
            rE = (byte)(rE | (1 << 5));
            return 8;
        }

        private int set_5_h()
        {
            rH = (byte)(rH | (1 << 5));
            return 8;
        }

        private int set_5_l()
        {
            rL = (byte)(rL | (1 << 5));
            return 8;
        }

        private int set_5_phl()
        {
            ram[rHL] = (byte)(ram[rHL] | (1 << 5));
            return 16;
        }


        private int set_6_a()
        {
            rA = (byte)(rA | (1 << 6));
            return 8;
        }

        private int set_6_b()
        {
            rB = (byte)(rB | (1 << 6));
            return 8;
        }

        private int set_6_c()
        {
            rC = (byte)(rC | (1 << 6));
            return 8;
        }

        private int set_6_d()
        {
            rD = (byte)(rD | (1 << 6));
            return 8;
        }

        private int set_6_e()
        {
            rE = (byte)(rE | (1 << 6));
            return 8;
        }

        private int set_6_h()
        {
            rH = (byte)(rH | (1 << 6));
            return 8;
        }

        private int set_6_l()
        {
            rL = (byte)(rL | (1 << 6));
            return 8;
        }

        private int set_6_phl()
        {
            ram[rHL] = (byte)(ram[rHL] | (1 << 6));
            return 16;
        }


        private int set_7_a()
        {
            rA = (byte)(rA | (1 << 7));
            return 8;
        }

        private int set_7_b()
        {
            rB = (byte)(rB | (1 << 7));
            return 8;
        }

        private int set_7_c()
        {
            rC = (byte)(rC | (1 << 7));
            return 8;
        }

        private int set_7_d()
        {
            rD = (byte)(rD | (1 << 7));
            return 8;
        }

        private int set_7_e()
        {
            rE = (byte)(rE | (1 << 7));
            return 8;
        }

        private int set_7_h()
        {
            rH = (byte)(rH | (1 << 7));
            return 8;
        }

        private int set_7_l()
        {
            rL = (byte)(rL | (1 << 7));
            return 8;
        }

        private int set_7_phl()
        {
            ram[rHL] = (byte)(ram[rHL] | (1 << 7));
            return 16;
        }

        private int sla_a()
        {
            fC = (rA & 0x80) == 0x80;

            rA = (byte)(rA << 1);

            fZ = rA == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sla_b()
        {
            fC = (rB & 0x80) == 0x80;

            rB = (byte)(rB << 1);

            fZ = rB == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sla_c()
        {
            fC = (rC & 0x80) == 0x80;

            rC = (byte)(rC << 1);

            fZ = rC == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sla_d()
        {
            fC = (rD & 0x80) == 0x80;

            rD = (byte)(rD << 1);

            fZ = rD == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sla_e()
        {
            fC = (rE & 0x80) == 0x80;

            rE = (byte)(rE << 1);

            fZ = rE == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sla_h()
        {
            fC = (rH & 0x80) == 0x80;

            rH = (byte)(rH << 1);

            fZ = rH == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sla_l()
        {
            fC = (rL & 0x80) == 0x80;

            rL = (byte)(rL << 1);

            fZ = rL == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sla_phl()
        {
            fC = (ram[rHL] & 0x80) == 0x80;

            ram[rHL] = (byte)(ram[rHL] << 1);

            fZ = ram[rHL] == 0;
            fN = false;
            fH = false;

            return 16;
        }

        private int sra_a()
        {
            fC = (rA & 0x01) == 0x01;

            rA = (byte)(rA >> 1);
            if ((rA & 0x40) == 0x40) rA |= 0x80;

            fZ = rA == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sra_b()
        {
            fC = (rB & 0x01) == 0x01;

            rB = (byte)(rB >> 1);
            if ((rB & 0x40) == 0x40) rB |= 0x80;

            fZ = rB == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sra_c()
        {
            fC = (rC & 0x01) == 0x01;

            rC = (byte)(rC >> 1);
            if ((rC & 0x40) == 0x40) rC |= 0x80;

            fZ = rC == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sra_d()
        {
            fC = (rD & 0x01) == 0x01;

            rD = (byte)(rD >> 1);
            if ((rD & 0x40) == 0x40) rD |= 0x80;

            fZ = rD == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sra_e()
        {
            fC = (rE & 0x01) == 0x01;

            rE = (byte)(rE >> 1);
            if ((rE & 0x40) == 0x40) rE |= 0x80;

            fZ = rE == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sra_h()
        {
            fC = (rH & 0x01) == 0x01;

            rH = (byte)(rH >> 1);
            if ((rH & 0x40) == 0x40) rH |= 0x80;

            fZ = rH == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sra_l()
        {
            fC = (rL & 0x01) == 0x01;

            rL = (byte)(rL >> 1);
            if ((rL & 0x40) == 0x40) rL |= 0x80;

            fZ = rL == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int sra_phl()
        {
            fC = (ram[rHL] & 0x01) == 0x01;

            ram[rHL] = (byte)(ram[rHL] >> 1);
            if ((ram[rHL] & 0x40) == 0x40) ram[rHL] |= 0x80;

            fZ = ram[rHL] == 0;
            fN = false;
            fH = false;

            return 16;
        }

        private int err_a()
        {
            bool oldFC = fC;

            fC = (rA & 0x01) == 0x01;
            rA = (byte)(rA >> 1);
            if (oldFC) rA |= 0x80;

            fZ = rA == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rr_b()
        {
            bool oldFC = fC;

            fC = (rB & 0x01) == 0x01;
            rB = (byte)(rB >> 1);
            if (oldFC) rB |= 0x80;

            fZ = rB == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rr_c()
        {
            bool oldFC = fC;

            fC = (rC & 0x01) == 0x01;
            rC = (byte)(rC >> 1);
            if (oldFC) rC |= 0x80;

            fZ = rC == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rr_d()
        {
            bool oldFC = fC;

            fC = (rD & 0x01) == 0x01;
            rD = (byte)(rD >> 1);
            if (oldFC) rD |= 0x80;

            fZ = rD == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rr_e()
        {
            bool oldFC = fC;

            fC = (rE & 0x01) == 0x01;
            rE = (byte)(rE >> 1);
            if (oldFC) rE |= 0x80;

            fZ = rE == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rr_h()
        {
            bool oldFC = fC;

            fC = (rH & 0x01) == 0x01;
            rH = (byte)(rH >> 1);
            if (oldFC) rH |= 0x80;

            fZ = rH == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rr_l()
        {
            bool oldFC = fC;

            fC = (rL & 0x01) == 0x01;
            rL = (byte)(rL >> 1);
            if (oldFC) rL |= 0x80;

            fZ = rL == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rr_phl()
        {
            bool oldFC = fC;

            fC = (ram[rHL] & 0x01) == 0x01;
            ram[rHL] = (byte)(ram[rHL] >> 1);
            if (oldFC) ram[rHL] |= 0x80;

            fZ = ram[rHL] == 0;
            fN = false;
            fH = false;

            return 16;
        }

        private int erl_a()
        {
            bool oldFC = fC;

            fC = (rA & 0x80) == 0x80;
            rA = (byte)(rA << 1);
            if (oldFC) rA |= 0x01;

            fZ = rA == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rl_b()
        {
            bool oldFC = fC;

            fC = (rB & 0x80) == 0x80;
            rB = (byte)(rB << 1);
            if (oldFC) rB |= 0x01;

            fZ = rB == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rl_c()
        {
            bool oldFC = fC;

            fC = (rC & 0x80) == 0x80;
            rC = (byte)(rC << 1);
            if (oldFC) rC |= 0x01;

            fZ = rC == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rl_d()
        {
            bool oldFC = fC;

            fC = (rD & 0x80) == 0x80;
            rD = (byte)(rD << 1);
            if (oldFC) rD |= 0x01;

            fZ = rD == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rl_e()
        {
            bool oldFC = fC;

            fC = (rE & 0x80) == 0x80;
            rE = (byte)(rE << 1);
            if (oldFC) rE |= 0x01;

            fZ = rE == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rl_h()
        {
            bool oldFC = fC;

            fC = (rH & 0x80) == 0x80;
            rH = (byte)(rH << 1);
            if (oldFC) rH |= 0x01;

            fZ = rH == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rl_l()
        {
            bool oldFC = fC;

            fC = (rL & 0x80) == 0x80;
            rL = (byte)(rL << 1);
            if (oldFC) rL |= 0x01;

            fZ = rL == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rl_phl()
        {
            bool oldFC = fC;

            fC = (ram[rHL] & 0x80) == 0x80;
            ram[rHL] = (byte)(ram[rHL] << 1);
            if (oldFC) ram[rHL] |= 0x01;

            fZ = ram[rHL] == 0;
            fN = false;
            fH = false;

            return 16;
        }

        private int srl_a()
        {
            fC = (rA & 0x01) == 0x01;

            rA = (byte)(rA >> 1);

            fZ = rA == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int srl_b()
        {
            fC = (rB & 0x01) == 0x01;

            rB = (byte)(rB >> 1);

            fZ = rB == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int srl_c()
        {
            fC = (rC & 0x01) == 0x01;

            rC = (byte)(rC >> 1);

            fZ = rC == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int srl_d()
        {
            fC = (rD & 0x01) == 0x01;

            rD = (byte)(rD >> 1);

            fZ = rD == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int srl_e()
        {
            fC = (rE & 0x01) == 0x01;

            rE = (byte)(rE >> 1);

            fZ = rE == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int srl_h()
        {
            fC = (rH & 0x01) == 0x01;

            rH = (byte)(rH >> 1);

            fZ = rH == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int srl_l()
        {
            fC = (rL & 0x01) == 0x01;

            rL = (byte)(rL >> 1);

            fZ = rL == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int srl_phl()
        {
            fC = (ram[rHL] & 0x01) == 0x01;

            ram[rHL] = (byte)(ram[rHL] >> 1);

            fZ = ram[rHL] == 0;
            fN = false;
            fH = false;

            return 16;
        }

        private int erlc_a()
        {
            fC = (rA & 0x80) == 0x80;

            rA = (byte)(rA << 1);
            if (fC) rA |= 0x01;

            fZ = rA == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rlc_b()
        {
            fC = (rB & 0x80) == 0x80;

            rB = (byte)(rB << 1);
            if (fC) rB |= 0x01;

            fZ = rB == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rlc_c()
        {
            fC = (rC & 0x80) == 0x80;

            rC = (byte)(rC << 1);
            if (fC) rC |= 0x01;

            fZ = rC == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rlc_d()
        {
            fC = (rD & 0x80) == 0x80;

            rD = (byte)(rD << 1);
            if (fC) rD |= 0x01;

            fZ = rD == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rlc_e()
        {
            fC = (rE & 0x80) == 0x80;

            rE = (byte)(rE << 1);
            if (fC) rE |= 0x01;

            fZ = rE == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rlc_h()
        {
            fC = (rH & 0x80) == 0x80;

            rH = (byte)(rH << 1);
            if (fC) rH |= 0x01;

            fZ = rH == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rlc_l()
        {
            fC = (rL & 0x80) == 0x80;

            rL = (byte)(rL << 1);
            if (fC) rL |= 0x01;

            fZ = rL == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rlc_phl()
        {
            fC = (ram[rHL] & 0x80) == 0x80;

            ram[rHL] = (byte)(ram[rHL] << 1);
            if (fC) ram[rHL] |= 0x01;

            fZ = ram[rHL] == 0;
            fN = false;
            fH = false;

            return 16;
        }

        private int errc_a()
        {
            fC = (rA & 0x01) == 0x01;

            rA = (byte)(rA >> 1);
            if (fC) rA |= 0x80;

            fZ = rA == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rrc_b()
        {
            fC = (rB & 0x01) == 0x01;

            rB = (byte)(rB >> 1);
            if (fC) rB |= 0x80;

            fZ = rB == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rrc_c()
        {
            fC = (rC & 0x01) == 0x01;

            rC = (byte)(rC >> 1);
            if (fC) rC |= 0x80;

            fZ = rC == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rrc_d()
        {
            fC = (rD & 0x01) == 0x01;

            rD = (byte)(rD >> 1);
            if (fC) rD |= 0x80;

            fZ = rD == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rrc_e()
        {
            fC = (rE & 0x01) == 0x01;

            rE = (byte)(rE >> 1);
            if (fC) rE |= 0x80;

            fZ = rE == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rrc_h()
        {
            fC = (rH & 0x01) == 0x01;

            rH = (byte)(rH >> 1);
            if (fC) rH |= 0x80;

            fZ = rH == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rrc_l()
        {
            fC = (rL & 0x01) == 0x01;

            rL = (byte)(rL >> 1);
            if (fC) rL |= 0x80;

            fZ = rL == 0;
            fN = false;
            fH = false;

            return 8;
        }

        private int rrc_phl()
        {
            fC = (ram[rHL] & 0x01) == 0x01;

            ram[rHL] = (byte)(ram[rHL] >> 1);
            if (fC) ram[rHL] |= 0x80;

            fZ = ram[rHL] == 0;
            fN = false;
            fH = false;

            return 16;
        }

        private int bit_0_a()
        {
            fZ = (rA & (1 << 0)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_0_b()
        {
            fZ = (rB & (1 << 0)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_0_c()
        {
            fZ = (rC & (1 << 0)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_0_d()
        {
            fZ = (rD & (1 << 0)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_0_e()
        {
            fZ = (rE & (1 << 0)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_0_h()
        {
            fZ = (rH & (1 << 0)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_0_l()
        {
            fZ = (rL & (1 << 0)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_0_phl()
        {
            fZ = (ram[rHL] & (1 << 0)) == 0;
            fN = false;
            fH = true;
            return 12;
        }


        private int bit_1_a()
        {
            fZ = (rA & (1 << 1)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_1_b()
        {
            fZ = (rB & (1 << 1)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_1_c()
        {
            fZ = (rC & (1 << 1)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_1_d()
        {
            fZ = (rD & (1 << 1)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_1_e()
        {
            fZ = (rE & (1 << 1)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_1_h()
        {
            fZ = (rH & (1 << 1)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_1_l()
        {
            fZ = (rL & (1 << 1)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_1_phl()
        {
            fZ = (ram[rHL] & (1 << 1)) == 0;
            fN = false;
            fH = true;
            return 12;
        }


        private int bit_2_a()
        {
            fZ = (rA & (1 << 2)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_2_b()
        {
            fZ = (rB & (1 << 2)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_2_c()
        {
            fZ = (rC & (1 << 2)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_2_d()
        {
            fZ = (rD & (1 << 2)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_2_e()
        {
            fZ = (rE & (1 << 2)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_2_h()
        {
            fZ = (rH & (1 << 2)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_2_l()
        {
            fZ = (rL & (1 << 2)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_2_phl()
        {
            fZ = (ram[rHL] & (1 << 2)) == 0;
            fN = false;
            fH = true;
            return 12;
        }


        private int bit_3_a()
        {
            fZ = (rA & (1 << 3)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_3_b()
        {
            fZ = (rB & (1 << 3)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_3_c()
        {
            fZ = (rC & (1 << 3)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_3_d()
        {
            fZ = (rD & (1 << 3)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_3_e()
        {
            fZ = (rE & (1 << 3)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_3_h()
        {
            fZ = (rH & (1 << 3)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_3_l()
        {
            fZ = (rL & (1 << 3)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_3_phl()
        {
            fZ = (ram[rHL] & (1 << 3)) == 0;
            fN = false;
            fH = true;
            return 12;
        }


        private int bit_4_a()
        {
            fZ = (rA & (1 << 4)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_4_b()
        {
            fZ = (rB & (1 << 4)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_4_c()
        {
            fZ = (rC & (1 << 4)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_4_d()
        {
            fZ = (rD & (1 << 4)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_4_e()
        {
            fZ = (rE & (1 << 4)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_4_h()
        {
            fZ = (rH & (1 << 4)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_4_l()
        {
            fZ = (rL & (1 << 4)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_4_phl()
        {
            fZ = (ram[rHL] & (1 << 4)) == 0;
            fN = false;
            fH = true;
            return 12;
        }


        private int bit_5_a()
        {
            fZ = (rA & (1 << 5)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_5_b()
        {
            fZ = (rB & (1 << 5)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_5_c()
        {
            fZ = (rC & (1 << 5)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_5_d()
        {
            fZ = (rD & (1 << 5)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_5_e()
        {
            fZ = (rE & (1 << 5)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_5_h()
        {
            fZ = (rH & (1 << 5)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_5_l()
        {
            fZ = (rL & (1 << 5)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_5_phl()
        {
            fZ = (ram[rHL] & (1 << 5)) == 0;
            fN = false;
            fH = true;
            return 12;
        }


        private int bit_6_a()
        {
            fZ = (rA & (1 << 6)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_6_b()
        {
            fZ = (rB & (1 << 6)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_6_c()
        {
            fZ = (rC & (1 << 6)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_6_d()
        {
            fZ = (rD & (1 << 6)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_6_e()
        {
            fZ = (rE & (1 << 6)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_6_h()
        {
            fZ = (rH & (1 << 6)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_6_l()
        {
            fZ = (rL & (1 << 6)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_6_phl()
        {
            fZ = (ram[rHL] & (1 << 6)) == 0;
            fN = false;
            fH = true;
            return 12;
        }


        private int bit_7_a()
        {
            fZ = (rA & (1 << 7)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_7_b()
        {
            fZ = (rB & (1 << 7)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_7_c()
        {
            fZ = (rC & (1 << 7)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_7_d()
        {
            fZ = (rD & (1 << 7)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_7_e()
        {
            fZ = (rE & (1 << 7)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_7_h()
        {
            fZ = (rH & (1 << 7)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_7_l()
        {
            fZ = (rL & (1 << 7)) == 0;
            fN = false;
            fH = true;
            return 8;
        }

        private int bit_7_phl()
        {
            fZ = (ram[rHL] & (1 << 7)) == 0;
            fN = false;
            fH = true;
            return 12;
        }
    }
}