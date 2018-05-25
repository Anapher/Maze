using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using Orcus.Server.Connection.JsonConverters;
using Orcus.Server.Service.Modules.Config.Base;

namespace Orcus.Server.Service.Modules.Config
{
    /// <summary>
    ///     Provides all required modules including their dependencies
    /// </summary>
    public interface IModulesLock
    {
        /// <summary>
        ///     All modules including their dependencies
        /// </summary>
        IImmutableDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> Modules { get; }

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
        /// <param name="dependencies">It's depdendencies</param>
        Task Add(PackageIdentity id, IReadOnlyList<PackageIdentity> dependencies);

        /// <summary>
        ///     Remove a module
        /// </summary>
        /// <param name="id">The module identity</param>
        Task Remove(PackageIdentity id);

        /// <summary>
        ///     Replace the whole module list
        /// </summary>
        /// <param name="modules">The new list</param>
        Task Replace(IDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> modules);
    }

    public class ModulesLock : JsonObjectFile<IImmutableDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>>>,
        IModulesLock
    {
        private readonly IImmutableDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> _empty;

        public ModulesLock(string path) : base(path)
        {
            _empty =
                new Dictionary<PackageIdentity, IReadOnlyList<PackageIdentity>>().ToImmutableDictionary(PackageIdentity
                    .Comparer);
            Modules = _empty;

            JsonSettings.Converters.Add(new PackageIdentityConverter());
            JsonSettings.Converters.Add(new NuGetVersionConverter());
        }

        public IImmutableDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> Modules { get; private set; }

        public virtual async Task Reload()
        {
            var data = await Load();
            Modules = data == null
                ? _empty
                : data.ToImmutableDictionary(PackageIdentity.Comparer);
        }

        public virtual Task Add(PackageIdentity id, IReadOnlyList<PackageIdentity> dependencies)
        {
            Modules = Modules.Add(id, dependencies);
            return Save(Modules);
        }

        public virtual Task Remove(PackageIdentity id)
        {
            Modules = Modules.Remove(id);
            return Save(Modules);
        }

        public Task Replace(IDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> modules)
        {
            Modules = modules.ToImmutableDictionary(PackageIdentity.Comparer);
            return Save(Modules);
        }
    }
}