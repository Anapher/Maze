namespace ClipboardManager.Shared.Dtos
{
    public class ClipboardData
    {
        public ClipboardDataFormat Format { get; set; }
        public string Value { get; set; }
        public ClipboardValueType ValueType { get; set; }
    }

    public enum ClipboardValueType
    {
        String,
        StringList,
        Image
    }
}