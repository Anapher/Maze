using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Management;
using SystemInformation.Client.Utilities;
using SystemInformation.Shared;
using SystemInformation.Shared.Dtos;
using SystemInformation.Shared.Dtos.Value;

namespace SystemInformation.Client.Providers
{
    public class OperatingSystemProvider : ISystemInfoProvider
    {
        public IEnumerable<SystemInfoDto> FetchInformation()
        {
            var list = new List<SystemInfoDto>();
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem"))
            using (var results = searcher.Get())
            {
                var managementObject = results.Cast<ManagementObject>().Single();
                list.TryAdd<string>(SystemInfoCategories.OperatingSystem, managementObject, "BuildNumber");
                list.TryAdd<string>(SystemInfoCategories.OperatingSystem, managementObject, "BuildType");
                list.TryAdd<string>(SystemInfoCategories.OperatingSystem, managementObject, "Caption");
                list.Add(new SystemInfoDto
                {
                    Category = SystemInfoCategories.OperatingSystem,
                    Name = "@OperatingSystem.SystemLanguage",
                    Value = new CultureValueDto(CultureInfo.InstalledUICulture)
                });
                list.TryAdd<string>(SystemInfoCategories.OperatingSystem, managementObject, "CSName");
                list.TryAdd<bool>(SystemInfoCategories.OperatingSystem, managementObject, "Debug");
                list.TryAdd<uint>(SystemInfoCategories.OperatingSystem, managementObject, "Encryption");
                list.TryAdd<ulong>(SystemInfoCategories.OperatingSystem, managementObject, "FreeSpaceInPagingFiles",
                    arg => new DataSizeValueDto((long) arg * 1000));
                list.TryAddDateTime(SystemInfoCategories.OperatingSystem, managementObject, "InstallDate");
                list.TryAddDateTime(SystemInfoCategories.OperatingSystem, managementObject, "LastBootUpTime");
                list.TryAdd<string>(SystemInfoCategories.OperatingSystem, managementObject, "Manufacturer");
                list.TryAdd<uint>(SystemInfoCategories.OperatingSystem, managementObject, "MaxNumberOfProcesses");
                list.TryAdd<ulong>(SystemInfoCategories.OperatingSystem, managementObject, "MaxProcessMemorySize",
                    arg => new DataSizeValueDto((long) arg * 1000));
                list.TryAdd<uint>(SystemInfoCategories.OperatingSystem, managementObject, "NumberOfLicensedUsers");
                list.TryAdd<uint>(SystemInfoCategories.OperatingSystem, managementObject, "NumberOfProcesses");
                list.TryAdd<uint>(SystemInfoCategories.OperatingSystem, managementObject, "NumberOfUsers");
                list.TryAdd<uint>(SystemInfoCategories.OperatingSystem, managementObject, "OperatingSystemSKU", u =>
                {
                    var sku = (OperatingSystemSku) u;
                    var attribute = sku.GetAttribute<OperatingSystemSku, DescriptionAttribute>();
                    return new TextValueDto(attribute.Description);
                });
                list.TryAdd<string>(SystemInfoCategories.OperatingSystem, managementObject, "OSArchitecture");
                list.TryAdd<string>(SystemInfoCategories.OperatingSystem, managementObject, "SerialNumber");
                list.TryAdd<string>(SystemInfoCategories.OperatingSystem, managementObject, "SystemDirectory");
                list.TryAdd<string>(SystemInfoCategories.OperatingSystem, managementObject, "WindowsDirectory");
            }

            list.Add(new SystemInfoDto
            {
                Category = SystemInfoCategories.OperatingSystem,
                Name = "@OperatingSystem.Version",
                Value = new TextValueDto(Environment.OSVersion.ToString())
            });

            return list;
        }
    }

    public enum OperatingSystemSku
    {
        [Description("Business")] PRODUCT_BUSINESS = 0x00000006,
        [Description("Business N")] PRODUCT_BUSINESS_N = 0x00000010,
        [Description("HPC Edition")] PRODUCT_CLUSTER_SERVER = 0x00000012,
        [Description("Server Hyper Core V")] PRODUCT_CLUSTER_SERVER_V = 0x00000040,
        [Description("Windows 10 Home")] PRODUCT_CORE = 0x00000065,
        [Description("Windows 10 Home China")] PRODUCT_CORE_COUNTRYSPECIFIC = 0x00000063,
        [Description("Windows 10 Home N")] PRODUCT_CORE_N = 0x00000062,

        [Description("Windows 10 Home Single Language")]
        PRODUCT_CORE_SINGLELANGUAGE = 0x00000064,

        [Description("Server Datacenter (evaluation installation)")]
        PRODUCT_DATACENTER_EVALUATION_SERVER = 0x00000050,

        [Description("Server Datacenter (full installation)")]
        PRODUCT_DATACENTER_SERVER = 0x00000008,

        [Description("Server Datacenter (core installation)")]
        PRODUCT_DATACENTER_SERVER_CORE = 0x0000000C,

