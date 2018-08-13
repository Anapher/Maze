using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anapher.Wpf.Swan;
using NuGet.Packaging;
using NuGet.Versioning;
using Orcus.Server.Connection.Modules;

namespace Orcus.Administration.ViewModels.Main.Overview.Modules
{
    public abstract class ModuleViewModel : PropertyChangedBase
    {
        public abstract string DisplayName { get; }
        public abstract string Authors { get; }
        public abstract NuGetVersion Version { get; }
        public abstract Uri ImageUri { get; }
        public abstract string Summary { get; }
        public abstract string Description { get; }
        public abstract string Copyright { get; }
        public abstract Uri LicenseUri { get; }
        public abstract Uri ProjectUri { get;  }
        public abstract List<PackageDependencyGroup> Dependencies { get; }
        public abstract bool IsInstalled { get; }
    }

    public class OnlinePackageInfo
    {
        public DateTimeOffset PublishedOn { get; }
        public long DownloadCount { get; }
        public List<NuGetVersion> AvailableVersions { get; }

    }

    public class InstalledModuleViewModel : ModuleViewModel
    {
        private readonly InstalledModule _moduleDto;

        public InstalledModuleViewModel(InstalledModule moduleDto)
        {
            _moduleDto = moduleDto;
        }

        public override string DisplayName =>
            string.IsNullOrEmpty(_moduleDto.Title) ? _moduleDto.Id.Id : _moduleDto.Title;


    }
}
