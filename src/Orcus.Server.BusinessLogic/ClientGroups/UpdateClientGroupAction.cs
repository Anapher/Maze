using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Orcus.Server.BusinessDataAccess.ClientGroups;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Clients;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.BusinessLogic.ClientGroups
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