using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Orcus.Administration.Modules.Extensions;

namespace Orcus.Administration.Modules.Models
{
    // This is the model class behind the package items in the infinite scroll list.
    // Some of its properties, such as Latest Version, Status, are fetched on-demand in the background.
    public class PackageItemListViewModel : INotifyPropertyChanged
    {
        private string _author;

        // True if the package is AutoReferenced
        private bool _autoReferenced;

        private Lazy<Task<NuGetVersion>> _backgroundLoader;

        private long? _downloadCount;

        // The installed version of the package.
        private NuGetVersion _installedVersion;

        // The version that can be installed or updated to. It is null
        // if the installed version is already the latest.
        private NuGetVersion _latestVersion;
        
        private bool _prefixReserved;

        private bool _selected;

        private PackageStatus _status = PackageStatus.NotInstalled;

        public string Id { get; set; }

        public NuGetVersion Version { get; set; }

        public VersionRange AllowedVersions { get; set; }

        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        public NuGetVersion InstalledVersion
        {
            get => _installedVersion;
            set
            {
                if (!VersionEquals(_installedVersion, value))
                {
                    _installedVersion = value;
                    OnPropertyChanged(nameof(InstalledVersion));
                }
            }
        }

        public NuGetVersion LatestVersion
        {
            get => _latestVersion;
            set
            {
                if (!VersionEquals(_latestVersion, value))
                {
                    _latestVersion = value;
                    OnPropertyChanged(nameof(LatestVersion));
                }
            }
        }

        public bool AutoReferenced
        {
            get => _autoReferenced;
            set
            {
                _autoReferenced = value;
                OnPropertyChanged(nameof(AutoReferenced));
            }
        }

        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged(nameof(Selected));
                }
            }
        }

        public long? DownloadCount
        {
            get => _downloadCount;
            set
            {
                _downloadCount = value;
                OnPropertyChanged(nameof(DownloadCount));
            }
        }

        public string Summary { get; set; }

        public PackageStatus Status
        {
            get
            {
                TriggerStatusLoader();
                return _status;
            }

            private set
            {
                bool refresh = _status != value;
                _status = value;

                if (refresh) OnPropertyChanged(nameof(Status));
            }
        }

        public bool PrefixReserved
        {
            get => _prefixReserved;
            set
            {
                if (_prefixReserved != value)
                {
                    _prefixReserved = value;
                    OnPropertyChanged(nameof(PrefixReserved));
                }
            }
        }

        public Uri IconUrl { get; set; }

        public Lazy<Task<IEnumerable<VersionInfo>>> Versions { private get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private bool VersionEquals(NuGetVersion v1, NuGetVersion v2)
        {
            if (v1 == null && v2 == null) return true;

            if (v1 == null) return false;

            return v1.Equals(v2, VersionComparison.Default);
        }

        public Task<IEnumerable<VersionInfo>> GetVersionsAsync()
        {
            return Versions.Value;
        }

        private async void TriggerStatusLoader()
        {
            if (!_backgroundLoader.IsValueCreated)
            {
                var result = await _backgroundLoader.Value;
                LatestVersion = result;
                Status = GetPackageStatus(LatestVersion, InstalledVersion, AutoReferenced);
            }
        }

        //public void UpdatePackageStatus(IEnumerable<InstalledModule> installedPackages)
        //{
        //    // Get the minimum version installed in any target project/solution
        //    InstalledVersion = installedPackages
        //        .GetPackageVersions(Id)
        //        .MinOrDefault();

        //    // Set auto referenced to true any reference for the given id contains the flag.
        //    //AutoReferenced = installedPackages.IsAutoReferenced(Id); //TODO

        //    _backgroundLoader = AsyncLazy.New(
        //        async () =>
        //        {
        //            var packageVersions = await GetVersionsAsync();

        //            // filter package versions based on allowed versions in pacakge.config
        //            packageVersions = packageVersions.Where(v => AllowedVersions.Satisfies(v.Version));
        //            var latestAvailableVersion = packageVersions
        //                .Select(p => p.Version)
        //                .MaxOrDefault();

        //            return latestAvailableVersion;
        //        });

        //    OnPropertyChanged(nameof(Status));
        //}

        private static PackageStatus GetPackageStatus(NuGetVersion latestAvailableVersion,
            NuGetVersion installedVersion, bool autoReferenced)
        {
            var status = PackageStatus.NotInstalled;

            if (autoReferenced)
            {
                status = PackageStatus.AutoReferenced;
            }
            else if (installedVersion != null)
            {
                status = PackageStatus.Installed;

                if (VersionComparer.VersionRelease.Compare(installedVersion, latestAvailableVersion) < 0)
                    status = PackageStatus.UpdateAvailable;
            }

            return status;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, e);
            }
        }

        public override string ToString()
        {
            return Id;
        }
    }
}