using System.Threading;
using System.Threading.Tasks;
using Orcus.Administration.Library.Channels;
using Orcus.Administration.Library.Clients.Helpers;

namespace Orcus.Administration.Library.Clients
{
    public abstract class ChannelResource<TResource> : ResourceBase<TResource> where TResource : IResourceId, new()
    {
        protected ChannelResource(string resource) : base(resource)
        {
        }

        protected static Task<TChannel> CreateChannel<TChannel>(string path, ITargetedRestClient restClient) where TChannel : IAwareDataChannel =>
            CreateRequest(HttpVerb.Get, path).CreateChannel<TChannel>(restClient, CancellationToken.None);
    }
}