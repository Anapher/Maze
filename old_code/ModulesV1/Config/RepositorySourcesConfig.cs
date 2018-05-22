using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Orcus.Server.Connection;
using Orcus.Server.Service.Modules.Config;

namespace Orcus.Server.Service.ModulesV1.Config
{
    public interface IRepositorySourceConfig
    {
        IImmutableList<SourceRepository> SourceRepositories { get; }
        SourceRepository OfficalRepository { get; set; }
        IImmutableList<Uri> Items { get; }
        Task AddItem(Uri item);
        Task RemoveItem(Uri item);
        Task Reload();
    }

    public class RepositorySourcesConfig : ChangeableJsonFile<Uri>, IRepositorySourceConfig
    {
        private readonly List<Lazy<INuGetResourceProvider>> _providers;

        public RepositorySourcesConfig(string path) : base(path)
        {
            SourceRepositories = new ImmutableArray<SourceRepository>();
            _providers = new List<Lazy<INuGetResourceProvider>>();
            _providers.AddRange(Repository.Provider.GetCoreV3());

            OfficalRepository = new SourceRepository(new PackageSource(OfficalOrcusRepository.Uri.OriginalString), _providers);
        }

        public IImmutableList<SourceRepository> SourceRepositories { get; private set; }
        public SourceRepository OfficalRepository { get; set; }

        public override Task AddItem(Uri item)
        {
            if (Items.Contains(item))
                throw new ArgumentException("The source repository already exists.", nameof(item));

            var packageSource = new PackageSource(item.OriginalString);
            SourceRepositories = SourceRepositories.Add(new SourceRepository(packageSource, _providers));

            return base.AddItem(item);
        }

        public override Task RemoveItem(Uri item)
        {
            var repository = SourceRepositories.First(x => x.PackageSource.SourceUri == item);
            SourceRepositories = SourceRepositories.Remove(repository);

            return base.RemoveItem(item);
        }

        public override async Task Reload()
        {
            await base.Reload();
            SourceRepositories =
                Items.Select(x => new SourceRepository(new PackageSource(x.OriginalString), _providers))
                    .ToImmutableList();
        }
    }
}