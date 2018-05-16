using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Orcus.Administration.Core.Clients
{
    public interface IOrcusRestClient : IDisposable
    {
        string Username { get; }
        Task<HttpResponseMessage> SendMessage(HttpRequestMessage request);
        Task<HubConnection> CreateHubConnection(string resource);
    }
}