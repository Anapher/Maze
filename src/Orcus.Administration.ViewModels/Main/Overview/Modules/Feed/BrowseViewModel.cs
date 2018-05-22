using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Anapher.Wpf.Swan;
using NuGet.Protocol.Core.Types;
using Orcus.Administration.Modules.Models;
using Orcus.Administration.Modules.Models.Feed;
using Orcus.Administration.ViewModels.Utilities;
using Orcus.Server.Connection.Modules;

namespace Orcus.Administration.ViewModels.Main.Overview.Modules.Feed
{
    public class BrowseViewModel : PropertyChangedBase, IModulesFeed
    {
        private AsyncRelayCommand _refreshViewCommand;
        private string _searchText;
        private readonly IPackageFeed _packageFeed;
        private CancellationTokenSource _searchCancellationToken;
        private bool _includePrerelease;

        public BrowseViewModel(IEnumerable<SourceRepository> sourceRepositories)
        {
            _packageFeed = new MultiSourcePackageFeed(sourceRepositories);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(value, ref _searchText))
                    SearchAsync(SearchText).Forget();
            }
        }

        public ICollectionView View { get; }

        public bool IncludePrerelease
        {
            get => _includePrerelease;
            set
            {
                if (SetProperty(value, ref _includePrerelease))
                    SearchAsync(SearchText).Forget();
            }
        }

        public ICommand RefreshViewCommand
        {
            get
            {
                return _refreshViewCommand ?? (_refreshViewCommand =
                           new AsyncRelayCommand(async parameter => { }));
            }
        }

        private async Task SearchAsync(string searchText)
        {
            _searchCancellationToken?.Cancel();
            _searchCancellationToken?.Dispose();

            _searchCancellationToken = new CancellationTokenSource();
            var token = _searchCancellationToken.Token;

            try
            {
                var packages = await _packageFeed.SearchAsync(searchText, new SearchFilter(IncludePrerelease), token);

            }
            catch (OperationCanceledException)
            {
                return;
            }


        }
    }
}