using System.Collections.Generic;
using NuGet.Packaging.Core;

namespace Orcus.Server.Service.Modules.Preview
{
    public class PreviewResult
    {
        public PreviewResult(IEnumerable<PackageIdentity> added, IEnumerable<PackageIdentity> deleted,
            IEnumerable<UpdatePreviewResult> updated)
        {
            Added = added;
            Deleted = deleted;
            Updated = updated;
        }

        public IEnumerable<PackageIdentity> Deleted { get; }
        public IEnumerable<PackageIdentity> Added { get; }
        public IEnumerable<UpdatePreviewResult> Updated { get; }
    }

    public class UpdatePreviewResult
    {
        public PackageIdentity Old { get; }
        public PackageIdentity New { get; }

        public UpdatePreviewResult(PackageIdentity oldPackage, PackageIdentity newPackage)
        {
            Old = oldPackage;
            New = newPackage;
        }

        public override string ToString()
        {
            return Old + " -> " + New;
        }
    }
}