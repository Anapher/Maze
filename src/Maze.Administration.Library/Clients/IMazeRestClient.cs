using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using Orcus.Administration.Library.Channels;
using Orcus.Modules.Api;

namespace Orcus.Administration.Library.Clients
{
    public interface IOrcusRestClient : IRestClient, IDisposable
    {
        string Username { get; }
        HubConnection HubConnection { get; }
        IComponentContext ServiceProvider { get; }
        Task<TChannel> OpenChannel<TChannel>(HttpRequestMessage message, CancellationToken cancellationToken) where TChannel : IAwareDataChannel;
        Task<HttpResponseMessage> SendChannelMessage(HttpRequestMessage request, IDataChannel channel, CancellationToken cancellationToken);
    }
}