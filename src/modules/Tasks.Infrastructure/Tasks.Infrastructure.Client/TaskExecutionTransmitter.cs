using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orcus.Client.Library.Services;
using Tasks.Infrastructure.Core.Data;

namespace Tasks.Infrastructure.Client
{
    public interface ITaskExecutionTransmitter
    {
        void EnqueueExecution(TaskExecutionReference taskExecution);
    }

    public class TaskExecutionTransmitter : ITaskExecutionTransmitter
    {
        public ConcurrentStack<TaskExecutionReference> WorkerItems { get; private set; }

        public void Initialize(IEnumerable<TaskExecutionReference> outstandingTasks)
        {
            WorkerItems = new ConcurrentStack<TaskExecutionReference>(outstandingTasks);
        }

        public void EnqueueExecution(TaskExecutionReference taskExecution)
        {
            WorkerItems.Push(taskExecution);
        }
    }
    
    public struct TaskExecutionReference
    {
        public TaskExecutionReference(string databaseFilename, int taskExecutionId, TaskExecution taskExecution)
        {
            DatabaseFilename = databaseFilename;
            TaskExecutionId = taskExecutionId;
            TaskExecution = taskExecution;
        }

        public string DatabaseFilename { get; }
        public int TaskExecutionId { get; }
        public TaskExecution TaskExecution { get; }
    }
}
