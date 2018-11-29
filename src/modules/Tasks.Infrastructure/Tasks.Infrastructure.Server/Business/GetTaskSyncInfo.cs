using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Server.Core;
using Tasks.Infrastructure.Server.Filter;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server.Business
{
    public interface IGetTaskSyncInfo : IGenericActionAsync<int ,List<TaskSyncDto>>
    {
    }

    public class GetTaskSyncInfo : BusinessActionErrors, IGetTaskSyncInfo
    {
        private readonly IOrcusTaskManager _orcusTaskManager;
        private readonly IServiceProvider _services;

        public GetTaskSyncInfo(IOrcusTaskManager orcusTaskManager, IServiceProvider services)
        {
            _orcusTaskManager = orcusTaskManager;
            _services = services;
        }

        public async Task<List<TaskSyncDto>> BizActionAsync(int inputData)
        {
            var tasks = new List<TaskSyncDto>();

            foreach (var taskInfo in _orcusTaskManager.ClientTasks)
            {
                var filters = new AggregatedClientFilter(_services.GetRequiredService<ILogger<AggregatedClientFilter>>());

                filters.Add(new AudienceFilter(taskInfo.OrcusTask.Audience));
                foreach (var filterInfo in taskInfo.OrcusTask.Filters)
                {
                    var filterType = typeof(IFilterService<>).MakeGenericType(filterInfo.GetType());
                    filters.Add(new CustomFilterFactory(filterType, filterInfo, _services.GetRequiredService<ILogger<CustomFilterFactory>>()));
                }

                if (!await filters.IsClientIncluded(inputData, _services))
                    continue;

                tasks.Add(new TaskSyncDto { TaskId = taskInfo.OrcusTask.Id, Hash = taskInfo.Hash.ToString() });
            }

            return tasks;

        }
    }
}