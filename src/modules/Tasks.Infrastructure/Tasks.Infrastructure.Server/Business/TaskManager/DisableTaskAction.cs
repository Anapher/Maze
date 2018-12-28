using System;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Server.BusinessDataAccess;
using Tasks.Infrastructure.Server.Core;

namespace Tasks.Infrastructure.Server.Business.TaskManager
{
    public interface IDisableTaskAction : IGenericActionInOnlyAsync<Guid>
    {
    }

    public class DisableTaskAction : BusinessActionErrors, IDisableTaskAction
    {
        private readonly ITaskReferenceDbAccess _dbAccess;
        private readonly IMazeTaskManagerManagement _management;

        public DisableTaskAction(IMazeTaskManagerManagement management, ITaskReferenceDbAccess dbAccess)
        {
            _management = management;
            _dbAccess = dbAccess;
        }

        public async Task BizActionAsync(Guid inputData)
        {
            await _dbAccess.SetTaskIsEnabled(inputData, false);
            await _management.CancelTask(inputData);
        }
    }
}