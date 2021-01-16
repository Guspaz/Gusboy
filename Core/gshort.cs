namespace GusBoy
{
    /// <summary>
    /// Basically just a ushort with access to the hi and lo bytes and some extra implicit conversions
    /// Changed to treat it more like int because .NET insists on converting everything to int when you do operations on stuff
    /// </summary>
    public struct gshort //: IFormattable, IConvertible, IComparable<UInt16>, IEquatable<UInt16>
    {
        // TODO: Add direct operators to maybe be faster than the automatic int conversions

        private ushort var;

        public gshort(int u)
        {
            this.var = (ushort)u;
        }

        public byte Lo
        {
            get => (byte)(this.var & 0x00FF);
            set => this.var = (ushort)((this.var & 0xFF00) | value);
        }

        public byte Hi
        {
            get => (byte)(this.var >> 8);
            set => this.var = (ushort)((this.var & 0x00FF) | (value << 8));
        }

        public bool this[int i]
        {
            get => (this.var & (1 << i)) != 0;
            set
            {
                if ( value )
                {
                    this.var = (ushort)(this.var | (1 << i));
                }
                else
                {
                    this.var = (ushort)(this.var & ~(1 << i));
                }
            }
        }

        public static implicit operator gshort(int i)
        {
            return new gshort(i);
        }

        public static implicit operator int(gshort g)
        {
            return g.var;
        }

        //public static implicit operator gshort(ushort u)
        //{
        //    return new gshort(u);
        //}

        //public static explicit operator sbyte (gshort g)
        //{
        //    return (sbyte)g.var;
        //}

        //public override string ToString()
        //{
        //    return var.ToString();
        //}

        //public override bool Equals(object obj)
        //{
        //    return var.Equals(obj);
        //}

        //public override int GetHashCode()
        //{
        //    return var.GetHashCode();
        //}

        //public string ToString(string format, IFormatProvider formatProvider)
        //{
        //    return var.ToString(format, formatProvider);
        //}

        //public int CompareTo(object obj)
        //{
        //    return var.CompareTo(obj);
        //}

        //public TypeCode GetTypeCode()
        //{
        //    return var.GetTypeCode();
        //}

        //public bool ToBoolean(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToBoolean(provider);
        //}

        //public char ToChar(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToChar(provider);
        //}

        //public sbyte ToSByte(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToSByte(provider);
        //}

        //public byte ToByte(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToByte(provider);
        //}

        //public short ToInt16(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToInt16(provider);
        //}

        //public ushort ToUInt16(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToUInt16(provider);
        //}

        //public int ToInt32(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToInt32(provider);
        //}

        //public uint ToUInt32(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToUInt32(provider);
        //}

        //public long ToInt64(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToInt64(provider);
        //}

        //public ulong ToUInt64(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToUInt64(provider);
        //}

        //public float ToSingle(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToSingle(provider);
        //}

        //public double ToDouble(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToDouble(provider);
        //}

        //public decimal ToDecimal(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToDecimal(provider);
        //}

        //public DateTime ToDateTime(IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToDateTime(provider);
        //}

        //public string ToString(IFormatProvider provider)
        //{
        //    return var.ToString(provider);
        //}

        //public object ToType(Type conversionType, IFormatProvider provider)
        //{
        //    return ((IConvertible)var).ToType(conversionType, provider);
        //}

        //public int CompareTo(ushort other)
        //{
        //    return var.CompareTo(other);
        //}

        //public bool Equals(ushort other)
        //{
        //    return var.Equals(other);
        //}
    }
}
