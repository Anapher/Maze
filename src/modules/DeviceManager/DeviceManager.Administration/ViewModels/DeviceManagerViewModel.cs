using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anapher.Wpf.Toolkit;
using Anapher.Wpf.Toolkit.Extensions;
using Anapher.Wpf.Toolkit.StatusBar;
using Anapher.Wpf.Toolkit.Windows;
using DeviceManager.Administration.Rest;
using DeviceManager.Shared.Dtos;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.ViewModels;
using Maze.Utilities;
using Prism.Commands;
using Prism.Regions;
using Unclassified.TxLib;

namespace DeviceManager.Administration.ViewModels
{
    public class DeviceManagerViewModel : ViewModelBase
    {
        private readonly ITargetedRestClient _restClient;
        private readonly IShellStatusBar _statusBar;
        private readonly IWindowService _windowService;
        private List<DeviceCategoryViewModel> _devices;

        private DelegateCommand<DeviceViewModel> _disableDeviceCommand;
        private DelegateCommand<DeviceViewModel> _enableDeviceCommand;
        private DelegateCommand<DeviceViewModel> _openDevicePropertiesCommand;
        private AsyncDelegateCommand _refreshCommand;

        public DeviceManagerViewModel(IShellStatusBar statusBar, ITargetedRestClient restClient, IWindowService windowService)
        {
            _statusBar = statusBar;
            _restClient = restClient;
            _windowService = windowService;
        }

        public List<DeviceCategoryViewModel> Devices
        {
            get => _devices;
            set => SetProperty(ref _devices, value);
        }

        public AsyncDelegateCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new AsyncDelegateCommand(LoadDevices));

        public DelegateCommand<DeviceViewModel> OpenDevicePropertiesCommand
        {
            get
            {
                return _openDevicePropertiesCommand ?? (_openDevicePropertiesCommand = new DelegateCommand<DeviceViewModel>(parameter =>
                {
                    _windowService.ShowDialog<PropertiesViewModel>(vm => vm.Initialize(parameter));
                }));
            }
        }

        public DelegateCommand<DeviceViewModel> EnableDeviceCommand
        {
            get
            {
                return _enableDeviceCommand ?? (_enableDeviceCommand = new DelegateCommand<DeviceViewModel>(parameter =>
                {
                    DevicesResource.EnableDevice(parameter.DeviceInfo.DeviceId, _restClient).DisplayOnStatusBarCatchErrors(_statusBar,
                        Tx.T("DeviceManager:ActivateDevice", "device", parameter.Caption)).Forget();
                }));
            }
        }

        public DelegateCommand<DeviceViewModel> DisableDeviceCommand
        {
            get
            {
                return _disableDeviceCommand ?? (_disableDeviceCommand = new DelegateCommand<DeviceViewModel>(parameter =>
                {
                    DevicesResource.DisableDevice(parameter.DeviceInfo.DeviceId, _restClient).DisplayOnStatusBarCatchErrors(_statusBar,
                        Tx.T("DeviceManager:DeactivateDevice", "device", parameter.Caption)).Forget();
                }));
            }
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            await LoadDevices();
        }

        private async Task LoadDevices()
        {
            var devicesRequest = await DeviceManagerResource.QueryDevices(_restClient)
                .DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("DeviceManager:LoadDevices"));
            if (devicesRequest.Failed)
                return;

            var deviceCategories = new List<DeviceCategoryViewModel>();

            foreach (var deviceGroup in devicesRequest.Result.GroupBy(x => x.Category))
                if (deviceGroup.Key != DeviceCategory.None)
                    deviceCategories.Add(new DeviceCategoryViewModel(deviceGroup, deviceGroup.Key));
                else
                    deviceCategories.AddRange(deviceGroup.GroupBy(x => x.CustomCategory)
                        .Select(otherCategory => new DeviceCategoryViewModel(otherCategory, otherCategory.Key)));

            Devices = deviceCategories.OrderBy(x => x.Caption).ToList();
        }
    }
}