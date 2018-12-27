using System;

namespace Orcus.Server.Service
{
    public class ClientNotFoundException : Exception
    {
        public ClientNotFoundException(int clientId) : base($"The client with id '{clientId}' was not found.")
        {
        }
    }
}