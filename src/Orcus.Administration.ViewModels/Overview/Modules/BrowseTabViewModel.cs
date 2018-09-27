using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using Orcus.Utilities;
using Prism.Mvvm;

namespace Orcus.Administration.ViewModels.Overview.Modules
{
    public class BrowseTabViewModel : BindableBase, IModuleTabViewModel
    {
        private AggregatedPackageSearchResource _searchResource;
        private AggregateListResource _listResource;
        private IModuleService _service;
        private CancellationTokenSource _initializationCancellation;

        private ICollectionView _modules;

        public ICollectionView Modules
        {
            get => _modules;
            set => SetProperty(ref _modules, value);
        }
        public ICommand RefreshCommand { get; }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    InitializePackages(value);
                }
            }
        }

        public bool IncludePrerelease { get; set; }

        private async Task InitializePackages(string searchText)
        {
            _initializationCancellation?.Cancel();
            _initializationCancellation = new CancellationTokenSource();

            var token = _initializationCancellation.Token;

            try
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    var packages = await _listResource.ListAsync(0, 30, token);
                    if (token.IsCancellationRequested)
                        return;

                    Modules = new ListCollectionView(packages.Select(CreateModuleViewModel).ToList());
                }
                else
                {
                    var packages = await _searchResource.SearchAsync(searchText, new SearchFilter(IncludePrerelease), 0, 30, token);
                    if (token.IsCancellationRequested)
                        return;

                    Modules = new ListCollectionView(packages.Select(CreateModuleViewModel).ToList());
                }
            }
            catch (Exception e)
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

            var searchResources = await TaskCombinators.ThrottledAsync(service.Repositories,
                (repository, token) => repository.GetResourceAsync<PackageSearchResource>(token), CancellationToken.None);
            var listResources = await TaskCombinators.ThrottledAsync(service.Repositories,
                (repository, token) => repository.GetResourceAsync<ListResource>(token), CancellationToken.None);

            _searchResource = new AggregatedPackageSearchResource(searchResources.ToList());
            _listResource = new AggregateListResource(listResources.ToList());

            await InitializePackages(null);
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
        private readonly IReadOnlyList<PackageSearchResource> _searchResources;

        public AggregatedPackageSearchResource(IReadOnlyList<PackageSearchResource> resources)
        {
            _searchResources = resources;
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> SearchAsync(string searchTerm, SearchFilter searchFilter, int skip, int take,
            CancellationToken cancellationToken)
        {
            var results = await TaskCombinators.ThrottledAsync(_searchResources,
                (resource, token) => resource.SearchAsync(searchTerm, searchFilter, skip, take, NullLogger.Instance, token), cancellationToken);

            return Combine(results);
        }
    }

    public class AggregateListResource : AggregatedResource
    {
        private readonly IReadOnlyList<ListResource> _listResources;
        private List<IEnumeratorAsync<IPackageSearchMetadata>> _listEnumerators;
        private readonly List<IPackageSearchMetadata> _cachedPackages;

        public AggregateListResource(IReadOnlyList<ListResource> resources)
        {
            _listResources = resources;
            _cachedPackages = new List<IPackageSearchMetadata>();
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> ListAsync(int skip, int take, CancellationToken cancellationToken)
        {
            if (_listEnumerators == null)
            {
                var enumerators = await TaskCombinators.ThrottledAsync(_listResources, async (resource, token) =>
                {
                    try
                    {
                        var result = await resource.ListAsync(string.Empty, false, true, false, NullLogger.Instance, cancellationToken);
                        return result.GetEnumeratorAsync();
                    }
                    catch (Exception e)
                    {
                        // ignored
                        return null;
                    }
                }, cancellationToken);

                _listEnumerators = enumerators.Where(x => x != null).ToList();
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
            while (result.Count < take)
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