using System;

namespace FileExplorer.Shared.Dtos
{
    public class FileProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public FilePropertyGroup Group { get; set; }
        public FilePropertyValueType ValueType { get; set; }

        public Guid? FormatId { get; set; }
        public int? PropertyId { get; set; }
    }
}