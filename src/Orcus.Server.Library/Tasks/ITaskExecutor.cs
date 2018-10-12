using System;
using System.Net.Http;
using System.Threading.Tasks;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Connection.Tasks.Commands;

namespace Orcus.Server.Library.Tasks
{
    public interface ITaskExecutor<in TCommandInfo> where TCommandInfo : CommandInfo
    {
        Task<HttpResponseMessage> InvokeAsync(TCommandInfo commandInfo, TargetId target, TaskExecutionContext context);
    }

    public abstract class TaskExecutionContext
    {
        public abstract TaskSession Session { get; }
        public abstract OrcusTask OrcusTask { get; }
    }

    public struct TargetId
    {
        private readonly int _clientId;

        public TargetId(int clientId)
        {
            _clientId = clientId;
            IsServer = false;
        }

        private TargetId(bool isServer)
        {
            IsServer = isServer;
            _clientId = 0;
        }

        public static TargetId ServerId => new TargetId(true);

        public bool IsServer { get; }

        public int ClientId
        {
            get
            {
                if (IsServer)
                    throw new InvalidOperationException("Target is server and has not client id");
                return _clientId;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is TargetId id &&
                   IsServer == id.IsServer &&
                   ClientId == id.ClientId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsServer, ClientId);
        }
    }
}