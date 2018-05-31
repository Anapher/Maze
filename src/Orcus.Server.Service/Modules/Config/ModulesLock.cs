using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using NuGet.Frameworks;
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
        IImmutableDictionary<NuGetFramework, PackagesLock> Modules { get; }

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
        Task Add(NuGetFramework framework, PackagesLock packagesLock);

        /// <summary>
        ///     Remove a module
        /// </summary>
        Task Remove(NuGetFramework framework);

        /// <summary>
        ///     Replace the whole module list
        /// </summary>
        /// <param name="modules">The new list</param>
        Task Replace(IReadOnlyDictionary<NuGetFramework, PackagesLock> modules);
    }

    public class ModulesLock : JsonObjectFile<IImmutableDictionary<NuGetFramework, PackagesLock>>,  IModulesLock
    {
        private readonly IImmutableDictionary<NuGetFramework, PackagesLock> _empty;

        public ModulesLock(string path) : base(path)
        {
            _empty = new Dictionary<NuGetFramework, PackagesLock>().ToImmutableDictionary(NuGetFramework.Comparer);
            Modules = _empty;

            JsonSettings.Converters.Add(new PackageIdentityConverter());
            JsonSettings.Converters.Add(new NuGetVersionConverter());
        }

        public IImmutableDictionary<NuGetFramework, PackagesLock> Modules { get; private set; }

        public virtual async Task Reload()
        {
            var data = await Load();
            Modules = data == null
                ? _empty
                : data.ToImmutableDictionary(NuGetFramework.Comparer);
        }

        public virtual Task Add(NuGetFramework framework, PackagesLock packagesLock)
        {
            Modules = Modules.Add(framework, packagesLock);
            return Save(Modules);
        }

        public virtual Task Remove(NuGetFramework framework)
        {
            Modules = Modules.Remove(framework);
            return Save(Modules);
        }

        public Task Replace(IReadOnlyDictionary<NuGetFramework, PackagesLock> modules)
        {
            Modules = modules.ToImmutableDictionary(NuGetFramework.Comparer);
            return Save(Modules);
        }
    }
}