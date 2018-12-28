using NuGet.Frameworks;
using NuGet.Versioning;

namespace Maze.Core
{
    public interface IApplicationInfo
    {
        NuGetFramework Framework { get; }
        NuGetVersion Version { get; }
    }
}