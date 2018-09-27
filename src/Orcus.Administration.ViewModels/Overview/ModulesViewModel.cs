using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MahApps.Metro.IconPacks;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Rest.Modules.V1;
using Orcus.Administration.Library.ViewModels;
using Orcus.Administration.ViewModels.Overview.Modules;
using Unclassified.TxLib;

namespace Orcus.Administration.ViewModels.Overview
{
    public class ModulesViewModel : OverviewTabBase, IModuleService
    {
        private readonly IOrcusRestClient _restClient;
        private readonly List<IModuleTabViewModel> _tabViewModels;
        private bool _includePrerelease;
        private int _index;
        private string _searchText;
        private IModuleTabViewModel _tabViewModel;

        public ModulesViewModel(IOrcusRestClient restClient) : base(Tx.T("Modules"), PackIconFontAwesomeKind.PuzzlePieceSolid)
        {
            _restClient = restClient;

            _tabViewModels = new List<IModuleTabViewModel> {new BrowseTabViewModel(), new InstalledTabViewModel(), new UpdatesTabViewModel()};
        }

        public IModuleTabViewModel TabViewModel
        {
            get => _tabViewModel;
            set => SetProperty(ref _tabViewModel, value);
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
                if (SetProperty(ref _includePrerelease, value)) TabViewModel.IncludePrerelease = value;
            }
        }

        public IReadOnlyList<SourceRepository> Repositories { get; private set; }

        public ObservableCollection<ModuleViewModel> InstalledModules { get; private set; }

        public override async void OnInitialize()
        {
            base.OnInitialize();

            var modules = await ModulesResource.GetInstalledModules(_restClient);
            var sources = ModulesResource.FetchRepositorySources(_restClient);

            InstalledModules = new ObservableCollection<ModuleViewModel>(modules.Installed.Select(x => new ModuleViewModel(x, ModuleStatus.Installed))
                .Concat(modules.ToBeInstalled.Select(x => new ModuleViewModel(x, ModuleStatus.ToBeInstalled))));

            var providers = Repository.Provider.GetCoreV3();
            Repositories = (await sources).Select(x => new SourceRepository(new PackageSource(x.AbsoluteUri), providers)).ToList();

            foreach (var moduleTabViewModel in _tabViewModels) moduleTabViewModel.Initialize(this);

            var metadata = await ModulesResource.FetchPackageInfo(modules.Installed, _restClient);
            foreach (var packageMetadata in metadata.Where(x => x != null))
            {
                var viewModel = InstalledModules.FirstOrDefault(x => x.PackageIdentity.Equals(packageMetadata.Identity));
                viewModel?.Initialize(packageMetadata);
            }
        }
    }

    public interface IModuleService
    {
        IReadOnlyList<SourceRepository> Repositories { get; }
        ObservableCollection<ModuleViewModel> InstalledModules { get; }
    }
}