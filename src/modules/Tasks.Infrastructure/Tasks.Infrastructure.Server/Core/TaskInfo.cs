using System;
using System.Threading;
using Maze.Server.Connection;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Server.Core
{
    public class TaskInfo : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public TaskInfo(MazeTask mazeTask, Hash hash, bool executeOnServer, bool executeOnClients)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Token = _cancellationTokenSource.Token;

            MazeTask = mazeTask;
            Hash = hash;
            ExecuteOnServer = executeOnServer;
            ExecuteOnClients = executeOnClients;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }

        /// <summary>
        ///     The maze task
        /// </summary>
        public MazeTask MazeTask { get; }

        /// <summary>
        ///     The cancellation token that will cancel the local execution and transmission
        /// </summary>
        public CancellationToken Token { get; }
        public Hash Hash { get; }

        public bool ExecuteOnServer { get; }
        public bool ExecuteOnClients { get; }
        public DateTimeOffset? NextExecution { get; set; }
    }
}