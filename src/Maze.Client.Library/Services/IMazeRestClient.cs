using Orcus.Client.Library.Clients;
using System;

namespace Orcus.Client.Library.Services
{
    public interface IOrcusRestClient : IRestClient
    {
        Uri BaseUri { get; }
        string Jwt { get; }
    }
}