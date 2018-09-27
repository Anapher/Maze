using Newtonsoft.Json;
using RegistryEditor.Shared.Converters;

namespace RegistryEditor.Shared.Dtos
{
    [JsonConverter(typeof(RegistryValueConverter))]
    public abstract class RegistryValueDto
    {
        public string Name { get; set; }
        public abstract RegistryValueType Type { get; }
    }

    public class StringRegistryValueDto : RegistryValueDto
    {
        public string Value { get; set; }
        public override RegistryValueType Type { get; } = RegistryValueType.String;
    }

    public class BinaryRegistryValueDto : RegistryValueDto
    {
        public byte[] Value { get; set; }
        public override RegistryValueType Type { get; } = RegistryValueType.Binary;
    }

    public class DWordRegistryValueDto : RegistryValueDto
    {
        public uint Value { get; set; }
        public override RegistryValueType Type { get; } = RegistryValueType.DWord;
    }

    public class QWordRegistryValueDto : RegistryValueDto
    {
        public ulong Value { get; set; }
        public override RegistryValueType Type { get; } = RegistryValueType.QWord;
    }

    public class MultiStringRegistryValueDto : RegistryValueDto
    {
        public string[] Value { get; set; }
        public override RegistryValueType Type { get; } = RegistryValueType.MultiString;
    }

    public class ExpandableStringRegistryValueDto : RegistryValueDto
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