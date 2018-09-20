using System.Collections.Generic;
using System.Linq;
using System.Management;
using SystemInformation.Client.Utilities;
using SystemInformation.Shared;
using SystemInformation.Shared.Dtos;
using SystemInformation.Shared.Dtos.Value;

namespace SystemInformation.Client.Providers
{
    public class ComputerSystemProvider : ISystemInfoProvider
    {
        public IEnumerable<SystemInfoDto> FetchInformation()
        {
            var list = new List<SystemInfoDto>();
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_ComputerSystem"))
            using (var results = searcher.Get())
            {
                var managementObject = results.Cast<ManagementObject>().Single();

                list.TryAdd<ushort>(SystemInfoCategories.ComputerSystem, managementObject, "AdminPasswordStatus",
                    arg => new TextValueDto(((AdminPasswordStatus) arg).GetDescription()));
                list.TryAdd<string>(SystemInfoCategories.ComputerSystem, managementObject, "BootupState");
                list.TryAdd<short>(SystemInfoCategories.ComputerSystem, managementObject, "CurrentTimeZone");
                list.TryAdd<bool>(SystemInfoCategories.ComputerSystem, managementObject, "DaylightInEffect");
                list.TryAdd<string>(SystemInfoCategories.ComputerSystem, managementObject, "DNSHostName");
                list.TryAdd<string>(SystemInfoCategories.ComputerSystem, managementObject, "Domain");
                list.TryAdd<string>(SystemInfoCategories.ComputerSystem, managementObject, "Manufacturer");
                list.TryAdd<string>(SystemInfoCategories.ComputerSystem, managementObject, "Model");
                list.TryAdd<uint>(SystemInfoCategories.ComputerSystem, managementObject, "NumberOfLogicalProcessors");
                list.TryAdd<uint>(SystemInfoCategories.ComputerSystem, managementObject, "NumberOfProcessors");
                list.TryAdd<ushort>(SystemInfoCategories.ComputerSystem, managementObject, "PCSystemType",
                    arg => new TextValueDto(((PcSystemType) arg).GetDescription()));
                list.TryAdd<ushort>(SystemInfoCategories.ComputerSystem, managementObject, "PowerSupplyState",
                    arg => new TextValueDto(((PowerSupplyState) arg).GetDescription()));
                list.TryAdd<string>(SystemInfoCategories.ComputerSystem, managementObject, "PrimaryOwnerName");
                list.TryAdd<ulong>(SystemInfoCategories.ComputerSystem, managementObject, "TotalPhysicalMemory");
                list.TryAdd<string>(SystemInfoCategories.ComputerSystem, managementObject, "UserName");
                list.TryAdd<string>(SystemInfoCategories.ComputerSystem, managementObject, "Workgroup");
                list.TryAdd<string>(SystemInfoCategories.ComputerSystem, managementObject, "SystemType");
            }

            return list;
        }

        private enum AdminPasswordStatus
        {
            Disabled = 0,
            Enabled = 1,
            NotImplemented = 2,
            Unknown = 3
        }

        private enum PcSystemType
        {
            Unspecified = 0,
            Desktop = 1,
            Mobile = 2,
            Workstation = 3,
            EnterpriseServer = 4,
            SOHOServer = 5,
            AppliancePC = 6,
            PerformanceServer = 7,
            Maximum = 8
        }

        private enum PowerSupplyState
        {
            Other = 1,
            Unknown = 2,
            Safe = 3,
            Warning = 4,
            Critical = 5,
            NonRecoverable = 6
        }
    }
}