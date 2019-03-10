using System.Collections.Generic;
using DeviceManager.Administration.Utilities;
using DeviceManager.Shared.Dtos;
using MahApps.Metro.IconPacks;
using Prism.Mvvm;

namespace DeviceManager.Administration.ViewModels
{
    public class DeviceViewModel : BindableBase, IDevicesEntryViewModel
    {
        private bool _displayWarning;

        public DeviceViewModel(DeviceInfoDto device, DeviceCategoryViewModel category)
        {
            DeviceInfo = device;
            Category = category;

            StatusErrorMessage = DeviceErrorMessageUtilities.GetErrorMessage(device.StatusCode);
            DisplayWarning = device.StatusCode != 0;
            ChildDevices = new List<DeviceViewModel>(0);
            Caption = device.Name;
        }

        public DeviceInfoDto DeviceInfo { get; }
        public DeviceCategoryViewModel Category { get; }

        public string StatusErrorMessage { get; }
        public List<DeviceViewModel> ChildDevices { get; }
        public string Caption { get; }

        public bool DisplayWarning
        {
            get => _displayWarning;
            set => SetProperty(ref _displayWarning, value);
        }

        public string WarningMessage => StatusErrorMessage;
        public PackIconMaterialKind Icon => Category.Icon;
        public bool IsCategory { get; } = false;
    }
}