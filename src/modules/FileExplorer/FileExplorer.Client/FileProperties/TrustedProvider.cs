using System.Collections.Generic;
using System.IO;
using FileExplorer.Client.Extensions;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Client.FileProperties
{
    public class TrustedProvider : IFilePropertyValueProvider
    {
        public IEnumerable<FileProperty> ProvideValues(FileInfo fileInfo, FilePropertiesDto dto)
        {
            yield return new FileProperty
            {
                Name = "IsTrusted",
                Value = AuthenticodeTools.IsTrusted(fileInfo.FullName).ToString(),
                Group = FilePropertyGroup.Executable
            };
        }
    }
}