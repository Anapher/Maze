using System;
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
    public interface ICreateClientConfigAction : IGenericActionAsync<ClientConfigurationDto, ClientConfiguration> { }

    public class CreateClientConfigAction : BusinessActionErrors, ICreateClientConfigAction
    {
        private readonly IClientConfigurationDbAccess _dbAccess;

        public CreateClientConfigAction(IClientConfigurationDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<ClientConfiguration> BizActionAsync(ClientConfigurationDto inputData)
        {
            if (ValidateModelFailed(inputData))
                return default;

            if (inputData.ClientGroupId == null)
                return ReturnError<ClientConfiguration>(BusinessErrors.ClientConfigurations.CannotCreateGlobalConfig);

            var clientConfig = await _dbAccess.FindAsync(inputData.ClientGroupId.Value);
            if (clientConfig != null)
                return ReturnError<ClientConfiguration>(BusinessErrors.ClientConfigurations.ConfigAlreadyExists);

            var model = new ClientConfiguration
            {
                ClientGroupId = inputData.ClientGroupId,
                Content = inputData.Content,
                UpdatedOn = DateTimeOffset.UtcNow,
                ContentHash = (int) MurmurHash2.Hash(inputData.Content)
            };

            _dbAccess.Add(model);
            return model;
        }
    }
}