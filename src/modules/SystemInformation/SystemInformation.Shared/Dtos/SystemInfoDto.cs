using System.Collections.Generic;
using SystemInformation.Shared.Dtos.Value;

namespace SystemInformation.Shared.Dtos
{
    public class SystemInfoDto
    {
        public string Name { get; set; }
        public ValueDto Value { get; set; }
        public List<SystemInfoDto> Childs { get; set; } = new List<SystemInfoDto>();
        public string Category { get; set; }
    }
}