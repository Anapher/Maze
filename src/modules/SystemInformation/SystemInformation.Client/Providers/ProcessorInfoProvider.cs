using System.Collections.Generic;
using System.Linq;
using System.Management;
using SystemInformation.Client.Utilities;
using SystemInformation.Shared;
using SystemInformation.Shared.Dtos;
using SystemInformation.Shared.Dtos.Value;

namespace SystemInformation.Client.Providers
{
    public class ProcessorInfoProvider : ISystemInfoProvider
    {
        public IEnumerable<SystemInfoDto> FetchInformation()
        {
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor"))
            using (var results = searcher.Get())
            {
                foreach (var managementObject in results.Cast<ManagementObject>())
                    if (managementObject.TryGetProperty("Name", out string name))
                    {
                        var info = new SystemInfoDto {Category = SystemInfoCategory.Processor, Name = name, Value = HeaderValueDto.Instance};
                        AddProcessorProperties(managementObject, info.Childs);
                        yield return info;
                    }
            }
        }

        private void AddProcessorProperties(ManagementObject managementObject, IList<SystemInfoDto> result)
        {
            result.TryAdd<ushort>(SystemInfoCategory.Processor, managementObject, "AddressWidth");
            result.TryAdd<ushort>(SystemInfoCategory.Processor, managementObject, "Architecture",
                arg => new TextValueDto(((ProcessorArchitecture) arg).GetDescription()));

            result.TryAdd<string>(SystemInfoCategory.Processor, managementObject, "Description");
            result.TryAdd<uint>(SystemInfoCategory.Processor, managementObject, "MaxClockSpeed", i => new TextValueDto($"{i} MHz"));
            result.TryAdd<ushort>(SystemInfoCategory.Processor, managementObject, "CurrentVoltage", i => new TextValueDto($"{(i & 254) / 10d} V"));
            result.TryAdd<uint>(SystemInfoCategory.Processor, managementObject, "ExtClock", i => new TextValueDto($"{i} MHz"));
            result.TryAdd<uint>(SystemInfoCategory.Processor, managementObject, "L2CacheSize", i => new DataSizeValueDto(i * 1024));
            result.TryAdd<uint>(SystemInfoCategory.Processor, managementObject, "L3CacheSize", i => new DataSizeValueDto(i * 1024));
            result.TryAdd<string>(SystemInfoCategory.Processor, managementObject, "Manufacturer");
            result.TryAdd<uint>(SystemInfoCategory.Processor, managementObject, "NumberOfCores");
            result.TryAdd<uint>(SystemInfoCategory.Processor, managementObject, "NumberOfLogicalProcessors");
            result.TryAdd<string>(SystemInfoCategory.Processor, managementObject, "ProcessorId");
            result.TryAdd<ushort>(SystemInfoCategory.Processor, managementObject, "ProcessorType",
                arg => new TextValueDto(((ProcessorType) arg).GetDescription()));
            result.TryAdd<string>(SystemInfoCategory.Processor, managementObject, "SerialNumber");
            result.TryAdd<string>(SystemInfoCategory.Processor, managementObject, "SocketDesignation");
            result.TryAdd<string>(SystemInfoCategory.Processor, managementObject, "Version");
        }

        private enum ProcessorArchitecture
        {
            x86 = 0,
            MIPS = 1,
            Alpha = 2,
            PowerPC = 3,
            ARM = 5,
            ItaniumBasedSystems = 6,
            x64 = 9
        }

        private enum ProcessorType
        {
            Other = 1,
            Unknown = 2,
            CentralProcessor = 3,
            MathProcessor = 4,
            DSPProcessor = 5,
            VideoProcessor = 6
        }
    }
}