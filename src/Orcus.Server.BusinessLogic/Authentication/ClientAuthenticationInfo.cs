using System.Net;
using Orcus.Server.Connection.Authentication.Client;

namespace Orcus.Server.BusinessLogic.Authentication
{
    public class ClientAuthenticationInfo
    {
        public ClientAuthenticationDto Dto { get; set; }
        public IPAddress IpAddress { get; set; }
    }
}