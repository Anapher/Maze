using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Administration.Library.Channels;
using Orcus.Administration.Library.Clients.Helpers;
using Orcus.Modules.Api;

namespace Orcus.Administration.Library.Clients
{
    public static class ChannelExtensions
    {
        public static async Task<TChannel> CreateChannel<TChannel>(this IRequestBuilder requestBuilder,
            ITargetedRestClient client, CancellationToken cancellationToken) where TChannel : IAwareDataChannel
        {
            using (var request = requestBuilder.Build())
            {
                return await client.OpenChannel<TChannel>(request, cancellationToken);
            }
        }

        public static async Task<HttpResponseMessage> ExecuteOnChannel(this IRequestBuilder requestBuilder, ITargetedRestClient client, IDataChannel channel)
        {
            using (var request = requestBuilder.Build())
                return await client.SendChannelMessage(request, channel, CancellationToken.None);
        }
    }
}