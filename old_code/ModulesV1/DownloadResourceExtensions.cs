using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Orcus.Server.Service.ModulesV1
{
    public static class DownloadResourceExtensions
    {
        public static Task<Uri> GetDownloadUrl(this DownloadResource downloadResource, PackageIdentity identity,
            ILogger logger, CancellationToken cancellationToken)
        {
            var method =
                typeof(DownloadResourceV3).GetMethod("GetDownloadUrl", BindingFlags.NonPublic | BindingFlags.Instance);
            return (Task<Uri>) method.Invoke(downloadResource, new object[] {identity, logger, cancellationToken});
        }
    }
}