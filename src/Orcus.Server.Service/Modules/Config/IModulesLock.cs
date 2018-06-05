using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using NuGet.Frameworks;
using Orcus.Server.Connection.Modules;

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
}