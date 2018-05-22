using System.Collections.Generic;
using System.Linq;
using NuGet.Versioning;

namespace Orcus.Administration.Modules.Extensions
{
    /// <summary>
    ///     Extension methods for collection of <see cref="NuGetVersion" />
    /// </summary>
    public static class VersionCollectionExtensions
    {
        public static NuGetVersion MinOrDefault(this IEnumerable<NuGetVersion> versions)
        {
            return versions
                .OrderBy(v => v, VersionComparer.Default)
                .FirstOrDefault();
        }

        public static NuGetVersion MaxOrDefault(this IEnumerable<NuGetVersion> versions)
        {
            return versions
                .OrderByDescending(v => v, VersionComparer.Default)
                .FirstOrDefault();
        }
    }
}