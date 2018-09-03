using System.Collections.Generic;
using System.IO;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Client.FileProperties
{
    public interface IFilePropertyValueProvider
    {
        IEnumerable<FileProperty> ProvideValues(FileInfo fileInfo, FilePropertiesDto dto);
    }
}