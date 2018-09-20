using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using SystemInformation.Client.Utilities;
using SystemInformation.Shared;
using SystemInformation.Shared.Dtos;
using SystemInformation.Shared.Dtos.Value;

namespace SystemInformation.Client.Providers
{
    public class DrivesProvider : ISystemInfoProvider
    {
        public IEnumerable<SystemInfoDto> FetchInformation()
        {
            var processedLogicalDrives = new HashSet<string>();

            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive"))
            using (var results = searcher.Get())
            {
                foreach (var managementObject in results.Cast<ManagementObject>())
                    if (managementObject.TryGetProperty("Caption", out string caption))
                    {
                        var driveRoot = new SystemInfoDto {Category = SystemInfoCategories.Drives, Name = caption, Value = HeaderValueDto.Instance};
                        driveRoot.Childs.AddRange(GetDiskDriveProperties(managementObject));

                        var deviceId = (string) managementObject.Properties["DeviceID"].Value;

                        foreach (var (partition, partitionDeviceId) in QueryPartitions(deviceId))
                        {
                            driveRoot.Childs.Add(partition);

                            foreach (var (logicalDrive, logicalDriveId) in QueryLogicalDrives(partitionDeviceId))
                            {
                                partition.Childs.Add(logicalDrive);
                                processedLogicalDrives.Add(logicalDriveId);
                            }
                        }

                        yield return driveRoot;
                    }
            }

            //find missing logical disks
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_LogicalDisk"))
            using (var results = searcher.Get())
            {
                foreach (var managementObject in results.Cast<ManagementObject>())
                {
                    var deviceId = (string) managementObject.Properties["DeviceID"].Value;
                    if (processedLogicalDrives.Contains(deviceId))
                        continue;

                    if (managementObject.TryGetProperty("Caption", out string caption))
                    {
                        var driveRoot = new SystemInfoDto {Name = caption, Value = HeaderValueDto.Instance};
                        driveRoot.Childs.AddRange(GetLogicalDriveProperties(managementObject));

                        yield return driveRoot;
                    }
                }
            }
        }

        public IEnumerable<(SystemInfoDto, string deviceId)> QueryLogicalDrives(string deviceId)
        {
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2",
                $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{deviceId}'}} WHERE ASSOCCLASS=Win32_LogicalDiskToPartition"))
            using (var results = searcher.Get())
            {
                foreach (var managementObject in results.Cast<ManagementObject>())
                    if (managementObject.TryGetProperty("Caption", out string caption))
                    {
                        var driveRoot = new SystemInfoDto {Name = caption, Value = HeaderValueDto.Instance};
                        driveRoot.Childs.AddRange(GetLogicalDriveProperties(managementObject));

                        yield return (driveRoot, (string) managementObject.Properties["DeviceID"].Value);
                    }
            }
        }

        public IEnumerable<(SystemInfoDto, string deviceId)> QueryPartitions(string deviceId)
        {
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2",
                $"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{deviceId}'}} WHERE ASSOCCLASS=Win32_DiskDriveToDiskPartition"))
            using (var results = searcher.Get())
            {
                foreach (var managementObject in results.Cast<ManagementObject>())
                    if (managementObject.TryGetProperty("Caption", out string caption))
                    {
                        var driveRoot = new SystemInfoDto {Name = caption, Value = HeaderValueDto.Instance};
                        driveRoot.Childs.AddRange(GetPartitionProperties(managementObject));

                        yield return (driveRoot, (string) managementObject.Properties["DeviceID"].Value);
                    }
            }
        }

        private IEnumerable<SystemInfoDto> GetLogicalDriveProperties(ManagementObject managementObject)
        {
            var result = new List<SystemInfoDto>();

            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "Description");

            result.TryAdd<uint>(SystemInfoCategories.Drives, managementObject, "DriveType", u =>
            {
                var driveType = (DriveType) u;
                return new TranslatedTextValueDto($"@Drives.DriveType.{driveType}");
            });

            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "FileSystem");
            result.TryAdd<ulong>(SystemInfoCategories.Drives, managementObject, "FreeSpace", i => new DataSizeValueDto((long) i));
            result.TryAdd<ulong>(SystemInfoCategories.Drives, managementObject, "Size", i => new DataSizeValueDto((long) i));
            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "VolumeName");
            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "VolumeSerialNumber");

            if (managementObject.TryGetProperty("Size", out ulong size) && managementObject.TryGetProperty("FreeSpace", out ulong freeSpace))
                result.Add(new SystemInfoDto {Name = "Drives.Capacity", Value = new ProgressValueDto(size - freeSpace, size)});

            return result;
        }

        private IEnumerable<SystemInfoDto> GetPartitionProperties(ManagementObject managementObject)
        {
            var result = new List<SystemInfoDto>();

            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "DeviceID");
            result.TryAdd<bool>(SystemInfoCategories.Drives, managementObject, "Bootable");
            result.TryAdd<bool>(SystemInfoCategories.Drives, managementObject, "BootPartition");
            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "Description");
            result.TryAdd<uint>(SystemInfoCategories.Drives, managementObject, "HiddenSectors");
            result.TryAdd<ulong>(SystemInfoCategories.Drives, managementObject, "BlockSize");
            result.TryAdd<ulong>(SystemInfoCategories.Drives, managementObject, "NumberOfBlocks");
            result.TryAdd<bool>(SystemInfoCategories.Drives, managementObject, "PrimaryPartition");
            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "Purpose");
            result.TryAdd<ulong>(SystemInfoCategories.Drives, managementObject, "Size", size => new DataSizeValueDto((long) size));
            result.TryAdd<ulong>(SystemInfoCategories.Drives, managementObject, "StartingOffset", size => new DataSizeValueDto((long) size));
            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "Type");

            return result;
        }

        private IEnumerable<SystemInfoDto> GetDiskDriveProperties(ManagementObject managementObject)
        {
            var result = new List<SystemInfoDto>();

            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "FirmwareRevision");
            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "InterfaceType");
            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "Manufacturer");
            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "MediaType");
            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "PNPDeviceID");
            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "SerialNumber");
            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "Signature");
            result.TryAdd<string>(SystemInfoCategories.Drives, managementObject, "Status");
            result.TryAdd<ulong>(SystemInfoCategories.Drives, managementObject, "TotalCylinders");
            result.TryAdd<uint>(SystemInfoCategories.Drives, managementObject, "TotalHeads");
            result.TryAdd<ulong>(SystemInfoCategories.Drives, managementObject, "TotalSectors");
            result.TryAdd<ulong>(SystemInfoCategories.Drives, managementObject, "TotalTracks");
            result.TryAdd<uint>(SystemInfoCategories.Drives, managementObject, "TracksPerCylinder");

            result.TryAdd<ulong>(SystemInfoCategories.Drives, managementObject, "Size", size => new DataSizeValueDto((long) size));

            return result;
        }
    }
}