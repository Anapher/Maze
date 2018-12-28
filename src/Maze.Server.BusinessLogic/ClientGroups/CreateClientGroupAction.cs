using System.Linq;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Maze.Server.BusinessDataAccess.ClientGroups;
using Maze.Server.Connection.Clients;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.BusinessLogic.ClientGroups
{
    public interface ICreateClientGroupAction : IGenericActionWriteDbAsync<ClientGroupDto, ClientGroup>
    {
    }

    public class CreateClientGroupAction : BusinessActionErrors, ICreateClientGroupAction
    {
        private readonly IClientGroupsDbAccess _dbAccess;

        public CreateClientGroupAction(IClientGroupsDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public Task<ClientGroup> BizActionAsync(ClientGroupDto inputData)
        {
            if (ValidateModelFailed(inputData))
                return Task.FromResult<ClientGroup>(null);

            var clientGroup = new ClientGroup {Name = inputData.Name};
            if (inputData.Clients?.Count > 0)
            {
                clientGroup.ClientGroupMemberships = inputData.Clients.Select(x => new ClientGroupMembership {ClientId = x}).ToList();
            }

            _dbAccess.AddClientGroup(clientGroup);
            return Task.FromResult(clientGroup);
        }
    }
}