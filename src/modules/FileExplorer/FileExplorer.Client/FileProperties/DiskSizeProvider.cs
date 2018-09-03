using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using FileExplorer.Client.Native;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Client.FileProperties
{
    public class DiskSizeProvider : IFilePropertyValueProvider
    {
        public IEnumerable<FileProperty> ProvideValues(FileInfo fileInfo, FilePropertiesDto dto)
        {
            dto.SizeOnDisk = GetFileSizeOnDisk(fileInfo);
            return Enumerable.Empty<FileProperty>();
        }

        public static long GetFileSizeOnDisk(FileInfo fileInfo)
        {
            var result = NativeMethods.GetDiskFreeSpaceW(fileInfo.Directory.Root.FullName, out var sectorsPerCluster,
                out var bytesPerSector, out _, out _);

            if (result == 0)
                throw new Win32Exception();

            var clusterSize = sectorsPerCluster * bytesPerSector;
            var losize = NativeMethods.GetCompressedFileSizeW(fileInfo.FullName, out var hosize);

            var size = ((long) hosize << 32) | losize;
            return (size + clusterSize - 1) / clusterSize * clusterSize;
        }
    }
}