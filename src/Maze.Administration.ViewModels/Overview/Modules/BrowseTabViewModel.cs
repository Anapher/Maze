using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using Maze.Utilities;
using Prism.Commands;
using Prism.Mvvm;

namespace Maze.Administration.ViewModels.Overview.Modules
{
    public class BrowseTabViewModel : BindableBase, IModuleTabViewModel
    {
        private AggregatedPackageSearchResource _searchResource;
        private AggregateListResource _listResource;
        private IModuleService _service;
        private CancellationTokenSource _initializationCancellation;
        private ICollectionView _modules;
        private string _searchText;
        private bool _includePrerelease;

        public BrowseTabViewModel()
        {
            RefreshCommand = new DelegateCommand(() => InitializePackages(SearchText).Forget());
        }

        public ICollectionView Modules
        {
            get => _modules;
            set => SetProperty(ref _modules, value);
        }

        public ICommand RefreshCommand { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    InitializePackages(value).Forget();
                }
            }
        }

        public bool IncludePrerelease
        {
            get => _includePrerelease;
            set
            {
                if (_includePrerelease != value)
                {
                    _includePrerelease = value;
                    InitializePackages(SearchText).Forget();
                }
            }
        }

        private async Task InitializePackages(string searchText)
        {
            _initializationCancellation?.Cancel();
            _initializationCancellation = new CancellationTokenSource();

            var token = _initializationCancellation.Token;

            try
            {
                var packages = await _searchResource.SearchAsync(searchText, new SearchFilter(IncludePrerelease), 0, 30, token);
                if (token.IsCancellationRequested)
                    return;

                Modules = new ListCollectionView(new ObservableCollection<ModuleViewModel>(packages.Select(CreateModuleViewModel)));
            }
            catch (Exception)
            {
                return; //TODO better handling
            }
        }

        private ModuleViewModel CreateModuleViewModel(IPackageSearchMetadata metadata)
        {
            var existing = _service.InstalledModules.FirstOrDefault(x => x.PackageIdentity.Id.Equals(metadata.Identity.Id, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Initialize(metadata);
                return existing;
            }

            var viewModel = new ModuleViewModel(metadata.Identity, ModuleStatus.None);
            viewModel.Initialize(metadata);
            return viewModel;
        }

        public async void Initialize(IModuleService service)
        {
            _service = service;
            _service.InstalledModules.CollectionChanged += InstalledModulesOnCollectionChanged;

            _searchResource = new AggregatedPackageSearchResource(service.Repositories);
            _listResource = new AggregateListResource(service.Repositories);

            await InitializePackages(null);
            service.BrowseLoaded.Publish();
        }

        private void InstalledModulesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var source = (ObservableCollection<ModuleViewModel>) Modules?.SourceCollection;
            if (source == null)
                return;
            
            var newItems = e.NewItems?.Cast<ModuleViewModel>().ToList();
            if (newItems?.Count > 0)
            {
                for (var i = 0; i < source.Count; i++)
                {
                    var moduleViewModel = source[i];
                    var newModule = newItems.FirstOrDefault(x =>
                        string.Equals(x.PackageIdentity.Id, moduleViewModel.PackageIdentity.Id, StringComparison.OrdinalIgnoreCase));
                    if (newModule != null)
                    {
                        source[i] = newModule;
                        newItems.Remove(newModule);

                        if (!newItems.Any())
                            break;
                    }
                }
            }
        }
    }

    public abstract class AggregatedResource
    {
        protected static IEnumerable<T> Combine<T>(IEnumerable<IEnumerable<T>> enumerables)
        {
            var enumerators = enumerables.Reverse().Select(x => x.GetEnumerator()).ToList();
            while (enumerators.Any())
                for (var i = enumerators.Count - 1; i >= 0; i--)
                {
                    var enumerator = enumerators[i];
                    if (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                    else
                    {
                        enumerators.Remove(enumerator);
                        enumerator.Dispose();
                    }
                }
        }
    }

    public class AggregatedPackageSearchResource : AggregatedResource
    {
        private readonly IReadOnlyList<SourceRepository> _repositories;

        public AggregatedPackageSearchResource(IReadOnlyList<SourceRepository> repositories)
        {
            _repositories = repositories;
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> SearchAsync(string searchTerm, SearchFilter searchFilter, int skip, int take,
            CancellationToken cancellationToken)
        {
            var results = await TaskCombinators.ThrottledAsync(_repositories,
                async (repository, token) =>
                    await (await repository.GetResourceAsync<PackageSearchResource>()).SearchAsync(searchTerm, searchFilter, skip, take,
                        NullLogger.Instance, token), cancellationToken);

            return Combine(results);
        }
    }

    public class AggregateListResource : AggregatedResource
    {
        private readonly IReadOnlyList<SourceRepository> _repositories;
        private List<IEnumeratorAsync<IPackageSearchMetadata>> _listEnumerators;
        private readonly List<IPackageSearchMetadata> _cachedPackages;

        public AggregateListResource(IReadOnlyList<SourceRepository> repositories)
        {
            _repositories = repositories;
            _cachedPackages = new List<IPackageSearchMetadata>();
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> ListAsync(int skip, int take, CancellationToken cancellationToken)
        {
            if (_listEnumerators == null)
            {
                var enumerators = await TaskCombinators.ThrottledIgnoreErrorsAsync(_repositories, async (repository, token) =>
                {
                    var resource = await repository.GetResourceAsync<ListResource>();

                    var result = await resource.ListAsync(string.Empty, false, true, false, NullLogger.Instance, cancellationToken);
                    return result.GetEnumeratorAsync();
                }, cancellationToken);

                _listEnumerators = enumerators.ToList();
            }

            IEnumerable<IPackageSearchMetadata> cachedPackages = Enumerable.Empty<IPackageSearchMetadata>();
            if (skip < _cachedPackages.Count)
            {
                cachedPackages = _cachedPackages.Skip(skip).Take(take);

                var diff = _cachedPackages.Count - (skip + take);
                if (diff >= 0)
                    return cachedPackages;

                take += diff;
            }

            var packages = await TakeCombineAsync(_listEnumerators, take);
            _cachedPackages.AddRange(packages);

            return cachedPackages.Concat(packages.Take(take));
        }

        protected static async Task<IReadOnlyList<T>> TakeCombineAsync<T>(IEnumerable<IEnumeratorAsync<T>> enumerables, int take)
        {
            var result = new List<T>();

            var enumerators = enumerables.Reverse().ToList();
            while (result.Count < take && enumerators.Any())
                for (var i = enumerators.Count - 1; i >= 0; i--)
                {
                    var enumerator = enumerators[i];
                    if (await enumerator.MoveNextAsync())
                    {
                        result.Add(enumerator.Current);
                    }
                    else
                    {
                        enumerators.Remove(enumerator);
                    }
                }

            return result;
        }
    }
}