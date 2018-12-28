using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Prism.Mvvm;

namespace Maze.Administration.ViewModels.Overview.Modules
{
    public class ModuleViewModel : BindableBase
    {
        private string _authors;
        private List<PackageDependencyGroup> _dependencySets;
        private string _description;
        private ImageSource _image;
        private Uri _imageUri;
        private bool _isLoaded;
        private Uri _licenseUri;
        private Uri _projectUri;
        private DateTimeOffset? _published;
        private ModuleStatus _status;
        private string _summary;
        private string _title;
        private NuGetVersion _version;
        private List<NuGetVersion> _versions;
        private IPackageSearchMetadata _searchMetadata;
        private NuGetVersion _newestVersion;
        private bool _includePrerelease;

        public ModuleViewModel(PackageIdentity packageIdentity, ModuleStatus status)
        {
            PackageIdentity = packageIdentity;
            Status = status;
        }

        public PackageIdentity PackageIdentity { get; }

        public ModuleStatus Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value))
                    RaisePropertyChanged(nameof(IsUpdateAvailable));
            }
        }

        public bool IsUpdateAvailable => Status != ModuleStatus.None && Version != null && NewestVersion > Version;

        public bool IsLoaded
        {
            get => _isLoaded;
            private set => SetProperty(ref _isLoaded, value);
        }

        public string Title
        {
            get => _title;
            private set => SetProperty(ref _title, value);
        }

        public string Authors
        {
            get => _authors;
            private set => SetProperty(ref _authors, value);
        }

        public NuGetVersion Version
        {
            get => _version;
            private set
            {
                if (SetProperty(ref _version, value))
                    RaisePropertyChanged(nameof(IsUpdateAvailable));
            }
        }

        public Uri ImageUri
        {
            get => _imageUri;
            private set
            {
                if (SetProperty(ref _imageUri, value)) Image = new BitmapImage(value);
            }
        }

        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public string Summary
        {
            get => _summary;
            private set => SetProperty(ref _summary, value);
        }

        public string Description
        {
            get => _description;
            private set => SetProperty(ref _description, value);
        }

        public Uri LicenseUri
        {
            get => _licenseUri;
            private set => SetProperty(ref _licenseUri, value);
        }

        public Uri ProjectUri
        {
            get => _projectUri;
            private set => SetProperty(ref _projectUri, value);
        }

        public DateTimeOffset? Published
        {
            get => _published;
            private set => SetProperty(ref _published, value);
        }

        public List<PackageDependencyGroup> DependencySets
        {
            get => _dependencySets;
            set => SetProperty(ref _dependencySets, value);
        }

        public List<NuGetVersion> Versions
        {
            get => _versions;
            private set => SetProperty(ref _versions, value);
        }

        public NuGetVersion NewestVersion
        {
            get => _newestVersion;
            private set
            {
                if (SetProperty(ref _newestVersion, value))
                    RaisePropertyChanged(nameof(IsUpdateAvailable));
            }
        }

        public void Initialize(IPackageSearchMetadata metadata)
        {
            Title = metadata.Title;
            Authors = metadata.Authors;
            Version = metadata.Identity.Version;
            ImageUri = metadata.IconUrl;
            Summary = metadata.Summary;
            Description = metadata.Description;
            LicenseUri = metadata.LicenseUrl;
            ProjectUri = metadata.ProjectUrl;
            Published = metadata.Published;

            DependencySets = metadata.DependencySets.Select(x =>
                new PackageDependencyGroup(x.TargetFramework,
                    x.Packages.Select(y => new PackageDependency(y.Id, y.VersionRange, y.Include, y.Exclude)))).ToList();

            IsLoaded = true;
            _searchMetadata = metadata;
        }

        public async Task<bool> LoadVersionsAsync()
        {
            if (Versions?.Count > 0)
                return true;

            if (_searchMetadata == null)
                return false;

            try
            {
                var versions = await _searchMetadata.GetVersionsAsync();
                OnUpdateVersions(versions?.Select(x => x.Version).ToList());
            }
            catch (Exception)
            {
                return false;
            }

            if (Versions == null)
                return false;

            return true;
        }

        public void OnUpdateVersion(NuGetVersion version)
        {
            Version = version;
            OnUpdateVersions(Versions);
            Status = ModuleStatus.ToBeInstalled;
        }

        public void OnUninstall()
        {
            Version = NewestVersion;
        }

        public void OnUpdateVersions(List<NuGetVersion> versions)
        {
            Versions = versions;
            if (versions != null && Status != ModuleStatus.None)
            {
                IEnumerable<NuGetVersion> actualVersions = versions;
                if (!IncludePrerelease)
                    actualVersions = actualVersions.Where(x => !x.IsPrerelease);
                if (!actualVersions.Any())
                    actualVersions = versions;

                var newestVersion = actualVersions.Max(x => x);
                NewestVersion = newestVersion;
            }
        }

        public bool IncludePrerelease
        {
            get => _includePrerelease;
            set
            {
                if (_includePrerelease != value)
                {
                    _includePrerelease = value;
                    OnUpdateVersions(Versions);
                }
            }
        }
    }

    public enum ModuleStatus
    {
        None,
        Installed,
        ToBeInstalled
    }
}