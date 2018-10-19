using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Server
{
    public class TaskShell
    {
        public TaskShell(OrcusTask orcusTask)
        {
            
        }

        public bool ExecuteOnClient { get; }
        public bool ExecuteOnServer { get; }

        public CancellationToken CancellationToken { get; }
    }
}
