using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;

namespace Orcus.Administration.Core.Clients.Helpers
{
    public class RequestBuilder : IRequestBuilder
    {
        public RequestBuilder(string baseUrl)
        {
            BaseUrl = baseUrl;
            Query = new NameValueCollection();
        }

        public string BaseUrl { get; }
        public HttpMethod HttpMethod { get; set; }
        public HttpContent Content { get; set; }
        public NameValueCollection Query { get; }

        public HttpRequestMessage Build()
        {
            var url = BaseUrl;
            if (Query.Count > 0)
                url += "?" + string.Join("&",
                           Query.Cast<string>().Select(x =>
                               string.Concat(Uri.EscapeDataString(x), "=", Uri.EscapeDataString(Query[x]))));

            return new HttpRequestMessage(HttpMethod, new Uri(url, UriKind.Relative)) {Content = Content};
        }
    }
}