using System.ComponentModel;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using NuGet.Protocol.Core.Types;
using Maze.Utilities;
using Prism.Commands;
using Prism.Mvvm;

namespace Maze.Administration.ViewModels.Overview.Modules
{
    public class UpdatesTabViewModel : BindableBase, IModuleTabViewModel
    {
        private ICollectionView _modules;
        private IModuleService _moduleService;

        public UpdatesTabViewModel()
        {
            RefreshCommand = new DelegateCommand(() =>
            {
                foreach (var module in _moduleService.InstalledModules)
                    module.OnUpdateVersions(null);

                LoadData();
            });
        }

        public ICollectionView Modules
        {
            get => _modules;
            set => SetProperty(ref _modules, value);
        }

        public ICommand RefreshCommand { get; }
        public string SearchText { get; set; }
        public bool IncludePrerelease { get; set; }

        public void Initialize(IModuleService service)
        {
            _moduleService = service;
            service.BrowseLoaded.Subscribe(LoadData);

            var modules = new ListCollectionView(service.InstalledModules) {Filter = ModulesUpdateFilter};
            modules.LiveFilteringProperties.Add(nameof(ModuleViewModel.IsUpdateAvailable));
            modules.IsLiveFiltering = true;

            Modules = modules;
        }

        private static bool ModulesUpdateFilter(object obj)
        {
            var moduleVm = (ModuleViewModel) obj;
            return moduleVm.IsUpdateAvailable;
        }

        private async void LoadData()
        {
            var context = new SourceCacheContext();
            await TaskCombinators.ThrottledAsync(_moduleService.InstalledModules,
                (model, token) => _moduleService.LoadVersions(model, context, token), CancellationToken.None);
        }
    }
}