using System.Collections.Generic;
using System.Linq;
using DeviceManager.Shared.Dtos;
using MahApps.Metro.IconPacks;
using Unclassified.TxLib;

namespace DeviceManager.Administration.ViewModels
{
    public class DeviceCategoryViewModel : IDevicesEntryViewModel
    {
        private DeviceCategoryViewModel(IEnumerable<DeviceInfoDto> childDevices)
        {
            ChildDevices = childDevices.Select(x => new DeviceViewModel(x, this)).OrderBy(x => x.Caption).ToList();
        }

        public DeviceCategoryViewModel(IEnumerable<DeviceInfoDto> childDevices, DeviceCategory deviceCategory) : this(childDevices)
        {
            DeviceCategory = deviceCategory;
            Caption = DeviceCategoryToString(deviceCategory);
            Icon = GetDeviceCategoryIcon(deviceCategory);
        }

        public DeviceCategoryViewModel(IEnumerable<DeviceInfoDto> childDevices, string deviceCategory) : this(childDevices)
        {
            DeviceCategory = DeviceCategory.None;
            Caption = deviceCategory;
            Icon = PackIconMaterialKind.CubeOutline;
        }

        public List<DeviceViewModel> ChildDevices { get; }
        public DeviceCategory DeviceCategory { get; }

        public string Caption { get; }
        public bool DisplayWarning { get; }
        public string WarningMessage { get; }
        public PackIconMaterialKind Icon { get; }
        public bool IsCategory { get; } = true;

        private static PackIconMaterialKind GetDeviceCategoryIcon(DeviceCategory deviceCategory)
        {
            switch (deviceCategory)
            {
                case DeviceCategory.AudioEndpoint:
                case DeviceCategory.Multimedia:
                    return PackIconMaterialKind.VolumeLow;
                case DeviceCategory.Computer:
                    return PackIconMaterialKind.Television;
                case DeviceCategory.PrintQueue:
                case DeviceCategory.Printers:
                case DeviceCategory.Printers_BusSpecificClassDrivers:
                    return PackIconMaterialKind.Printer;
                case DeviceCategory.CDROMDrives:
                    return PackIconMaterialKind.Disk;
                case DeviceCategory.HumanInterfaceDevicesHID:
                    return PackIconMaterialKind.KeyboardVariant;
                case DeviceCategory.DiskDrives:
                case DeviceCategory.StorageVolumes:
                case DeviceCategory.StorageVolumeSnapshots:
                    return PackIconMaterialKind.Harddisk;
                case DeviceCategory.Keyboard:
                    return PackIconMaterialKind.Keyboard;
                case DeviceCategory.DisplayAdapters:
                    return PackIconMaterialKind.Monitor;
                case DeviceCategory.Monitor:
                    return PackIconMaterialKind.MonitorMultiple;
                case DeviceCategory.USBDevice:
                case DeviceCategory.USBBusDevices_hubsandhostcontrollers:
                    return PackIconMaterialKind.Usb;
                case DeviceCategory.Mouse:
                    return PackIconMaterialKind.Mouse;
                case DeviceCategory.WSDPrintDevice:
                    return PackIconMaterialKind.Printer;
                case DeviceCategory.SCSIandRAIDControllers:
                    return PackIconMaterialKind.Raspberrypi;
                case DeviceCategory.NetworkService:
                case DeviceCategory.NetworkAdapter:
                case DeviceCategory.NetworkClient:
                case DeviceCategory.NetworkTransport:
                    return PackIconMaterialKind.LanConnect;
                case DeviceCategory.Processors:
                    return PackIconMaterialKind.Raspberrypi;
                case DeviceCategory.Sensors:
                    return PackIconMaterialKind.AccessPoint;
                case DeviceCategory.BatteryDevices:
                    return PackIconMaterialKind.Battery50;
                default:
                    return PackIconMaterialKind.CubeOutline;
            }
        }

        private static string DeviceCategoryToString(DeviceCategory deviceCategory)
        {
            if (Tx.TryGetText($"DeviceManager:Categories.{deviceCategory}", out var text))
                return text;

            return deviceCategory.ToString();
        }
    }
}