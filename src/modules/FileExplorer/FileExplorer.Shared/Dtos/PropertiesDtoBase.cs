using System;
using System.IO;

namespace FileExplorer.Shared.Dtos
{
    public abstract class PropertiesDtoBase
    {
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset LastAccessTime { get; set; }
        public DateTimeOffset LastWriteTime { get; set; }
        public FileAttributes Attributes { get; set; }
    }
}