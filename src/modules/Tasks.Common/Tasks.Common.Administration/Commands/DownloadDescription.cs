using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library.Command;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Commands
{
    public class DownloadDescription : ICommandDescription
    {
        public DownloadDescription(VisualStudioIcons icons)
        {

        }

        public string Name => Tx.T("TasksCommon:Commands.Download");

        public string Summary => Tx.T("TasksCommon:Commands.Download.Summary");

        public UIElement Icon => throw new NotImplementedException();

        public Type DtoType => typeof(DownloadCommandInfo);
    }
}
