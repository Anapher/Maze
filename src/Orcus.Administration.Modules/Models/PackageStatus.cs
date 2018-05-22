namespace Orcus.Administration.Modules.Models
{
    public enum PackageStatus
    {
        NotInstalled,

        // the latest applicable version is installed.
        Installed,

        UpdateAvailable,

        // The package is installed but may not be managed.
        AutoReferenced
    }
}