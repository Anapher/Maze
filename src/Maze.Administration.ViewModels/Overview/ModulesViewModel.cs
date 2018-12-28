using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Anapher.Wpf.Swan.Extensions;
using MahApps.Metro.IconPacks;
using Microsoft.AspNetCore.SignalR.Client;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Rest.Modules.V1;
using Maze.Administration.Library.ViewModels;
using Maze.Administration.Library.Views;
using Maze.Administration.ViewModels.Overview.Modules;
using Maze.Server.Connection;
using Maze.Server.Connection.Utilities;
using Maze.Utilities;
using Prism.Commands;
using Prism.Events;
using Unclassified.TxLib;

namespace Maze.Administration.ViewModels.Overview
{
    public class ModulesViewModel : OverviewTabBase, IModuleService
    {
        private readonly IMazeRestClient _restClient;
        private readonly List<IModuleTabViewModel> _tabViewModels;
        private readonly IWindowService _windowService;

        private DelegateCommand _displayRepositorySourcesInfoCommand;
        private List<FindPackageByIdResource> _findPackageByIdResources;
        private bool _includePrerelease;
        private int _index;
        private DelegateCommand<ModuleViewModel> _installModuleCommand;
        private DelegateCommand<NuGetVersion> _installVersionCommand;
        private List<PackageMetadataResource> _packageMetadataResources;
        private string _repositoryUris;
        private string _searchText;
        private ModuleViewModel _selectedModule;
        private NuGetVersion _selectedVersion;
        private IModuleTabViewModel _tabViewModel;
        private DelegateCommand<ModuleViewModel> _uninstallModuleCommand;
        private DelegateCommand<ModuleViewModel> _updateModuleCommand;

        public ModulesViewModel(IMazeRestClient restClient, IWindowService windowService) : base(Tx.T("Modules"), PackIconFontAwesomeKind.PuzzlePieceSolid)
        {
            _restClient = restClient;
            _windowService = windowService;

            _tabViewModels = new List<IModuleTabViewModel> {new BrowseTabViewModel(), new InstalledTabViewModel(), new UpdatesTabViewModel()};
            BrowseLoaded = new PubSubEvent();
        }

        public IModuleTabViewModel TabViewModel
        {
            get => _tabViewModel;
            set => SetProperty(ref _tabViewModel, value);
        }

        public ModuleViewModel SelectedModule
        {
            get => _selectedModule;
            set
            {
                if (SetProperty(ref _selectedModule, value) && value != null)
                    LoadVersions(value, new SourceCacheContext(), CancellationToken.None).ContinueWith(x =>
                    {
                        if (_selectedModule == value)
                            SelectedVersion = value.Version;
                    });
            }
        }

        public NuGetVersion SelectedVersion
        {
            get => _selectedVersion;
            set => SetProperty(ref _selectedVersion, value);
        }

        public int Index
        {
            get => _index;
            set
            {
                if (SetProperty(ref _index, value))
                {
                    TabViewModel = _tabViewModels[value];
                    TabViewModel.SearchText = SearchText;
                    TabViewModel.IncludePrerelease = IncludePrerelease;
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value)) TabViewModel.SearchText = value;
            }
        }

        public bool IncludePrerelease
        {
            get => _includePrerelease;
            set
            {
                if (SetProperty(ref _includePrerelease, value))
                {
                    TabViewModel.IncludePrerelease = value;
                    foreach (var installedModule in InstalledModules)
                        installedModule.IncludePrerelease = value;
                }
            }
        }

        public string RepositoryUris
        {
            get => _repositoryUris;
            set => SetProperty(ref _repositoryUris, value);
        }

        public DelegateCommand<NuGetVersion> InstallVersionCommand
        {
            get
            {
                return _installVersionCommand ?? (_installVersionCommand =
                           new DelegateCommand<NuGetVersion>(
                               parameter => { InstallModule(new PackageIdentity(SelectedModule.PackageIdentity.Id, parameter)).Forget(); },
                               version => version != null && version != SelectedModule.Version)).ObservesProperty(() => SelectedVersion);
            }
        }

        public DelegateCommand<ModuleViewModel> InstallModuleCommand
        {
            get
            {
                return _installModuleCommand ?? (_installModuleCommand = new DelegateCommand<ModuleViewModel>(parameter =>
                {
                    InstallModule(parameter.PackageIdentity).Forget();
                }));
            }
        }

        public DelegateCommand<ModuleViewModel> UpdateModuleCommand
        {
            get
            {
                return _updateModuleCommand ?? (_updateModuleCommand = new DelegateCommand<ModuleViewModel>(parameter =>
                {
                    InstallModule(new PackageIdentity(parameter.PackageIdentity.Id, parameter.NewestVersion)).Forget();
                }));
            }
        }

        public DelegateCommand<ModuleViewModel> UninstallModuleCommand
        {
            get
            {
                return _uninstallModuleCommand ?? (_uninstallModuleCommand = new DelegateCommand<ModuleViewModel>(parameter =>
                {
                    UninstallModule(parameter.PackageIdentity).Forget();
                }));
            }
        }

