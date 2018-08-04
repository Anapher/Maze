using System;
using Orcus.Client.Library.Services;
using Orcus.Core.Connection;

namespace Orcus.Core.Services
{
    public interface IOrcusRestClientFactory
    {
        OrcusRestClient Create(Uri baseUri);
    }

    public class OrcusRestClientFactory : IOrcusRestClientFactory
    {
        private readonly IHttpClientService _httpClientService;

        public OrcusRestClientFactory(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public OrcusRestClient Create(Uri baseUri) => new OrcusRestClient(_httpClientService.Client, baseUri);
    }
}