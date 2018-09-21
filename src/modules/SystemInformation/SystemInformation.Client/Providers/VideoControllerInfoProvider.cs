using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using SystemInformation.Client.Utilities;
using SystemInformation.Shared;
using SystemInformation.Shared.Dtos;
using SystemInformation.Shared.Dtos.Value;

namespace SystemInformation.Client.Providers
{
    public class VideoControllerInfoProvider : ISystemInfoProvider
    {
        public IEnumerable<SystemInfoDto> FetchInformation()
        {
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController"))
            using (var results = searcher.Get())
            {
                foreach (var managementObject in results.Cast<ManagementObject>())
                {
                    if (managementObject.TryGetProperty("Caption", out string caption))
                    {
                        var info = new SystemInfoDto {Category = SystemInfoCategories.VideoCard, Name = caption, Value = HeaderValueDto.Instance};
                        AddVideoCardProperties(managementObject, info.Childs);
                        yield return info;
                    }
                }
            }
        }

        private void AddVideoCardProperties(ManagementObject managementObject, IList<SystemInfoDto> result)
        {
            result.TryAdd<string>(SystemInfoCategories.VideoCard, managementObject, "AdapterCompatibility");
            result.TryAdd<string>(SystemInfoCategories.VideoCard, managementObject, "AdapterDACType");
            result.TryAdd<uint>(SystemInfoCategories.VideoCard, managementObject, "CurrentRefreshRate");
            result.TryAddDateTime(SystemInfoCategories.VideoCard, managementObject, "DriverDate");
            result.TryAdd<string>(SystemInfoCategories.VideoCard, managementObject, "DriverVersion");
            result.TryAdd<uint>(SystemInfoCategories.VideoCard, managementObject, "MaxRefreshRate");
            result.TryAdd<uint>(SystemInfoCategories.VideoCard, managementObject, "MinRefreshRate");
            result.TryAdd<bool>(SystemInfoCategories.VideoCard, managementObject, "Monochrome");
            result.TryAdd<ushort>(SystemInfoCategories.VideoCard, managementObject, "VideoArchitecture",
                arg => new TextValueDto(((VideoArchitecture) arg).GetDescription()));
            result.TryAdd<ushort>(SystemInfoCategories.VideoCard, managementObject, "VideoMemoryType",
                arg => new TextValueDto(((VideoMemoryType) arg).GetDescription()));
            result.TryAdd<string>(SystemInfoCategories.VideoCard, managementObject, "VideoModeDescription");
            result.TryAdd<string>(SystemInfoCategories.VideoCard, managementObject, "VideoProcessor");
        }

        private enum VideoArchitecture
        {
            Other = 1,
            Unknown = 2,
            CGA = 3,
            EGA = 4,
            VGA = 5,
            SVGA = 6,
            MDA = 7,
            HGC = 8,
            MCGA = 9,
            [Description("8514A")]
            V8514A = 10,
            XGA = 11,
            LinearFrameBuffer = 12,
            [Description("PC-98")]
            PC98 = 160
        }

        private enum VideoMemoryType
        {
            Other = 1,
            Unknown = 2,
            VRAM = 3,
            DRAM = 4,
            SRAM = 5,
            WRAM = 6,
            [Description("EDO RAM")]
            EDORAM = 7,
            BurstSynchronousDRAM = 8,
            PipelinedBurstSRAM = 9,
            CDRAM = 10,
            [Description("3DRAM")]
            V3DRAM = 11,
            SDRAM = 12,
            SGRAM = 13
        }
    }
}