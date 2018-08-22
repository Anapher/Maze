using System;
using FileExplorer.Shared.Converters;
using Newtonsoft.Json;

namespace FileExplorer.Shared.Dtos
{
    [JsonConverter(typeof(FileExplorerEntryConverter))]
    public abstract class FileExplorerEntry
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTimeOffset LastAccess { get; set; }
        public DateTimeOffset CreationTime { get; set; }

        public abstract FileExplorerEntryType Type { get; }
    }
}