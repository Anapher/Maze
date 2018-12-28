using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Maze.Server.BusinessDataAccess.ClientGroups;
using Maze.Server.Connection;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.BusinessLogic.ClientGroups
{
    public interface IDeleteClientGroupAction : IGenericActionWriteDbAsync<int, ClientGroup>
    {
    }

    public class DeleteClientGroupAction : BusinessActionErrors, IDeleteClientGroupAction
    {
        private readonly IClientGroupsDbAccess _dbAccess;

        public DeleteClientGroupAction(IClientGroupsDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<ClientGroup> BizActionAsync(int inputData)
        {
            var clientGroup = await _dbAccess.FindAsync(inputData);
            if (clientGroup == null)
                return ReturnError<ClientGroup>(BusinessErrors.ClientGroups.GroupNotFound);

            _dbAccess.Remove(clientGroup);
            return clientGroup;
        }
    }
}