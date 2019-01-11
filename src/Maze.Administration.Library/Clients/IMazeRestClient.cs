using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Maze.Administration.Library.Channels;
using Maze.Modules.Api;

namespace Maze.Administration.Library.Clients
{
    public interface IMazeRestClient : IRestClient, IDisposable
    {
        string Username { get; }
        HubConnection HubConnection { get; }
        IServiceProvider ServiceProvider { get; }
        Task<TChannel> OpenChannel<TChannel>(HttpRequestMessage message, CancellationToken cancellationToken) where TChannel : IAwareDataChannel;
        Task<HttpResponseMessage> SendChannelMessage(HttpRequestMessage request, IDataChannel channel, CancellationToken cancellationToken);
    }
}