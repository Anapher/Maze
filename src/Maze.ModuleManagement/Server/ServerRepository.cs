using System;
using System.Collections.Generic;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Maze.ModuleManagement.Server
{
    public class ServerRepository : SourceRepository
    {
        public ServerRepository(Uri serverNuGetUri) : base(new PackageSource(serverNuGetUri.AbsoluteUri), GetResourceProviders(serverNuGetUri))
        {
        }

        private static IEnumerable<INuGetResourceProvider> GetResourceProviders(Uri serverNuGetUri)
        {
            yield return new HttpHandlerResourceV3Provider();
            yield return new HttpSourceResourceProvider();
            yield return new ServerDownloadResourceProvider(serverNuGetUri.AbsoluteUri);
        }
    }
}