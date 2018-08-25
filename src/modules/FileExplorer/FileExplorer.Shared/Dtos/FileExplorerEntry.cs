using System;
using FileExplorer.Shared.Converters;
using Newtonsoft.Json;

namespace FileExplorer.Shared.Dtos
{
    [JsonConverter(typeof(FileExplorerEntryConverter))]
    public abstract class FileExplorerEntry
    {
        public string Name { get; set; }
        public string Path { get; set; } // Parent == null ? Name : System.IO.Path.Combine(Parent.Path, Name);
        public DateTimeOffset LastAccess { get; set; }
        public DateTimeOffset CreationTime { get; set; }

        public DirectoryEntry Parent { get; set; }

        public abstract FileExplorerEntryType Type { get; }
    }
}