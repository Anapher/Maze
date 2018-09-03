using System.Collections.Generic;

namespace FileExplorer.Shared.Dtos
{
    public class FilePropertiesDto : PropertiesDtoBase
    {
        public string OpenWithProgramPath { get; set; }
        public string OpenWithProgramName { get; set; }
        public long Size { get; set; }
        public long SizeOnDisk { get; set; }
        
        public IList<FileProperty> Properties { get; set; }
    }
}