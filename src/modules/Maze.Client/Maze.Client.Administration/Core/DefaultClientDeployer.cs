using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Anapher.Wpf.Toolkit.Extensions;
using Anapher.Wpf.Toolkit.Windows;
using Maze.Administration.Library.Deployment;
using Maze.Administration.Library.Models;
using Maze.Client.Administration.Core.Wix;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Unclassified.TxLib;

namespace Maze.Client.Administration.Core
{
    public class DefaultClientDeployer : IClientDeployer
    {
        private readonly IWindowService _windowService;
        private readonly WixTools _wixTools;

        public DefaultClientDeployer(IWindowService windowService)
        {
            _windowService = windowService;
        }

        public string Name { get; } = PrismModule.ClientVersion.ToString();
        public string Description { get; } = Tx.T("Maze.Client:Description");

        public bool ShowInstallerUi { get; set; } = true;

        public async Task Deploy(IEnumerable<ClientGroupViewModel> groups, ILogger logger)
        {
            var sfd = new SaveFileDialog {Filter = Tx.T("Maze.Client:SaveFilter"), Title = Tx.T("Maze.Client:SaveDialogTitle")};
            if (_windowService.ShowDialog(sfd) != true)
                throw new ClientDeploymentException(Tx.T("Maze.Client:Exceptions.NoLocationSelected"));


            
        }
    }
}