        public DelegateCommand DisplayRepositorySourcesInfoCommand
        {
            get
            {
                return _displayRepositorySourcesInfoCommand ?? (_displayRepositorySourcesInfoCommand = new DelegateCommand(() =>
                {
                    _windowService.ShowMessage(Tx.T("ModulesView:SourcesInfo"), Tx.T("Information"), MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }));
            }
        }

        public PubSubEvent BrowseLoaded { get; }
        public IReadOnlyList<SourceRepository> Repositories { get; private set; }
        public ObservableCollection<ModuleViewModel> InstalledModules { get; private set; }

        public async Task LoadVersions(ModuleViewModel moduleViewModel, SourceCacheContext context, CancellationToken cancellationToken)
        {
            if (!await moduleViewModel.LoadVersionsAsync())
                foreach (var findPackageByIdResource in _findPackageByIdResources)
                    try
                    {
                        var versions = (await findPackageByIdResource.GetAllVersionsAsync(moduleViewModel.PackageIdentity.Id, context,
                            NullLogger.Instance, cancellationToken))?.ToList();
                        if (versions?.Any() == true)
                        {
                            moduleViewModel.OnUpdateVersions(versions);
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
        }

        public override async void OnInitialize()
        {
            base.OnInitialize();

            var modules = await ModulesResource.GetInstalledModules(_restClient);
            _restClient.HubConnection.On<string>(HubEventNames.ModuleInstalled, OnModuleInstalled);
            _restClient.HubConnection.On<string>(HubEventNames.ModuleUninstalled, OnModuleUninstalled);

            var sources = ModulesResource.FetchRepositorySources(_restClient);

            InstalledModules = new ObservableCollection<ModuleViewModel>(modules.Installed.Select(x => new ModuleViewModel(x, ModuleStatus.Installed))
                .Concat(modules.ToBeInstalled.Select(x => new ModuleViewModel(x, ModuleStatus.ToBeInstalled))));

            var providers = Repository.Provider.GetCoreV3();
            Repositories = (await sources).Select(x => new SourceRepository(new PackageSource(x.AbsoluteUri), providers)).ToList();
            RepositoryUris = string.Join(", ", sources.Result.Select(x => x.Host));

            _packageMetadataResources = (await TaskCombinators.ThrottledAsync(Repositories,
                (repository, token) => repository.GetResourceAsync<PackageMetadataResource>(token), CancellationToken.None)).ToList();

            _findPackageByIdResources = (await TaskCombinators.ThrottledAsync(Repositories,
                (repository, token) => repository.GetResourceAsync<FindPackageByIdResource>(token), CancellationToken.None)).ToList();

            foreach (var moduleTabViewModel in _tabViewModels) moduleTabViewModel.Initialize(this);

            var metadata = await ModulesResource.FetchPackageInfo(modules.Installed, _restClient);
            foreach (var packageMetadata in metadata.Where(x => x != null))
            {
                var viewModel = InstalledModules.FirstOrDefault(x => x.PackageIdentity.Equals(packageMetadata.Identity));
                viewModel?.Initialize(packageMetadata);
            }

            await TaskCombinators.ThrottledAsync(InstalledModules.Where(x => !x.IsLoaded), (model, token) => LoadModuleMetadata(model),
                CancellationToken.None);

            TabViewModel = _tabViewModels.First();
        }

        private async Task InstallModule(PackageIdentity packageIdentity)
        {
            try
            {
                await ModulesResource.InstallModule(packageIdentity, _restClient);
            }
            catch (Exception e)
            {
                e.ShowMessage(_windowService);
            }
        }

        private async Task UninstallModule(PackageIdentity packageIdentity)
        {
            try
            {
                await ModulesResource.UninstallModule(packageIdentity, _restClient);
            }
            catch (Exception e)
            {
                e.ShowMessage(_windowService);
            }
        }

        private void OnModuleInstalled(string identity)
        {
            var packageIdentity = PackageIdentityConvert.ToPackageIdentity(identity);
            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
                var viewModel = InstalledModules.FirstOrDefault(x => x.PackageIdentity.Id == packageIdentity.Id);
                if (viewModel != null)
                {
                    viewModel.OnUpdateVersion(packageIdentity.Version);
                }
                else
                {
                    viewModel = new ModuleViewModel(packageIdentity, ModuleStatus.ToBeInstalled);
                    LoadModuleMetadata(viewModel).Forget();
                    InstalledModules.Add(viewModel);
                }
            }));
        }

        private void OnModuleUninstalled(string identity)
        {
            var packageIdentity = PackageIdentityConvert.ToPackageIdentity(identity);

            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
                var viewModel = InstalledModules.FirstOrDefault(x => x.PackageIdentity.Id == packageIdentity.Id);
                if (viewModel != null)
                {
                    viewModel.OnUninstall();
                    InstalledModules.Remove(viewModel);
                }
            }));
        }

        private async Task LoadModuleMetadata(ModuleViewModel moduleViewModel)
        {
            foreach (var packageMetadataResource in _packageMetadataResources)
                try
                {
                    var metadata = await packageMetadataResource.GetMetadataAsync(moduleViewModel.PackageIdentity, new SourceCacheContext(),
                        NullLogger.Instance, CancellationToken.None);
                    if (metadata != null)
                    {
                        moduleViewModel.Initialize(metadata);
                        break;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
        }
    }

    public interface IModuleService
    {
        PubSubEvent BrowseLoaded { get; }

        IReadOnlyList<SourceRepository> Repositories { get; }
        ObservableCollection<ModuleViewModel> InstalledModules { get; }

        Task LoadVersions(ModuleViewModel moduleViewModel, SourceCacheContext context, CancellationToken cancellationToken);
    }
}