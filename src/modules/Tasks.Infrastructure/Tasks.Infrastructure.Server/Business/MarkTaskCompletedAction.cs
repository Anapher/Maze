using System;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Server.BusinessDataAccess;

namespace Tasks.Infrastructure.Server.Business
{
    public interface IMarkTaskCompletedAction : IGenericActionInOnlyAsync<Guid>
    {
    }

    public class MarkTaskCompletedAction : BusinessActionErrors, IMarkTaskCompletedAction
    {
        private readonly ITaskReferenceDbAccess _dbAccess;

        public MarkTaskCompletedAction(ITaskReferenceDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public Task BizActionAsync(Guid inputData) => _dbAccess.SetCompletionStatus(inputData, true);
    }
}