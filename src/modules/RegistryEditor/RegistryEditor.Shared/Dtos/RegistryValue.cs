using Newtonsoft.Json;
using RegistryEditor.Shared.Converters;

namespace RegistryEditor.Shared.Dtos
{
    [JsonConverter(typeof(RegistryValueConverter))]
    public abstract class RegistryValue
    {
        public string Name { get; set; }
        public abstract RegistryValueType Type { get; }
    }

    public class StringRegistryValue : RegistryValue
    {
        public string Value { get; set; }
        public override RegistryValueType Type { get; } = RegistryValueType.String;
    }

    public class BinaryRegistryValue : RegistryValue
    {
        public byte[] Value { get; set; }
        public override RegistryValueType Type { get; } = RegistryValueType.Binary;
    }

    public class DWordRegistryValue : RegistryValue
    {
        public uint Value { get; set; }
        public override RegistryValueType Type { get; } = RegistryValueType.DWord;
    }

    public class QWordRegistryValue : RegistryValue
    {
        public ulong Value { get; set; }
        public override RegistryValueType Type { get; } = RegistryValueType.QWord;
    }

    public class MultiStringRegistryValue : RegistryValue
    {
        public string[] Value { get; set; }
        public override RegistryValueType Type { get; } = RegistryValueType.MultiString;
    }

    public class ExpandableStringRegistryValue : RegistryValue
    {
        public string Value { get; set; }
        public override RegistryValueType Type { get; } = RegistryValueType.ExpandableString;
    }

    public enum RegistryValueType
    {
        String,
        Binary,
        DWord,
        QWord,
        MultiString,
        ExpandableString
    }
}