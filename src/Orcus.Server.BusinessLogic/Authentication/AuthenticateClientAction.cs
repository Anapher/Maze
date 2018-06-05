using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using CodeElements.BizRunner.Generic;
using Microsoft.Extensions.Logging;
using Orcus.Server.BusinessDataAccess.Clients;
using Orcus.Server.Connection;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.BusinessLogic.Authentication
{
    public interface IAuthenticateClientAction : IGenericActionWriteDbAsync<ClientAuthenticationInfo, Client>
    {
    }

    public class AuthenticateClientAction : LoggingBusinessActionErrors, IAuthenticateClientAction
    {
        private readonly IClientDbAccess _dbAccess;

        public AuthenticateClientAction(IClientDbAccess dbAccess, ILogger<AuthenticateClientAction> logger) :
            base(logger)
        {
            _dbAccess = dbAccess;
        }

        public async Task<Client> BizActionAsync(ClientAuthenticationInfo inputData)
        {
            if (ValidateModelFailed(inputData.Dto))
                return default;

            var normalizedHwid = Hash.Parse(inputData.Dto.HardwareId).ToString();
            var client = await _dbAccess.FindClientByHardwareId(normalizedHwid);
            if (client == null)
            {
                client = new Client
                {
                    HardwareId = normalizedHwid,
                    ClientSessions = new List<ClientSession>()
                };

                _dbAccess.AddClient(client);
            }

            client.OperatingSystem = inputData.Dto.OperatingSystem;
            client.MacAddress = new PhysicalAddress(inputData.Dto.MacAddress).ToString();
            client.Username = inputData.Dto.Username;
            client.SystemLanguage = inputData.Dto.SystemLanguage;
            client.ClientSessions.Add(new ClientSession
            {
                IsAdministrator = inputData.Dto.IsAdministrator,
                ClientVersion = inputData.Dto.ClientVersion.ToString(),
                ClientPath = inputData.Dto.ClientPath,
                IpAddress = inputData.IpAddress.ToString()
            });

            return client;
        }
    }
}