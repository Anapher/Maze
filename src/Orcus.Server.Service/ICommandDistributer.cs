using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Modules.Api.Request;
using Orcus.Server.Connection.Commanding;

namespace Orcus.Server.Service
{
    public interface ICommandDistributer
    {
        Task Execute(OrcusRequest request, CommandTargetCollection targets, CommandExecutionPolicy executionPolicy);
        Task<HttpResponseMessage> Execute(HttpRequestMessage request, int clientId, int accountId, CancellationToken cancellationToken);
    }
}