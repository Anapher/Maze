using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Server.Core;

namespace Tasks.Infrastructure.Server.Business
{
    public interface IGetTaskSyncInfo : IGenericActionOutOnlyAsync<List<TaskSyncDto>>
    {
    }

    public class GetTaskSyncInfo : BusinessActionErrors, IGetTaskSyncInfo
    {
        private readonly OrcusTaskManager _orcusTaskManager;

        public GetTaskSyncInfo(OrcusTaskManager orcusTaskManager)
        {
            _orcusTaskManager = orcusTaskManager;
        }

        public Task<List<TaskSyncDto>> BizActionAsync()
        {
            var tasks = _orcusTaskManager.ClientTasks;
            var result = tasks.Select(x => new TaskSyncDto {TaskId = x.OrcusTask.Id, Hash = x.Hash.ToString()}).ToList();
            return Task.FromResult(result);
        }
    }
}