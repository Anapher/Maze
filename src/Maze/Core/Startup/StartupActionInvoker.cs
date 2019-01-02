using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Maze.Client.Library.Interfaces;
using Maze.Core.Modules;
using Maze.ModuleManagement;
using Microsoft.Extensions.Configuration;

namespace Maze.Core.Startup
{
    public interface IStartupActionInvoker
    {
        Task Load(CancellationToken cancellationToken);
    }

    public class StartupActionInvoker : IStartupActionInvoker
    {
        private readonly IEnumerable<IStartupAction> _actions;
        private readonly IConfiguration _configuration;
        private readonly IModulesCatalog _modulesCatalog;

        public StartupActionInvoker(IConfigurationRootProvider configuration, IEnumerable<IStartupAction> actions, IModulesCatalog modulesCatalog)
        {
            _configuration = configuration.ConfigurationRoot;
            _actions = actions;
            _modulesCatalog = modulesCatalog;
        }

        public Task Load(CancellationToken cancellationToken)
        {
            var moduleNames = new HashSet<string>(_configuration.GetSection("Modules:LoadOnStartup").GetChildren().Select(x => x.Key),
                StringComparer.OrdinalIgnoreCase);

            var modules = _modulesCatalog.Packages.Where(x => moduleNames.Contains(x.Context.Package.Id)).ToList();

            var invoker = new ActionInterfaceInvoker<IStartupAction>(_actions.Select(x => (x, x.GetType()))
                .Where(x => modules.Any(y => x.Item2.Assembly.FullName == y.Assembly.FullName)).Select(x => x.x).ToList());
            return invoker.Invoke(cancellationToken);
        }
    }
}