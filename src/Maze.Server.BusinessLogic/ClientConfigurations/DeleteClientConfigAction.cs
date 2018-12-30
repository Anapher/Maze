using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Maze.Server.BusinessDataAccess.ClientConfigurations;
using Maze.Server.Connection;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.BusinessLogic.ClientConfigurations
{
    public interface IDeleteClientConfigAction : IGenericActionWriteDbAsync<int, ClientConfiguration>
    {
    }

    public class DeleteClientConfigAction : BusinessActionErrors, IDeleteClientConfigAction
    {
        private readonly IClientConfigurationDbAccess _dbAccess;

        public DeleteClientConfigAction(IClientConfigurationDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<ClientConfiguration> BizActionAsync(int inputData)
        {
            var clientConfiguration = await _dbAccess.FindAsync(inputData);
            if (clientConfiguration == null)
                return ReturnError<ClientConfiguration>(BusinessErrors.ClientConfigurations.NotFound);

            _dbAccess.Remove(clientConfiguration);
            return clientConfiguration;
        }
    }
}