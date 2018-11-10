using System;
using System.Linq;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Microsoft.EntityFrameworkCore;
using Orcus.Server.Connection;
using Orcus.Server.Data.EfCode;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Server.Business
{
    public interface IGetOrCreateTaskSessionAction : IGenericActionWriteDbAsync<(Guid taskId, string sessionKey, string description), TaskSession>
    {
    }

    public class GetOrCreateTaskSessionAction : BusinessActionErrors, IGetOrCreateTaskSessionAction
    {
        private readonly AppDbContext _context;

        public GetOrCreateTaskSessionAction(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskSession> BizActionAsync((Guid taskId, string sessionKey, string description) inputData)
        {
            var taskSession = await _context.Set<TaskSession>()
                .Where(x => x.TaskReference.TaskId == inputData.taskId && x.TaskSessionHash == inputData.sessionKey)
                .FirstOrDefaultAsync();
            if (taskSession == null)
            {
                var task = await _context.Set<TaskReference>().FirstOrDefaultAsync(x => x.TaskId == inputData.taskId);
                if (task == null)
                    return ReturnError<TaskSession>(BusinessErrors.Tasks.TaskNotFound);

                taskSession = new TaskSession {TaskReference = task, TaskSessionHash = inputData.sessionKey};
                _context.Add(taskSession);
            }

            if (!string.IsNullOrWhiteSpace(inputData.description))
                taskSession.Description = inputData.description;

            return taskSession;
        }
    }
}
