using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Client.FileProperties
{
    public class FileVersionInfoProvider : PropertiesProviderBase, IFilePropertyValueProvider
    {
        public IEnumerable<FileProperty> ProvideValues(FileInfo fileInfo, FilePropertiesDto dto)
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(fileInfo.FullName);
            foreach (var propertyInfo in typeof(FileVersionInfo).GetProperties())
            {
                if (propertyInfo.Name.EndsWith("Part"))
                    continue;

                var value = propertyInfo.GetValue(fileVersionInfo, null);
                if (value == null)
                    continue;

                if (propertyInfo.PropertyType == typeof(bool) && !(bool) value)
                    continue;

                var (valueString, valueType) = ObjectToString(value);
                if (string.IsNullOrEmpty(valueString))
                    continue;

                yield return new FileProperty
                {
                    Name = propertyInfo.Name,
                    Value = valueString,
                    ValueType = valueType,
                    Group = FilePropertyGroup.Details
                };
            }
        }
    }
}