        [Description("Server Datacenter without Hyper-V (core installation)")]
        PRODUCT_DATACENTER_SERVER_CORE_V = 0x00000027,

        [Description("Server Datacenter without Hyper-V (full installation)")]
        PRODUCT_DATACENTER_SERVER_V = 0x00000025,
        [Description("Windows 10 Education")] PRODUCT_EDUCATION = 0x00000079,

        [Description("Windows 10 Education N")]
        PRODUCT_EDUCATION_N = 0x0000007A,
        [Description("Windows 10 Enterprise")] PRODUCT_ENTERPRISE = 0x00000004,

        [Description("Windows 10 Enterprise E")]
        PRODUCT_ENTERPRISE_E = 0x00000046,

        [Description("Windows 10 Enterprise Evaluation")]
        PRODUCT_ENTERPRISE_EVALUATION = 0x00000048,

        [Description("Windows 10 Enterprise N")]
        PRODUCT_ENTERPRISE_N = 0x0000001B,

        [Description("Windows 10 Enterprise N Evaluation")]
        PRODUCT_ENTERPRISE_N_EVALUATION = 0x00000054,

        [Description("Windows 10 Enterprise 2015 LTSB")]
        PRODUCT_ENTERPRISE_S = 0x0000007D,

        [Description("Windows 10 Enterprise 2015 LTSB Evaluation")]
        PRODUCT_ENTERPRISE_S_EVALUATION = 0x00000081,

        [Description("Windows 10 Enterprise 2015 LTSB N")]
        PRODUCT_ENTERPRISE_S_N = 0x0000007E,

        [Description("Windows 10 Enterprise 2015 LTSB N Evaluation")]
        PRODUCT_ENTERPRISE_S_N_EVALUATION = 0x00000082,

        [Description("Server Enterprise (full installation)")]
        PRODUCT_ENTERPRISE_SERVER = 0x0000000A,

        [Description("Server Enterprise (core installation)")]
        PRODUCT_ENTERPRISE_SERVER_CORE = 0x0000000E,

        [Description("Server Enterprise without Hyper-V (core installation) ")]
        PRODUCT_ENTERPRISE_SERVER_CORE_V = 0x00000029,

        [Description("Server Enterprise for Itanium-based Systems")]
        PRODUCT_ENTERPRISE_SERVER_IA64 = 0x0000000F,

        [Description("Server Enterprise without Hyper-V (full installation)")]
        PRODUCT_ENTERPRISE_SERVER_V = 0x00000026,

        [Description("Windows Essential Server Solution Additional")]
        PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL = 0x0000003C,

        [Description("Windows Essential Server Solution Additional SVC")]
        PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC = 0x0000003E,

        [Description("Windows Essential Server Solution Management")]
        PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT = 0x0000003B,

        [Description("Windows Essential Server Solution Management SVC")]
        PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC = 0x0000003D,
        [Description("Home Basic ")] PRODUCT_HOME_BASIC = 0x00000002,
        [Description("Not supported")] PRODUCT_HOME_BASIC_E = 0x00000043,
        [Description("Home Basic N")] PRODUCT_HOME_BASIC_N = 0x00000005,
        [Description("Home Premium")] PRODUCT_HOME_PREMIUM = 0x00000003,
        [Description("Not supported")] PRODUCT_HOME_PREMIUM_E = 0x00000044,
        [Description("Home Premium N")] PRODUCT_HOME_PREMIUM_N = 0x0000001A,

        [Description("Windows Home Server 2011")]
        PRODUCT_HOME_PREMIUM_SERVER = 0x00000022,

        [Description("Windows Storage Server 2008 R2 Essentials")]
        PRODUCT_HOME_SERVER = 0x00000013,

        [Description("Microsoft Hyper-V Server")]
        PRODUCT_HYPERV = 0x0000002A,
        [Description("Windows 10 IoT Core")] PRODUCT_IOTUAP = 0x0000007B,

        [Description("Windows 10 IoT Core Commercial")]
        PRODUCT_IOTUAPCOMMERCIAL = 0x00000083,

        [Description("Windows Essential Business Server Management Server")]
        PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT = 0x0000001E,

        [Description("Windows Essential Business Server Messaging Server")]
        PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING = 0x00000020,

        [Description(" Windows Essential Business Server Security Server")]
        PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY = 0x0000001F,
        [Description("Windows 10 Mobile")] PRODUCT_MOBILE_CORE = 0x00000068,

        [Description("Windows 10 Mobile Enterprise")]
        PRODUCT_MOBILE_ENTERPRISE = 0x00000085,

        [Description("Windows MultiPoint Server Premium (full installation)")]
        PRODUCT_MULTIPOINT_PREMIUM_SERVER = 0x0000004D,

        [Description("Windows MultiPoint Server Standard (full installation)")]
        PRODUCT_MULTIPOINT_STANDARD_SERVER = 0x0000004C,

        [Description("Windows 10 Pro for Workstations")]
        PRODUCT_PRO_WORKSTATION = 0x000000A1,

