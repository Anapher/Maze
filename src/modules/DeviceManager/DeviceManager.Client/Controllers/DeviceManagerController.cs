using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using DeviceManager.Client.Utilities;
using DeviceManager.Shared.Dtos;
using DeviceManager.Shared.Utilities;
using Maze.Modules.Api;
using Maze.Modules.Api.Routing;

namespace DeviceManager.Client.Controllers
{
    public class DeviceManagerController : MazeController
    {
        [MazeGet]
        public IActionResult QueryDevices()
        {
            var list = new List<DeviceInfoDto>();
            using (var searcher = new ManagementObjectSearcher(@"\\" + Environment.MachineName + @"\root\CIMV2", "Select * from Win32_PnPEntity"))
            using (var collection = searcher.Get())
            {
                foreach (var managementObject in collection.Cast<ManagementObject>())
                {
                    if (managementObject.TryGetProperty<string>("DeviceId") == @"HTREE\ROOT\0")
                        continue;

                    var device = new DeviceInfoDto
                    {
                        Name = managementObject.TryGetProperty<string>("Caption"),
                        DeviceId = managementObject.TryGetProperty<string>("DeviceId"),
                        Description = managementObject.TryGetProperty<string>("Description"),
                        Manufacturer = managementObject.TryGetProperty<string>("Manufacturer"),
                        StatusCode = managementObject.TryGetProperty<uint>("ConfigManagerErrorCode")
                    };
                    var hardwareIds = managementObject.TryGetProperty<string[]>("HardWareID");
                    if (hardwareIds?.Length > 0)
                        device.HardwareId = hardwareIds[0];

                    list.Add(device);
                    var classGuidString = managementObject.TryGetProperty<string>("ClassGuid");

                    if (!Guid.TryParse(classGuidString, out var classGuid))
                        classGuid = Guid.Empty;

                    device.Category = DeviceCategory.None;


                    foreach (var value in (DeviceCategory[]) Enum.GetValues(typeof(DeviceCategory)))
                        if (value.GetGuid() == classGuid)
                        {
                            device.Category = value;
                            break;
                        }

                    if (device.Category == DeviceCategory.None)
                        device.CustomCategory = managementObject.TryGetProperty<string>("PNPClass");
                }
            }

            using (var searcher =
                new ManagementObjectSearcher(@"\\" + Environment.MachineName + @"\root\CIMV2", "Select * from Win32_PnPSignedDriver"))
            using (var collection = searcher.Get())
            {
                foreach (var managementObject in collection.Cast<ManagementObject>())
                {
                    var deviceId = managementObject.TryGetProperty<string>("DeviceID");
                    var device = list.FirstOrDefault(x => x.DeviceId == deviceId);
                    if (device != null)
                    {
                        device.DriverFriendlyName = managementObject.TryGetProperty<string>("FriendlyName");
                        if (managementObject.TryGetDateTime("DriverDate", out var buildDate))
                            device.DriverBuildDate = buildDate;

                        device.DriverDescription = managementObject.TryGetProperty<string>("Description");
                        if (managementObject.TryGetDateTime("InstallDate", out var installDate))
                            device.DriverInstallDate = installDate;

                        device.DriverName = managementObject.TryGetProperty<string>("DriverName");
                        device.DriverProviderName = managementObject.TryGetProperty<string>("DriverProviderName");
                        device.DriverSigner = managementObject.TryGetProperty<string>("Signer");
                        device.DriverVersion = managementObject.TryGetProperty<string>("DriverVersion");
                        device.DriverInfName = managementObject.TryGetProperty<string>("InfName");
                    }
                }
            }

            return Ok(list);
        }
    }
}