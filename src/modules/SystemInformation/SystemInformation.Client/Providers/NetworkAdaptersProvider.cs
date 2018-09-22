using System.Collections.Generic;
using System.Linq;
using System.Management;
using SystemInformation.Client.Utilities;
using SystemInformation.Shared;
using SystemInformation.Shared.Dtos;
using SystemInformation.Shared.Dtos.Value;

namespace SystemInformation.Client.Providers
{
    public class NetworkAdaptersProvider : ISystemInfoProvider
    {
        public IEnumerable<SystemInfoDto> FetchInformation()
        {
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_NetworkAdapter"))
            using (var results = searcher.Get())
            {
                foreach (var managementObject in results.Cast<ManagementObject>())
                    if (managementObject.TryGetProperty("Caption", out string caption))
                    {
                        var networkAdapter = new SystemInfoDto
                        {
                            Name = caption, Category = SystemInfoCategory.Network, Value = HeaderValueDto.Instance
                        };
                        networkAdapter.Childs.AddRange(GetNetworkAdapterProperties(managementObject));
                        yield return networkAdapter;
                    }
            }
        }

        private IEnumerable<SystemInfoDto> GetNetworkAdapterProperties(ManagementObject managementObject)
        {
            var result = new List<SystemInfoDto>();

            result.TryAdd<string>(SystemInfoCategory.Network, managementObject, "AdapterType");
            result.TryAdd<uint>(SystemInfoCategory.Network, managementObject, "Availability", u => new TextValueDto(((AdapterAvailability)u).GetDescription()));
            result.TryAdd<string>(SystemInfoCategory.Network, managementObject, "Description");
            result.TryAdd<string>(SystemInfoCategory.Network, managementObject, "MACAddress");
            result.TryAdd<string>(SystemInfoCategory.Network, managementObject, "Manufacturer");
            result.TryAdd<string>(SystemInfoCategory.Network, managementObject, "Name");
            result.TryAdd<bool>(SystemInfoCategory.Network, managementObject, "NetEnabled");
            result.TryAdd<bool>(SystemInfoCategory.Network, managementObject, "PhyscialAdapter");
            result.TryAdd<string>(SystemInfoCategory.Network, managementObject, "PNPDeviceID");
            result.TryAdd<string>(SystemInfoCategory.Network, managementObject, "ServiceName");
            result.TryAdd<ulong>(SystemInfoCategory.Network, managementObject, "Speed", i => new DataSizeValueDto((long) (i / 8)));
            result.TryAddDateTime(SystemInfoCategory.Network, managementObject, "TimeOfLastReset");

            return result;
        }

        private enum AdapterAvailability
        {
            Other = 1,
            Unknown = 2,
            Running = 3,
            Warning = 4,
            InTest = 5,
            NotApplicable = 6,
            PowerOff = 7,
            OffLine = 8,
            OffDuty = 9,
            Degraded = 10,
            NotInstalled = 11,
            InstallError = 12,
            PowerSaveUnknown = 13,
            PowerSaveLowPowerMode = 14,
            PowerSaveStandby = 15,
            PowerCycle = 16,
            PowerSaveWarning = 17,
            Paused = 18,
            NotReady = 19,
            NotConfigured = 20,
            Quiesced = 21
        }
    }
}