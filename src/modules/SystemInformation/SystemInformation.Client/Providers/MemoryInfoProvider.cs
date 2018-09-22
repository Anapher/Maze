using System.Collections.Generic;
using System.Linq;
using System.Management;
using SystemInformation.Client.Utilities;
using SystemInformation.Shared;
using SystemInformation.Shared.Dtos;
using SystemInformation.Shared.Dtos.Value;

namespace SystemInformation.Client.Providers
{
    public class MemoryInfoProvider : ISystemInfoProvider
    {
        public IEnumerable<SystemInfoDto> FetchInformation()
        {
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory"))
            using (var results = searcher.Get())
            {
                foreach (var managementObject in results.Cast<ManagementObject>())
                    if (managementObject.TryGetProperty("Tag", out string name))
                    {
                        var info = new SystemInfoDto {Category = SystemInfoCategory.Memory, Name = name, Value = HeaderValueDto.Instance};
                        AddMemoryProperties(managementObject, info.Childs);
                        yield return info;
                    }
            }
        }

        private void AddMemoryProperties(ManagementObject managementObject, IList<SystemInfoDto> result)
        {
            result.TryAdd<string>(SystemInfoCategory.Memory, managementObject, "BankLabel");
            result.TryAdd<ulong>(SystemInfoCategory.Memory, managementObject, "Capacity", arg => new DataSizeValueDto((long) arg));
            result.TryAdd<uint>(SystemInfoCategory.Memory, managementObject, "ConfiguredClockSpeed", u => new TextValueDto($"{u} MHz"));
            result.TryAdd<uint>(SystemInfoCategory.Memory, managementObject, "ConfiguredVoltage", s => new TextValueDto($"{s / 1000d} V"));
            result.TryAdd<ushort>(SystemInfoCategory.Memory, managementObject, "FormFactor",
                s => new TextValueDto(((FormFactor) s).GetDescription()));
            result.TryAdd<bool>(SystemInfoCategory.Memory, managementObject, "HotSwappable");
            result.TryAdd<string>(SystemInfoCategory.Memory, managementObject, "Manufacturer");
            result.TryAdd<string>(SystemInfoCategory.Memory, managementObject, "Model");
            result.TryAdd<string>(SystemInfoCategory.Memory, managementObject, "PartNumber");
            result.TryAdd<string>(SystemInfoCategory.Memory, managementObject, "SerialNumber");
            result.TryAdd<uint>(SystemInfoCategory.Memory, managementObject, "Speed");
        }

        private enum FormFactor
        {
            Unknown = 0,
            Other = 1,
            SIP = 2,
            DIP = 3,
            ZIP = 4,
            SOJ = 5,
            Proprietary = 6,
            SIMM = 7,
            DIMM = 8,
            TSOP = 9,
            PGA = 10,
            RIMM = 11,
            SODIMM = 12,
            SRIMM = 13,
            SMD = 14,
            SSMP = 15,
            QFP = 16,
            TQFP = 17,
            SOIC = 18,
            LCC = 19,
            PLCC = 20,
            BGA = 21,
            FPBGA = 22,
            LGA = 23
        }
    }
}