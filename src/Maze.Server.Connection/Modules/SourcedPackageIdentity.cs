using System;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace Orcus.Server.Connection.Modules
{
    public class SourcedPackageIdentity : PackageIdentity
    {
        public SourcedPackageIdentity(string id, NuGetVersion version, Uri sourceRepository) : base(id, version)
        {
            SourceRepository = sourceRepository;
        }

        public Uri SourceRepository { get; }
    }
}