        [Description("Windows 10 Pro for Workstations N")]
        PRODUCT_PRO_WORKSTATION_N = 0x000000A2,
        [Description("Windows 10  Pro")] PRODUCT_PROFESSIONAL = 0x00000030,
        [Description(" Not supported")] PRODUCT_PROFESSIONAL_E = 0x00000045,
        [Description(" Windows 10 Pro N")] PRODUCT_PROFESSIONAL_N = 0x00000031,

        [Description("Professional with Media Center")]
        PRODUCT_PROFESSIONAL_WMC = 0x00000067,

        [Description("Windows Small Business Server 2011 Essentials")]
        PRODUCT_SB_SOLUTION_SERVER = 0x00000032,

        [Description("Server For SB Solutions EM")]
        PRODUCT_SB_SOLUTION_SERVER_EM = 0x00000036,

        [Description("Server For SB Solutions")]
        PRODUCT_SERVER_FOR_SB_SOLUTIONS = 0x00000033,

        [Description("Server For SB Solutions EM")]
        PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM = 0x00000037,

        [Description("Windows Server 2008 for Windows Essential Server Solutions")]
        PRODUCT_SERVER_FOR_SMALLBUSINESS = 0x00000018,

        [Description("Windows Server 2008 without Hyper-V for Windows Essential Server Solutions")]
        PRODUCT_SERVER_FOR_SMALLBUSINESS_V = 0x00000023,
        [Description("Server Foundation")] PRODUCT_SERVER_FOUNDATION = 0x00000021,

        [Description("Windows Small Business Server")]
        PRODUCT_SMALLBUSINESS_SERVER = 0x00000009,

        [Description("Small Business Server Premium")]
        PRODUCT_SMALLBUSINESS_SERVER_PREMIUM = 0x00000019,

        [Description("Small Business Server Premium (core installation)")]
        PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE = 0x0000003F,

        [Description("Windows MultiPoint Server")]
        PRODUCT_SOLUTION_EMBEDDEDSERVER = 0x00000038,

        [Description("Server Standard (evaluation installation)")]
        PRODUCT_STANDARD_EVALUATION_SERVER = 0x0000004F,
        [Description("Server Standard")] PRODUCT_STANDARD_SERVER = 0x00000007,

        [Description("Server Standard (core installation)")]
        PRODUCT_STANDARD_SERVER_CORE = 0x0000000D,

        [Description("Server Standard without Hyper-V (core installation)")]
        PRODUCT_STANDARD_SERVER_CORE_V = 0x00000028,

        [Description("Server Standard without Hyper-V")]
        PRODUCT_STANDARD_SERVER_V = 0x00000024,

        [Description("Server Solutions Premium ")]
        PRODUCT_STANDARD_SERVER_SOLUTIONS = 0x00000034,

        [Description("Server Solutions Premium (core installation)")]
        PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE = 0x00000035,
        [Description("Starter")] PRODUCT_STARTER = 0x0000000B,
        [Description("Not supported")] PRODUCT_STARTER_E = 0x00000042,
        [Description("Starter N")] PRODUCT_STARTER_N = 0x0000002F,

        [Description("Storage Server Enterprise")]
        PRODUCT_STORAGE_ENTERPRISE_SERVER = 0x00000017,

        [Description("Storage Server Enterprise (core installation)")]
        PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE = 0x0000002E,

        [Description("Storage Server Express")]
        PRODUCT_STORAGE_EXPRESS_SERVER = 0x00000014,

        [Description("Storage Server Express (core installation)")]
        PRODUCT_STORAGE_EXPRESS_SERVER_CORE = 0x0000002B,

        [Description("Storage Server Standard (evaluation installation)")]
        PRODUCT_STORAGE_STANDARD_EVALUATION_SERVER = 0x00000060,

        [Description("Storage Server Standard")]
        PRODUCT_STORAGE_STANDARD_SERVER = 0x00000015,

        [Description("Storage Server Standard (core installation)")]
        PRODUCT_STORAGE_STANDARD_SERVER_CORE = 0x0000002C,

        [Description("Storage Server Workgroup (evaluation installation)")]
        PRODUCT_STORAGE_WORKGROUP_EVALUATION_SERVER = 0x0000005F,

        [Description("Storage Server Workgroup")]
        PRODUCT_STORAGE_WORKGROUP_SERVER = 0x00000016,

        [Description("Storage Server Workgroup (core installation)")]
        PRODUCT_STORAGE_WORKGROUP_SERVER_CORE = 0x0000002D,
        [Description("Ultimate")] PRODUCT_ULTIMATE = 0x00000001,
        [Description("Not supported")] PRODUCT_ULTIMATE_E = 0x00000047,
        [Description("Ultimate N")] PRODUCT_ULTIMATE_N = 0x0000001C,
        [Description("An unknown product")] PRODUCT_UNDEFINED = 0x00000000,

        [Description("Web Server (full installation)")]
        PRODUCT_WEB_SERVER = 0x00000011,

        [Description("Web Server (core installation)")]
        PRODUCT_WEB_SERVER_CORE = 0x0000001D
    }
}