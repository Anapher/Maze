namespace Orcus.Administration.ViewModels.Main.Overview.Modules
{
    public enum ModuleStatus
    {
        NotInstalled,

        // the latest applicable version is installed.
        Installed,

        UpdateAvailable,

        // The package is installed but may not be managed.
        AutoReferenced
    }
}