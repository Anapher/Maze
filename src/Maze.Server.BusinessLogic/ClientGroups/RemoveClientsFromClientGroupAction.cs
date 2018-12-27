using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Orcus.Server.BusinessDataAccess.ClientGroups;
using Orcus.Server.Connection;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.BusinessLogic.ClientGroups
{
    public interface IRemoveClientsFromClientGroupAction : IGenericActionWriteDbAsync<(int, IEnumerable<int>), ClientGroup> { }

    public class RemoveClientsFromClientGroupAction : BusinessActionErrors, IRemoveClientsFromClientGroupAction
    {
        private readonly IClientGroupsDbAccess _dbAccess;

        public RemoveClientsFromClientGroupAction(IClientGroupsDbAccess dbAccess)
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
                var membership = group.ClientGroupMemberships.FirstOrDefault(x => x.ClientId == clientId);
                if (membership != null)
                    group.ClientGroupMemberships.Remove(membership);
            }

            return group;
        }
    }
}
