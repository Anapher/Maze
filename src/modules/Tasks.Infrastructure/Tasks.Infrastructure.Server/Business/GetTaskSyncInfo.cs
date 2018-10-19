using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Server.Business
{
    public interface IGetTaskSyncInfo : IGenericActionOutOnlyAsync<List<TaskSyncDto>>
    {
    }

    public class GetTaskSyncInfo : BusinessActionErrors, IGetTaskSyncInfo
    {
        public GetTaskSyncInfo()
        {

        }

        public Task<List<TaskSyncDto>> BizActionAsync()
        {
            throw new NotImplementedException();
        }
    }
}
