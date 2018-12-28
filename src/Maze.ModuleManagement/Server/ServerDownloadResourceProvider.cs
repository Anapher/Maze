using System;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Maze.ModuleManagement.Server
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
}