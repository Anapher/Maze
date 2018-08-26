using System;
using FileExplorer.Shared.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace FileExplorer.Shared.Converters
{
    internal class FileExplorerEntryConverter : CustomCreationConverter<FileExplorerEntry>
    {
        private FileExplorerEntryType _type;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var jobj = JToken.ReadFrom(reader);
            _type = jobj["type"].ToObject<FileExplorerEntryType>();

            return base.ReadJson(jobj.CreateReader(), objectType, existingValue, serializer);
        }

        public override FileExplorerEntry Create(Type objectType)
        {
            switch (_type)
            {
                case FileExplorerEntryType.File:
                    return new FileEntry();
                case FileExplorerEntryType.Directory:
                    return new DirectoryEntry();
                case FileExplorerEntryType.Drive:
                    return new DriveDirectoryEntry();
                case FileExplorerEntryType.SpecialDirectory:
                    return new SpecialDirectoryEntry();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}