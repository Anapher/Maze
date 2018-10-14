using System;
using System.Linq;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Microsoft.EntityFrameworkCore;
using Orcus.Server.Connection;
using Orcus.Server.Data.EfClasses.Tasks;
using Orcus.Server.Data.EfCode;

namespace Orcus.Server.BusinessLogic.Tasks
{
    public interface IGetOrCreateTaskSessionAction : IGenericActionWriteDbAsync<(Guid taskId, string sessionName, string sourceTrigger), TaskSession>
    {
    }

    public class GetOrCreateTaskSessionAction : BusinessActionErrors, IGetOrCreateTaskSessionAction
    {
        private readonly AppDbContext _context;

        public GetOrCreateTaskSessionAction(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskSession> BizActionAsync((Guid taskId, string sessionName, string sourceTrigger) inputData)
        {
            var taskSession = await _context.Set<TaskSession>().Where(x => x.TaskReference.TaskId == inputData.taskId && x.Name == inputData.sessionName)
                .FirstOrDefaultAsync();
            if (taskSession == null)
            {
                var task = await _context.Set<TaskReference>().FirstOrDefaultAsync(x => x.TaskId == inputData.taskId);
                if (task == null)
                    return ReturnError<TaskSession>(BusinessErrors.Tasks.TaskNotFound);

                taskSession = new TaskSession {TaskReference = task, Name = inputData.sessionName, SourceTrigger = inputData.sourceTrigger};
                _context.Add(taskSession);
            }

            return taskSession;
        }
    }
}
