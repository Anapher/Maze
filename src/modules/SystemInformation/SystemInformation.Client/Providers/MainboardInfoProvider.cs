using System.Collections.Generic;
using System.Linq;
using System.Management;
using SystemInformation.Client.Utilities;
using SystemInformation.Shared;
using SystemInformation.Shared.Dtos;

namespace SystemInformation.Client.Providers
{
    public class MainboardInfoProvider : ISystemInfoProvider
    {
        public IEnumerable<SystemInfoDto> FetchInformation()
        {
            var list = new List<SystemInfoDto>();
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard"))
            using (var results = searcher.Get())
            {
                var managementObject = results.Cast<ManagementObject>().Single();

                list.TryAdd<bool>(SystemInfoCategories.Mainboard, managementObject, "HostingBoard");
                list.TryAdd<bool>(SystemInfoCategories.Mainboard, managementObject, "HotSwappable");
                list.TryAdd<string>(SystemInfoCategories.Mainboard, managementObject, "Manufacturer");
                list.TryAdd<string>(SystemInfoCategories.Mainboard, managementObject, "Product");
                list.TryAdd<bool>(SystemInfoCategories.Mainboard, managementObject, "Removable");
                list.TryAdd<bool>(SystemInfoCategories.Mainboard, managementObject, "Replacable");
                list.TryAdd<string>(SystemInfoCategories.Mainboard, managementObject, "SerialNumber");
            }

            return list;
        }
    }
}