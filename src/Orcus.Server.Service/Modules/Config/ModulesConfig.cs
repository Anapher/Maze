using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using Orcus.Server.Connection.JsonConverters;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Modules.Config.Base;
using Orcus.Server.Service.Modules.Extensions;

namespace Orcus.Server.Service.Modules.Config
{
    /// <summary>
    ///     Provides the primary modules only (user installed)
    /// </summary>
    public interface IModulesConfig
    {
        /// <summary>
        ///     The primary modules
        /// </summary>
        IImmutableList<SourcedPackageIdentity> Modules { get; }

        /// <summary>
        ///     The local path to the module file
        /// </summary>
        string Path { get; }

        /// <summary>
        ///     Reload the config file from disk
        /// </summary>
        Task Reload();

        /// <summary>
        ///     Add a new module
        /// </summary>
        /// <param name="id">The module identity</param>
        Task Add(SourcedPackageIdentity id);

        /// <summary>
        ///     Remove a module
        /// </summary>
        /// <param name="id">The module identity</param>
        Task Remove(PackageIdentity id);

        /// <summary>
        ///     Replace the whole module list
        /// </summary>
        /// <param name="modules">The new list</param>
        Task Replace(IEnumerable<SourcedPackageIdentity> modules);
    }

    public class ModulesConfig : JsonObjectFile<IList<ModuleRepositoryGroup>>, IModulesConfig
    {
        public ModulesConfig(string path) : base(path)
        {
            Modules = ImmutableList<SourcedPackageIdentity>.Empty;

            JsonSettings.Converters.Add(new PackageIdentityConverter());
            JsonSettings.Converters.Add(new NuGetVersionConverter());
        }

        public IImmutableList<SourcedPackageIdentity> Modules { get; private set; }

        public virtual async Task Reload()
        {
            var data = await Load();
            Modules = data == null
                ? ImmutableList<SourcedPackageIdentity>.Empty
                : data.SelectMany(x => x.Modules.Select(y => new SourcedPackageIdentity(y.Id, y.Version, x.Source)))
                    .ToImmutableList();
        }

        public virtual Task Add(SourcedPackageIdentity id)
        {
            if (Modules.Any(x => x.IsSameId(id)))
                throw new ArgumentException($"Module '{id}' already exists.", nameof(id));

            Modules = Modules.Add(id);
            return Save(Modules);
        }

        public virtual Task Remove(PackageIdentity id)
        {
            var module = Modules.FirstOrDefault(id.Equals);
            if (module == null)
                throw new ArgumentException($"Module '{id}' does not exist.", nameof(id));

            Modules = Modules.Remove(module);
            return Save(Modules);
        }

        public Task Replace(IEnumerable<SourcedPackageIdentity> modules)
        {
            Modules = modules.ToImmutableList();
            return Save(Modules);
        }

        protected virtual Task Save(IEnumerable<SourcedPackageIdentity> ids)
        {
            return Save(ids.GroupBy(x => x.SourceRepository).Select(x =>
                new ModuleRepositoryGroup {Source = x.Key, Modules = x.Cast<PackageIdentity>().ToList()}).ToList());
        }
    }
}