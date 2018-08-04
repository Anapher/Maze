using Orcus.Server.Connection.Authentication.Client;

namespace Orcus.Core.Connection
{
    public interface IClientInfoProvider
    {
        ClientAuthenticationDto GetAuthenticationDto();
    }

    public class ClientInfoProvider : IClientInfoProvider
    {
        private readonly IApplicationInfo _applicationInfo;

        public ClientInfoProvider(IApplicationInfo applicationInfo)
        {
            _applicationInfo = applicationInfo;
        }

        public ClientAuthenticationDto GetAuthenticationDto()
        {
            return new ClientAuthenticationDto {ClientVersion = _applicationInfo.Version};
        }
    }
}