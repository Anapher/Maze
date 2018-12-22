using Nito.AsyncEx;
using Orcus.Server.Library.Interfaces;
using Orcus.Server.Library.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Triggers;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Common.Server.Triggers
{
    public interface IClientEventNotifier
    {
        event EventHandler<IClientConnection> ClientConnected;
    }

    public class ClientEventNotifier : IClientEventNotifier
    {
        public event EventHandler<IClientConnection> ClientConnected;

        public void OnClientConnected(IClientConnection clientConnection)
        {
            ClientConnected?.Invoke(this, clientConnection);
        }
    }

    public class ClientConnectedEvent : IClientConnectedAction
    {
        private readonly ClientEventNotifier _clientEventNotifier;

        public ClientConnectedEvent(ClientEventNotifier clientEventNotifier)
        {
            _clientEventNotifier = clientEventNotifier;
        }

        public Task Execute(IClientConnection context)
        {
            _clientEventNotifier.OnClientConnected(context);
            return Task.CompletedTask;
        }
    }

    public class OnJoinTriggerService : ITriggerService<OnJoinTriggerInfo>, IDisposable
    {
        private readonly IClientEventNotifier _clientEvents;
        private readonly AsyncLock _clientConnectedLock;
        private TriggerContext _context;
        private readonly AsyncLazy<TaskSessionTrigger> _sessionTrigger;

        public OnJoinTriggerService(IClientEventNotifier clientEvents)
        {
            _clientEvents = clientEvents;
            _clientConnectedLock = new AsyncLock();
            _sessionTrigger = new AsyncLazy<TaskSessionTrigger>(() => _context.CreateSession(SessionKey.Create("OnClientJoin")));
        }

        public void Dispose()
        {
            _clientEvents.ClientConnected -= OnClientConnected;
        }

        public Task InvokeAsync(OnJoinTriggerInfo triggerInfo, TriggerContext context, CancellationToken cancellationToken)
        {
            _context = context;

            _clientEvents.ClientConnected += OnClientConnected;
            return Task.Delay(TimeSpan.MaxValue, cancellationToken);
        }

        private async void OnClientConnected(object sender, IClientConnection e)
        {
            using (await _clientConnectedLock.LockAsync())
            {
                await (await _sessionTrigger).InvokeClient(e.ClientId);
            }
        }
    }
}
