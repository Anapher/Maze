using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Maze.Server.BusinessDataAccess.ClientConfigurations;
using Maze.Server.BusinessLogic.Utilities;
using Maze.Server.Connection;
using Maze.Server.Connection.Clients;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.BusinessLogic.ClientConfigurations
{
    public interface IUpdateClientConfigAction : IGenericActionAsync<ClientConfigurationDto, ClientConfiguration> { }

    public class UpdateClientConfigAction : BusinessActionErrors, IUpdateClientConfigAction
    {
        private readonly IClientConfigurationDbAccess _dbAccess;

        public UpdateClientConfigAction(IClientConfigurationDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<ClientConfiguration> BizActionAsync(ClientConfigurationDto inputData)
        {
            if (ValidateModelFailed(inputData))
                return default;

            var clientConfig = await _dbAccess.FindAsync(inputData.ClientGroupId);
            if (clientConfig == null)
                return ReturnError<ClientConfiguration>(BusinessErrors.ClientConfigurations.NotFound);

            clientConfig.Content = inputData.Content;
            clientConfig.ContentHash = (int) MurmurHash2.Hash(inputData.Content);
            clientConfig.UpdatedOn = DateTimeOffset.UtcNow;

            return clientConfig;
        }
    }
}
