using System;
using System.Threading;
using Orcus.Server.Connection;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Server
{
    public class TaskInfo : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public TaskInfo(OrcusTask orcusTask, Hash hash, bool executeOnServer, bool executeOnClients)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Token = _cancellationTokenSource.Token;

            OrcusTask = orcusTask;
            Hash = hash;
            ExecuteOnServer = executeOnServer;
            ExecuteOnClients = executeOnClients;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }

        public OrcusTask OrcusTask { get; }
        public CancellationToken Token { get; }
        public Hash Hash { get; }

        public bool ExecuteOnServer { get; }
        public bool ExecuteOnClients { get; }
    }
}