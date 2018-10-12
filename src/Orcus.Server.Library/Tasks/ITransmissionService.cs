using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Server.Connection.Tasks.Transmission;
using Orcus.Server.Data.EfClasses.Tasks;

namespace Orcus.Server.Library.Tasks
{
    public interface ITransmissionService<in TTransmissionInfo> where TTransmissionInfo : TransmissionInfo
    {
        Task InvokeAsync(TTransmissionInfo transmissionInfo, TransmissionContext context, CancellationToken cancellationToken);
    }

    public abstract class TransmissionContext
    {
        public abstract Task<TaskTransmissionSession> GetSession(string name);
        public abstract Task<bool> IsClientIncluded(int clientId);
        public abstract bool IsServerIncluded();
    }
    
    public abstract class TaskTransmissionSession
    {
        public abstract Task InvokeAll();
        public abstract Task InvokeClient(int clientId);
        public abstract Task InvokeServer();

        public abstract TaskSession Info { get; }
    }
}
