using System;
using System.Net.Http;

namespace Orcus.Administration.Core.Clients.Helpers
{
    public abstract class Resource<TResource> : IResourceId where TResource : IResourceId, new()
    {
        public abstract string ResourceUri { get; }

        protected static string CombineUrlWithResource(string url, string resource)
        {
            if (resource == null)
                return url;
            return url + "/" + resource;
        }

        protected static RequestBuilder CreateRequest(HttpVerb verb = HttpVerb.Get, object resource = null, object content = null)
        {
            return CreateRequest(verb, resource, content == null ? null : new JsonContent(content));
        }

        protected static RequestBuilder CreateRequest(HttpVerb verb, object resource, HttpContent content)
        {
            HttpMethod method;
            switch (verb)
            {
                case HttpVerb.Get:
                    method = HttpMethod.Get;
                    break;
                case HttpVerb.Post:
                    method = HttpMethod.Post;
                    break;
                case HttpVerb.Put:
                    method = HttpMethod.Put;
                    break;
                case HttpVerb.Delete:
                    method = HttpMethod.Delete;
                    break;
                case HttpVerb.Patch:
                    method = new HttpMethod("PATCH");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(verb), verb, null);
            }

            return new RequestBuilder(CombineUrlWithResource(new TResource().ResourceUri, resource?.ToString()))
            {
                Content = content,
                HttpMethod = method
            };
        }
    }
}