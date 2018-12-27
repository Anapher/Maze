using System.Collections.Generic;
using System.Linq;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orcus.Server.Connection.Modules;

namespace Orcus.Server.Service.Modules
{
    public class ResolvedAction
    {
        public ResolvedAction(PackageIdentity packageIdentity, SourceRepository sourceRepository,
            ResolvedActionType action)
        {
            PackageIdentity = packageIdentity;
            SourceRepository = sourceRepository;
            Action = action;
        }

        /// <summary>
        ///     PackageIdentity on which the action is performed
        /// </summary>
        public PackageIdentity PackageIdentity { get; }

        /// <summary>
        ///     For ResolvedActionType.Install, SourceRepository from which the package should be installed
        ///     For ResolvedActionType.Uninstall, this will be null
        /// </summary>
        public SourceRepository SourceRepository { get; }

        /// <summary>
        ///     Type of ResolvedActionType. Install/Uninstall
        /// </summary>
        public ResolvedActionType Action { get; }

        public static ResolvedAction CreateInstall(PackageIdentity packageIdentity, SourceRepository sourceRepository)
        {
            return new ResolvedAction(packageIdentity, sourceRepository, ResolvedActionType.Install);
        }

        public static ResolvedAction CreateInstall(SourcedPackageIdentity packageIdentity,
            IEnumerable<SourceRepository> repositories)
        {
            return new ResolvedAction(packageIdentity,
                repositories.First(x => x.PackageSource.SourceUri == packageIdentity.SourceRepository),
                ResolvedActionType.Install);
        }

        public static ResolvedAction CreateUninstall(PackageIdentity packageIdentity)
        {
            return new ResolvedAction(packageIdentity, null, ResolvedActionType.Uninstall);
        }
    }
}
