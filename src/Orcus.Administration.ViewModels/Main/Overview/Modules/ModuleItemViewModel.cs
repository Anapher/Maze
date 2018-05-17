using System;
using Anapher.Wpf.Swan;
using NuGet.Versioning;

namespace Orcus.Administration.ViewModels.Main.Overview.Modules
{
    public class ModuleItemViewModel : PropertyChangedBase
    {
        private ModuleStatus _status;

        public string Id { get; set; }
        public string Author { get; set; }
        public NuGetVersion Version { get; set; }
        public NuGetVersion LatestVersion { get; set; }
        public bool AutoReferenced { get; set; }
        public long? DownloadCount { get; set; }

        public ModuleStatus Status
        {
            get => _status;
            set => SetProperty(value, ref _status);
        }

        public Uri IconUri { get; set; }
    }
}