using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using NuGet.Packaging.Core;

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
}