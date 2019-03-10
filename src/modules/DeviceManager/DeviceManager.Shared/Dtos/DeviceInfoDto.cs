using System;

namespace DeviceManager.Shared.Dtos
{
    public class DeviceInfoDto
    {
        public string Name { get; set; }
        public string DeviceId { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public DeviceCategory Category { get; set; }
        public string CustomCategory { get; set; }
        public uint StatusCode { get; set; }
        public string HardwareId { get; set; }

        public string DriverName { get; set; }
        public string DriverFriendlyName { get; set; }
        public DateTimeOffset DriverBuildDate { get; set; }
        public string DriverDescription { get; set; }
        public string DriverVersion { get; set; }
        public string DriverProviderName { get; set; }
        public DateTimeOffset DriverInstallDate { get; set; }
        public string DriverSigner { get; set; }
        public string DriverInfName { get; set; }
    }
}