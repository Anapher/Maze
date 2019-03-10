using System.Collections.Generic;
using System.Linq;
using Anapher.Wpf.Toolkit.StatusBar;
using DeviceManager.Administration.Rest;
using DeviceManager.Shared.Dtos;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.ViewModels;
using Prism.Regions;
using Unclassified.TxLib;

namespace DeviceManager.Administration.ViewModels
{
    public class DeviceManagerViewModel : ViewModelBase
    {
        private readonly ITargetedRestClient _restClient;
        private readonly IShellStatusBar _statusBar;
        private List<DeviceCategoryViewModel> _devices;

        public DeviceManagerViewModel(IShellStatusBar statusBar, ITargetedRestClient restClient)
        {
            _statusBar = statusBar;
            _restClient = restClient;
        }

        public List<DeviceCategoryViewModel> Devices
        {
            get => _devices;
            set => SetProperty(ref _devices, value);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            var devicesRequest = await DeviceManagerResource.QueryDevices(_restClient)
                .DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("DeviceManager:LoadDevices"));
            if (devicesRequest.Failed)
            {
                // Wtf
            }

            var deviceCategories = new List<DeviceCategoryViewModel>();

            foreach (var deviceGroup in devicesRequest.Result.GroupBy(x => x.Category))
            {
                if (deviceGroup.Key != DeviceCategory.None)
                    deviceCategories.Add(new DeviceCategoryViewModel(deviceGroup, deviceGroup.Key));
                else
                {
                    deviceCategories.AddRange(
                        deviceGroup.GroupBy(x => x.CustomCategory)
                            .Select(otherCategory => new DeviceCategoryViewModel(otherCategory, otherCategory.Key)));
                }
            }

            Devices = deviceCategories.OrderBy(x => x.Caption).ToList();
        }
    }
}