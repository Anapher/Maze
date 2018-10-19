using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orcus.Client.Library.Interfaces;
using Orcus.Client.Library.Services;

namespace Tasks.Infrastructure.Client
{
    public class OnConnectedAction : IConnectedAction
    {
        private readonly IOrcusRestClient _restClient;

        public OnConnectedAction(IOrcusRestClient restClient)
        {
            _restClient = restClient;
        }

        public Task Execute()
        {
            _restClient.SendMessage()
        }
    }
}
