using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Maze.Administration.Library.Models;
using Microsoft.Extensions.Logging;

namespace Maze.Administration.Library.Deployment
{
    public interface IClientDeployer
    {
        string Name { get; }
        string Description { get; }

        Task Deploy(IEnumerable<ClientGroupViewModel> groups, ILogger logger, CancellationToken cancellationToken);
    }
}