using System;
using System.Threading.Tasks;
using CodeElements.BizRunner.Generic;
using Microsoft.Extensions.Logging;
using Orcus.Server.BusinessDataAccess.Clients;
using Orcus.Server.Connection.Authentication.Client;
using Orcus.Server.Connection.Clients;

namespace Orcus.Server.BusinessLogic.Authentication
{
    public interface IAuthenticateClientAction : IGenericActionWriteDbAsync<ClientAuthenticationInfo, ClientDto> { }

    public class AuthenticateClientAction : LoggingBusinessActionErrors, IAuthenticateClientAction
    {
        private readonly IClientDbAccess _dbAccess;

        public AuthenticateClientAction(IClientDbAccess dbAccess, ILogger<AuthenticateClientAction> logger) :
            base(logger)
        {
            _dbAccess = dbAccess;
        }

        public async Task<ClientDto> BizActionAsync(ClientAuthenticationInfo inputData)
        {
            if (!ValidateModelFailed(inputData))
                return default;

            var client = await _dbAccess.FindClientByHardwareId(inputData.HardwareId);
            throw new NotImplementedException();
        }
    }
}