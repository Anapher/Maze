using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Maze.Server.BusinessDataAccess.ClientGroups;
using Maze.Server.Connection;
using Maze.Server.Connection.Clients;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.BusinessLogic.ClientGroups
{
    public interface IUpdateClientGroupAction : IGenericActionWriteDbAsync<ClientGroupDto, ClientGroup>
    {
    }

    public class UpdateClientGroupAction : BusinessActionErrors, IUpdateClientGroupAction
    {
        private readonly IClientGroupsDbAccess _dbAccess;

        public UpdateClientGroupAction(IClientGroupsDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<ClientGroup> BizActionAsync(ClientGroupDto inputData)
        {
            var group = await _dbAccess.FindAsync(inputData.ClientGroupId);
            if (group == null)
                return ReturnError<ClientGroup>(BusinessErrors.ClientGroups.GroupNotFound);

            group.Name = inputData.Name;
            return group;
        }
    }
}