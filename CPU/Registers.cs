﻿namespace Gusboy
{
    /// <summary>
    /// General registers that don't belong in another category.
    /// </summary>
    public partial class CPU
    {
#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable IDE1006 // Naming Styles

        // 16-bit registers
        private Gshort rSP = 0xFFFE;

        // 8-bit registers
        private byte rA = 0x01;
        private byte rF = 0xB0;
        private byte rB = 0x00;
        private byte rC = 0x13;
        private byte rD = 0x00;
        private byte rE = 0xD8;
        private byte rH = 0x01;
        private byte rL = 0x4D;

        public bool fHalt { get; set; }

        public bool fStop { get; set; }

        public int rPC { get; set; }

        // 16-bit registers
        private int rAF
        {
            get => (this.rA << 8) | this.rF;

            set
            {
                this.rA = (byte)(value >> 8);
                this.rF = (byte)value;
            }
        }

        private int rBC
        {
            get => (this.rB << 8) | this.rC;

            set
            {
                this.rB = (byte)(value >> 8);
                this.rC = (byte)value;
            }
        }

        private int rDE
        {
            get => (this.rD << 8) | this.rE;

            set
            {
                this.rD = (byte)(value >> 8);
                this.rE = (byte)value;
            }
        }

        private int rHL
        {
            get => (this.rH << 8) | this.rL;

            set
            {
                this.rH = (byte)(value >> 8);
                this.rL = (byte)value;
            }
        }

        // Flags
        private bool fZ
        {
            get => this.GetFlag(7);
            set => this.SetFlag(value, 7);
        }

        private bool fN
        {
            get => this.GetFlag(6);
            set => this.SetFlag(value, 6);
        }

        private bool fH
        {
            get => this.GetFlag(5);
            set => this.SetFlag(value, 5);
        }

        private bool fC
        {
            get => this.GetFlag(4);
            set => this.SetFlag(value, 4);
        }

        private bool GetFlag(int i) => (this.rF & (1 << i)) != 0;

        private void SetFlag(bool value, int i)
        {
            if (value)
            {
                this.rF |= (byte)(1 << i);
            }
            else
            {
                this.rF &= (byte)(~(1 << i));
            }
        }
    }
}