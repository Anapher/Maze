//using System;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using Autofac;
//using Microsoft.AspNetCore.SignalR.Client;
//using Orcus.Administration.Library.Channels;
//using Orcus.Administration.Library.Extensions;
//using Orcus.Modules.Api;

//namespace Orcus.Administration.Library.Clients
//{
//    public class PackageRestClient : IPackageRestClient
//    {
//        private readonly ITargetedRestClient _targetedRestClient;
//        private readonly Uri _packageUri;

//        public PackageRestClient(ITargetedRestClient targetedRestClient, string packageName)
//        {
//            _targetedRestClient = targetedRestClient;
//            _packageUri = new Uri(packageName, UriKind.Relative);
//        }

//        public void Dispose()
//        {
//        }

//        public string Username => _targetedRestClient.Username;
//        public HubConnection HubConnection => _targetedRestClient.HubConnection;
//        public IComponentContext ServiceProvider => _targetedRestClient.ServiceProvider;

//        public Task<HttpResponseMessage> SendMessage(HttpRequestMessage request, CancellationToken cancellationToken)
//        {
//            PatchMessage(request);
//            return _targetedRestClient.SendMessage(request, cancellationToken);
//        }

//        public Task<TChannel> OpenChannel<TChannel>(HttpRequestMessage message, CancellationToken cancellationToken) where TChannel : IAwareDataChannel
//        {
//            PatchMessage(message);
//            return _targetedRestClient.OpenChannel<TChannel>(message, cancellationToken);
//        }

//        public Task<HttpResponseMessage> SendChannelMessage(HttpRequestMessage request, IDataChannel channel, CancellationToken cancellationToken)
//        {
//            PatchMessage(request);
//            return _targetedRestClient.SendChannelMessage(request, channel, cancellationToken);
//        }

//        private void PatchMessage(HttpRequestMessage request)
//        {
//            request.RequestUri = UriHelper.CombineRelativeUris(_packageUri, request.RequestUri);
//        }
//    }
//}