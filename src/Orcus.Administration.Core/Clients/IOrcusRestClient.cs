using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Orcus.Administration.Core.Clients
{
    public interface IOrcusRestClient : IRestClient, IDisposable
    {
        string Username { get; }
        Task<HubConnection> CreateHubConnection(string resource);
    }
}