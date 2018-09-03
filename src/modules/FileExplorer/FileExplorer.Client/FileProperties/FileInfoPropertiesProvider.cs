using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Client.FileProperties
{
    public class FileInfoPropertiesProvider : IFilePropertyValueProvider
    {
        public IEnumerable<FileProperty> ProvideValues(FileInfo fileInfo, FilePropertiesDto dto)
        {
            dto.Size = fileInfo.Length;
            dto.CreationTime = fileInfo.CreationTimeUtc;
            dto.LastAccessTime = fileInfo.LastAccessTimeUtc;
            dto.LastWriteTime = fileInfo.LastWriteTimeUtc;
            dto.Attributes = fileInfo.Attributes;

            return Enumerable.Empty<FileProperty>();
        }
    }
}