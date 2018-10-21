using System.Threading.Tasks;
using Orcus.Client.Library.Interfaces;
using Orcus.Client.Library.Services;
using Tasks.Infrastructure.Client.Rest;

namespace Tasks.Infrastructure.Client.Hooks
{
    public class OnConnectedAction : IConnectedAction
    {
        private readonly IOrcusRestClient _restClient;

        public OnConnectedAction(IOrcusRestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task Execute()
        {
            var tasks = await TasksResource.GetSyncInfo(_restClient);
        }
    }
}
