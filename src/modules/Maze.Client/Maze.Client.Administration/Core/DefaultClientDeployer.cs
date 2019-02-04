using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Anapher.Wpf.Toolkit.Windows;
using Maze.Administration.Library.Deployment;
using Maze.Administration.Library.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Unclassified.TxLib;

namespace Maze.Client.Administration.Core
{
    public class DefaultClientDeployer : IClientDeployer
    {
        private readonly IWindowService _windowService;
        private readonly IMazeWixBuilder _wixBuilder;

        public DefaultClientDeployer(IWindowService windowService, IMazeWixBuilder wixBuilder)
        {
            _windowService = windowService;
            _wixBuilder = wixBuilder;
        }

        public bool ShowInstallerUi { get; set; } = true;

        public string Name { get; } = $"WiX Installer [{PrismModule.ClientVersion}]";
        public string Description { get; } = Tx.T("Maze.Client:Description");

        public Task Deploy(IEnumerable<ClientGroupViewModel> groups, ILogger logger, CancellationToken cancellationToken)
        {
            var sfd = new SaveFileDialog {Filter = Tx.T("Maze.Client:SaveDialogFilter"), Title = Tx.T("Maze.Client:SaveDialogTitle")};
            if (_windowService.ShowDialog(sfd) != true)
                throw new ClientDeploymentException(Tx.T("Maze.Client:Exceptions.NoLocationSelected"));

            return _wixBuilder.Build(groups, sfd.FileName, logger, cancellationToken);
        }
    }
}