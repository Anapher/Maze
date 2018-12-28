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
        private readonly IMazeTaskManager _mazeTaskManager;
        private readonly IServiceProvider _services;

        public GetTaskSyncInfo(IMazeTaskManager mazeTaskManager, IServiceProvider services)
        {
            _mazeTaskManager = mazeTaskManager;
            _services = services;
        }

        public async Task<List<TaskSyncDto>> BizActionAsync(int inputData)
        {
            var tasks = new List<TaskSyncDto>();

            foreach (var taskInfo in _mazeTaskManager.ClientTasks)
            {
                var filters = new AggregatedClientFilter(_services.GetRequiredService<ILogger<AggregatedClientFilter>>());

                filters.Add(new AudienceFilter(taskInfo.MazeTask.Audience));
                foreach (var filterInfo in taskInfo.MazeTask.Filters)
                {
                    var filterType = typeof(IFilterService<>).MakeGenericType(filterInfo.GetType());
                    filters.Add(new CustomFilterFactory(filterType, filterInfo, _services.GetRequiredService<ILogger<CustomFilterFactory>>()));
                }

                if (!await filters.IsClientIncluded(inputData, _services))
                    continue;

                tasks.Add(new TaskSyncDto { TaskId = taskInfo.MazeTask.Id, Hash = taskInfo.Hash.ToString()});
            }

            return tasks;

        }
    }
}