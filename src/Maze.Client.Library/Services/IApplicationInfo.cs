using NuGet.Frameworks;
using NuGet.Versioning;

namespace Maze.Client.Library.Services
{
    /// <summary>
    ///     Information about the current Maze application
    /// </summary>
    public interface IApplicationInfo
    {
        /// <summary>
        ///     The client framework
        /// </summary>
        NuGetFramework Framework { get; }

        /// <summary>
        ///     The current version
        /// </summary>
        NuGetVersion Version { get; }
    }
}