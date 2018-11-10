using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Orcus.Server.Library.Services;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management.Channels;

namespace Tasks.Infrastructure.Server.Core
{
    public class ConnectedClientInfo
    {
        private ITaskManagerChannel _channel;

        public ConnectedClientInfo(IClientConnection connection)
        {
            Connection = connection;
            CommandProcesses = new ConcurrentDictionary<Guid, CommandProcessDto>();
        }

        public ConcurrentDictionary<Guid, CommandProcessDto> CommandProcesses { get; }
        public IClientConnection Connection { get; }

        public async Task<ITaskManagerChannel> GetChannel()
        {
        }
    }
}