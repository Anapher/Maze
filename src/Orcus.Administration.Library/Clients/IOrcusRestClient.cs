using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Orcus.Administration.Library.Clients
{
    public interface IOrcusRestClient : IRestClient, IDisposable
    {
        string Username { get; }
        HubConnection HubConnection { get; }
    }
}