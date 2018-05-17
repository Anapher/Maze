using NuGet.Frameworks;

namespace Orcus.Server.Connection
{
    public class OrcusFrameworks
    {
        public static readonly NuGetFramework Server = new NuGetFramework("net");
        public static readonly NuGetFramework Client = new NuGetFramework("win");
        public static readonly NuGetFramework Administration = new NuGetFramework("uap");
    }
}