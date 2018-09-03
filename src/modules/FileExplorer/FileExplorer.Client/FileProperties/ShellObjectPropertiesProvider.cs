using System;
using System.Collections.Generic;
using System.IO;
using FileExplorer.Shared.Dtos;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace FileExplorer.Client.FileProperties
{
    public class ShellObjectPropertiesProvider : PropertiesProviderBase, IFilePropertyValueProvider
    {
        public IEnumerable<FileProperty> ProvideValues(FileInfo fileInfo, FilePropertiesDto dto)
        {
            using (var shellObject = ShellObject.FromParsingName(fileInfo.FullName))
            {
                if (shellObject == null)
                    yield break;

                foreach (var property in shellObject.Properties.DefaultPropertyCollection)
                {
                    if (string.IsNullOrEmpty(property.CanonicalName))
                        continue;

                    var (valueString, valueType) = ObjectToString(property.ValueAsObject);
                    if (string.IsNullOrEmpty(valueString))
                        continue;

                    yield return new FileProperty
                    {
                        FormatId = property.PropertyKey.FormatId,
                        PropertyId = property.PropertyKey.PropertyId,
                        Name = property.CanonicalName,
                        Value = valueString,
                        ValueType = valueType,
                        Group = GetGroup(property)
                    };
                }
            }
        }

        private static FilePropertyGroup GetGroup(IShellProperty shellProperty)
        {
            var split = shellProperty.CanonicalName.Split('.');
            if (split.Length >= 3 && Enum.TryParse(split[1], true, out FilePropertyGroup group))
                return group;

            return FilePropertyGroup.Details;
        }
    }
}