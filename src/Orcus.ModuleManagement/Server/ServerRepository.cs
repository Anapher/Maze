using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Orcus.ModuleManagement.Server
{
    public class ServerDownloadResourceProvider : ResourceProvider
    {
        private readonly string _baseUri;

        public ServerDownloadResourceProvider(string baseUri) : base(typeof(DownloadResource),
            nameof(ServerDownloadResourceProvider))
        {
            _baseUri = baseUri;
        }

        public override async Task<Tuple<bool, INuGetResource>> TryCreate(SourceRepository source,
            CancellationToken token)
        {
            var httpSourceResource = await source.GetResourceAsync<HttpSourceResource>(token);
            var client = httpSourceResource.HttpSource;

            return new Tuple<bool, INuGetResource>(true, new DownloadResourceV3(client, _baseUri));
        }
    }

    public class ServerRepository : SourceRepository
    {
        public ServerRepository(Uri serverNuGetUri) : base(new PackageSource(serverNuGetUri.AbsoluteUri),
            GetResourceProviders(serverNuGetUri))
        {
        }

        private static IEnumerable<INuGetResourceProvider> GetResourceProviders(Uri serverNuGetUri)
        {
            yield return new HttpSourceResourceProvider();
            yield return new ServerDownloadResourceProvider(serverNuGetUri.AbsoluteUri);
        }
    }
}