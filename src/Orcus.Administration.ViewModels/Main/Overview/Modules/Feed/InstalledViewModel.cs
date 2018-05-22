using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Anapher.Wpf.Swan;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

namespace Orcus.Administration.ViewModels.Main.Overview.Modules.Feed
{
    public class LocalModuleRepository
    {

    }

    public class InstalledViewModel : PropertyChangedBase, IModulesFeed
    {
        private string _searchText;

        public InstalledViewModel(List<PackageIdentity> packages)
        {
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(value, ref _searchText))
                    View.Refresh();
            }
        }

        public ICollectionView View { get; }
        public ICommand RefreshViewCommand { get; }
        public bool IncludePrerelease { get; set; }
    }
}