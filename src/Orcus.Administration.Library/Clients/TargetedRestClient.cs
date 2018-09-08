using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using Orcus.Administration.Library.Channels;
using Orcus.Administration.Library.Extensions;
using Orcus.Modules.Api;

namespace Orcus.Administration.Library.Clients
{
    public class TargetedRestClient : ITargetedRestClient
    {
        private static readonly Uri RelativeBaseUri = new Uri("v1/modules", UriKind.Relative);
        private readonly string _commandTargetHeader;
        private readonly IOrcusRestClient _orcusRestClient;

        public TargetedRestClient(IOrcusRestClient orcusRestClient, int clientId)
        {
            _orcusRestClient = orcusRestClient;
            _commandTargetHeader = "C" + clientId;
        }

        public Task<HttpResponseMessage> SendMessage(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            PatchMessage(request);
            return _orcusRestClient.SendMessage(request, cancellationToken);
        }

        public void Dispose()
        {
        }

        public string Username => _orcusRestClient.Username;
        public HubConnection HubConnection => _orcusRestClient.HubConnection;
        public IComponentContext ServiceProvider => _orcusRestClient.ServiceProvider;

        public Task<TChannel> OpenChannel<TChannel>(HttpRequestMessage message, CancellationToken cancellationToken)
            where TChannel : IAwareDataChannel
        {
            PatchMessage(message);
            return _orcusRestClient.OpenChannel<TChannel>(message, cancellationToken);
        }

        private void PatchMessage(HttpRequestMessage request)
        {
            request.Headers.Add("CommandTarget", _commandTargetHeader);

            //request.Uri = TestModule/Controller
            request.RequestUri = UriHelper.CombineRelativeUris(RelativeBaseUri, request.RequestUri);
        }
    }
}