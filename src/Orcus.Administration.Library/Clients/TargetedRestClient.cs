using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Administration.Library.Extensions;

namespace Orcus.Administration.Library.Clients
{
    public class TargetedRestClient : ITargetedRestClient
    {
        private readonly IOrcusRestClient _orcusRestClient;
        private readonly string _commandTargetHeader;
        private static readonly Uri RelativeBaseUri = new Uri("v1/modules", UriKind.Relative);

        public TargetedRestClient(IOrcusRestClient orcusRestClient, int clientId)
        {
            _orcusRestClient = orcusRestClient;
            _commandTargetHeader = "C" + clientId;
        }

        public Task<HttpResponseMessage> SendMessage(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("CommandTarget", _commandTargetHeader);

            //request.Uri = TestModule/Controller
            request.RequestUri = UriHelper.CombineRelativeUris(RelativeBaseUri, request.RequestUri);
            return _orcusRestClient.SendMessage(request, cancellationToken);
        }
    }
}