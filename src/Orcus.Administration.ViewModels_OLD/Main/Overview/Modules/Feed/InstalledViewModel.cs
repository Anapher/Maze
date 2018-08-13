using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Anapher.Wpf.Swan;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orcus.Administration.Core.Clients;
using Orcus.Administration.Core.Rest.Modules.V1;
using Orcus.Administration.ViewModels.Utilities;
using Orcus.Server.Connection.Modules;

namespace Orcus.Administration.ViewModels.Main.Overview.Modules.Feed
{
    public interface IServerModuleInfo
    {
        ObservableCollection<PackageIdentity> PrimaryModules { get; }
        HashSet<string> ServerInstalledModules { get; }
        HashSet<string> AdministrationInstalledModules { get; }
    }

    public class ServerModuleInfo : IServerModuleInfo
    {
        public ObservableCollection<PackageIdentity> PrimaryModules { get; }
        public HashSet<string> ServerInstalledModules { get; set; }
        public HashSet<string> AdministrationInstalledModules { get; set; }
    }

    public class InstalledViewModel : PropertyChangedBase, IModulesFeed
    {
        private readonly IServerModuleInfo _serverInfo;
        private readonly IOrcusRestClient _client;
        private string _searchText;
        private List<ModuleItemViewModel> _modules;
        private ICollectionView _view;
        private ICommand _refreshViewCommand;

        public InstalledViewModel(IServerModuleInfo serverInfo, IOrcusRestClient client)
        {
            _serverInfo = serverInfo;
            _client = client;
            serverInfo.PrimaryModules.CollectionChanged += PrimaryModulesOnCollectionChanged;
            UpdateInstalledModules().Forget();
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(value, ref _searchText))
                    View?.Refresh();
            }
        }

        public ICollectionView View
        {
            get => _view;
            set => SetProperty(value, ref _view);
        }

        public ICommand RefreshViewCommand
        {
            get
            {
                return _refreshViewCommand ?? (_refreshViewCommand = new AsyncRelayCommand(async parameter =>
                {
                    await UpdateInstalledModules();
                }));
            }
        }

        public bool IncludePrerelease { get; set; }

        private async Task UpdateInstalledModules()
        {
            var installedModules = await ModulesResource.FetchInstalledModules(_client);
            _modules = installedModules.Select(x => x.ToItem(module =>
                    _serverInfo.PrimaryModules.Any(y => string.Equals(x.Id, y.Id, StringComparison.OrdinalIgnoreCase))))
                .ToList();

            var listView = new ListCollectionView(_modules) {Filter = Filter};
            listView.SortDescriptions.Add(new SortDescription(nameof(ModuleItemViewModel.Id), ListSortDirection.Ascending));
            View = listView;
        }

        private bool Filter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            var viewModel = (ModuleItemViewModel) obj;
            return viewModel.Id.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) > -1;
        }

        private void PrimaryModulesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateInstalledModules().Forget();
        }

        public void Dispose()
        {
            _serverInfo.PrimaryModules.CollectionChanged -= PrimaryModulesOnCollectionChanged;
        }
    }

    public static class InstalledModuleExtensions
    {
        public static ModuleItemViewModel ToItem(this InstalledModule installedModule, Func<InstalledModule, bool> autoReferencedFunc)
        {
            return new ModuleItemViewModel
            {
                Id = installedModule.Id,
                Version = installedModule.Version,
                AutoReferenced = autoReferencedFunc(installedModule),
                DownloadCount = null,
                Author = installedModule.Authors,
                Status = ModuleStatus.Installed,
                IconUri = string.IsNullOrEmpty(installedModule.IconUrl) ? null : new Uri(installedModule.IconUrl)
            };
        }
    }
}