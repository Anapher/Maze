using NuGet.Frameworks;
using NuGet.Versioning;

namespace Orcus.Core
{
    public interface IApplicationInfo
    {
        NuGetFramework Framework { get; }
        NuGetVersion Version { get; }
    }
}