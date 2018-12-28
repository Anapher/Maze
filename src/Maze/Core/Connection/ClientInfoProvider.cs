using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Forms;
using Maze.Server.Connection.Authentication.Client;

namespace Maze.Core.Connection
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
            var result = new ClientAuthenticationDto
            {
                ClientVersion = _applicationInfo.Version,
                Framework = _applicationInfo.Framework,
                Username = Environment.UserName,
                OperatingSystem = Environment.OSVersion.ToString(),
                SystemLanguage = CultureInfo.CurrentUICulture.ToString(),
                IsAdministrator = false,
                ClientPath = Application.StartupPath,
                HardwareId = "FF",
                MacAddress = new byte[] {1, 1, 1, 1, 1, 1}
            };
            
            return result;
        }
    }
}