using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using Orcus.Server.Connection.JsonConverters;
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
        IImmutableList<PackageIdentity> Modules { get; }

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
        Task Add(PackageIdentity id);

        /// <summary>
        ///     Remove a module
        /// </summary>
        /// <param name="id">The module identity</param>
        Task Remove(PackageIdentity id);

        /// <summary>
        ///     Replace the whole module list
        /// </summary>
        /// <param name="modules">The new list</param>
        Task Replace(IEnumerable<PackageIdentity> modules);
    }

    public class ModulesConfig : JsonObjectFile<IReadOnlyList<PackageIdentity>>, IModulesConfig
    {
        public ModulesConfig(string path) : base(path)
        {
            Modules = ImmutableList<PackageIdentity>.Empty;

            JsonSettings.Converters.Add(new PackageIdentityConverter());
            JsonSettings.Converters.Add(new NuGetVersionConverter());
        }

        public IImmutableList<PackageIdentity> Modules { get; private set; }

        public virtual async Task Reload()
        {
            var data = await Load();
            Modules = data?.ToImmutableList() ?? ImmutableList<PackageIdentity>.Empty;
        }

        public virtual Task Add(PackageIdentity id)
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

        public Task Replace(IEnumerable<PackageIdentity> modules)
        {
            Modules = modules.ToImmutableList();
            return Save(Modules);
        }
    }
}