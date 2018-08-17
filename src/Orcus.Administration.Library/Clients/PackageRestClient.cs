using System;
using System.Net.Http;
using System.Threading.Tasks;
using Orcus.Administration.Library.Extensions;

namespace Orcus.Administration.Library.Clients
{
    public class PackageRestClient : IPackageRestClient
    {
        private readonly ITargetedRestClient _targetedRestClient;
        private readonly Uri _packageUri;

        public PackageRestClient(ITargetedRestClient targetedRestClient, string packageName)
        {
            _targetedRestClient = targetedRestClient;
            _packageUri = new Uri(packageName, UriKind.Relative);
        }

        public Task<HttpResponseMessage> SendMessage(HttpRequestMessage request)
        {
            request.RequestUri = UriHelper.CombineRelativeUris(_packageUri, request.RequestUri);
            return _targetedRestClient.SendMessage(request);
        }
    }
}