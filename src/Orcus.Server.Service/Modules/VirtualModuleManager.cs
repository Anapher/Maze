using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NuGet.Packaging.Core;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Modules.Config;

namespace Orcus.Server.Service.Modules
{
    public interface IVirtualModuleManager
    {
        IImmutableList<PackageIdentity> Packages { get; }

        InstalledModulesDto GetModules();
        void InstallPackage(PackageIdentity packageIdentity);
        void UninstallPackage(PackageIdentity packageIdentity);
    }

    public class VirtualModuleManager : IVirtualModuleManager
    {
        private readonly IModulesConfig _modulesConfig;
        private readonly IModuleProject _moduleProject;
        private readonly object _packageLock = new object();

        public VirtualModuleManager(IModulesConfig modulesConfig, IModuleProject moduleProject)
        {
            _modulesConfig = modulesConfig;
            _moduleProject = moduleProject;
        }

        // ReSharper disable once InconsistentlySynchronizedField
        public IImmutableList<PackageIdentity> Packages => _modulesConfig.Modules;

        public InstalledModulesDto GetModules()
        {
            var installed = new List<PackageIdentity>();
            var toBeInstalled = new List<PackageIdentity>();

            foreach (var packageIdentity in Packages)
            {
                if (_moduleProject.PrimaryPackages.Any(x => x.Equals(packageIdentity)))
                    installed.Add(packageIdentity);
                else toBeInstalled.Add(packageIdentity);
            }

            return new InstalledModulesDto {Installed = installed, ToBeInstalled = toBeInstalled};
        }

        public void InstallPackage(PackageIdentity packageIdentity)
        {
            lock (_packageLock)
            {
                UninstallPackage(packageIdentity);

                _modulesConfig.Add(packageIdentity);
            }
        }

        public void UninstallPackage(PackageIdentity packageIdentity)
        {
            lock (_packageLock)
            {
                var existingPackage = Packages.FirstOrDefault(x => x.Id == packageIdentity.Id);
                if (existingPackage != null) _modulesConfig.Remove(existingPackage);
            }
        }
    }
}