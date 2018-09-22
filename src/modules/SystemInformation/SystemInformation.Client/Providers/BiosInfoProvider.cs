using System.Collections.Generic;
using System.Linq;
using System.Management;
using SystemInformation.Client.Utilities;
using SystemInformation.Shared;
using SystemInformation.Shared.Dtos;
using SystemInformation.Shared.Dtos.Value;

namespace SystemInformation.Client.Providers
{
    public class BiosInfoProvider : ISystemInfoProvider
    {
        public IEnumerable<SystemInfoDto> FetchInformation()
        {
            var list = new List<SystemInfoDto>();
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS"))
            using (var results = searcher.Get())
            {
                var managementObject = results.Cast<ManagementObject>().First();
                list.TryAdd<string>(SystemInfoCategory.Bios, managementObject, "Name");
                list.TryAdd<string>(SystemInfoCategory.Bios, managementObject, "Version");
                list.TryAdd<string>(SystemInfoCategory.Bios, managementObject, "CurrentLanguage");
                list.TryAdd<string>(SystemInfoCategory.Bios, managementObject, "InstallableLanguages");
                list.TryAddDateTime(SystemInfoCategory.Bios, managementObject, "ReleaseDate");
                list.TryAdd<string>(SystemInfoCategory.Bios, managementObject, "SerialNumber");

                if (managementObject.TryGetProperty("SystemBiosMajorVersion", out byte majorVersion))
                {
                    managementObject.TryGetProperty("SystemBiosMinorVersion", out byte minorVersion);
                    list.Add(new SystemInfoDto
                    {
                        Category = SystemInfoCategory.Bios,
                        Name = "@Bios.SystemBiosVersion",
                        Value = new TextValueDto($"{majorVersion}.{minorVersion}")
                    });
                }
            }

            return list;
        }
    }
}