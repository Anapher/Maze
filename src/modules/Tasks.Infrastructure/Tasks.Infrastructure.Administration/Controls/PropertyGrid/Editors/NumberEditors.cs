namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class ByteEditor : UpDownEditor<byte>
    {
        public ByteEditor() : base(byte.MinValue, byte.MaxValue)
        {
        }
    }

    public class SByteEditor : UpDownEditor<sbyte>
    {
        public SByteEditor() : base(sbyte.MinValue, sbyte.MaxValue)
        {
        }
    }

    public class ShortEditor : UpDownEditor<short>
    {
        public ShortEditor() : base(short.MinValue, short.MaxValue)
        {
        }
    }

    public class UShortEditor : UpDownEditor<ushort>
    {
        public UShortEditor() : base(ushort.MinValue, ushort.MaxValue)
        {
        }
    }

    public class IntEditor : UpDownEditor<int>
    {
        public IntEditor() : base(int.MinValue, int.MaxValue)
        {
        }
    }

    public class UIntEditor : UpDownEditor<uint>
    {
        public UIntEditor() : base(uint.MinValue, uint.MaxValue)
        {
        }
    }

    public class LongEditor: UpDownEditor<long>
    {
        public LongEditor() : base(long.MinValue, long.MaxValue)
        {
        }
    }

    public class ULongEditor : UpDownEditor<ulong>
    {
        public ULongEditor() : base(ulong.MinValue, ulong.MaxValue)
        {
        }
    }

    public class FloatEditor : UpDownEditor<float>
    {
        public FloatEditor() : base(float.MinValue, float.MaxValue, true)
        {
        }
    }

    public class DoubleEditor : UpDownEditor<double>
    {
        public DoubleEditor() : base(double.MinValue, double.MaxValue, true)
        {
        }
    }

    public class DecimalEditor : UpDownEditor<decimal>
    {
        public DecimalEditor() : base(decimal.ToDouble(decimal.MinValue), decimal.ToDouble(decimal.MaxValue), true)
        {
        }
    }
}