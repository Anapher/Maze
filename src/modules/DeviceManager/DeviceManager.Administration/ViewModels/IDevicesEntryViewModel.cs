using System.Collections.Generic;
using MahApps.Metro.IconPacks;

namespace DeviceManager.Administration.ViewModels
{
    public interface IDevicesEntryViewModel
    {
        string Caption { get; }
        bool DisplayWarning { get; }
        string WarningMessage { get; }
        PackIconMaterialKind Icon { get; }
        bool IsCategory { get; }
        List<DeviceViewModel> ChildDevices { get; }
    }
}