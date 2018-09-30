namespace ClipboardManager.Shared.Dtos
{
    public class ClipboardData
    {
        public ClipboardDataFormat Format { get; set; }
        public string Value { get; set; }
        public ClipboardValueType ValueType { get; set; }

        protected bool Equals(ClipboardData other) => Format == other.Format && string.Equals(Value, other.Value) && ValueType == other.ValueType;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ClipboardData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Format;
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) ValueType;
                return hashCode;
            }
        }
    }

    public enum ClipboardValueType
    {
        String,
        StringList,
        Image
    }
}