using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Orcus.Server.BusinessDataAccess.ClientGroups;
using Orcus.Server.Connection;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.BusinessLogic.ClientGroups
{
    public interface IAddClientsToClientGroupAction : IGenericActionWriteDbAsync<(int, IEnumerable<int>), ClientGroup>
    {
    }

    public class AddClientsToClientGroupAction : BusinessActionErrors, IAddClientsToClientGroupAction
    {
        private readonly IClientGroupsDbAccess _dbAccess;

        public AddClientsToClientGroupAction(IClientGroupsDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<ClientGroup> BizActionAsync((int, IEnumerable<int>) inputData)
        {
            var group = await _dbAccess.FindAsync(inputData.Item1);
            if (group == null)
                return ReturnError<ClientGroup>(BusinessErrors.ClientGroups.GroupNotFound);

            foreach (var clientId in inputData.Item2)
            {
                if (group.ClientGroupMemberships.Any(x => x.ClientId == clientId))
                    continue;

                group.ClientGroupMemberships.Add(new ClientGroupMembership {ClientId = clientId});
            }

            return group;
        }
    }
}