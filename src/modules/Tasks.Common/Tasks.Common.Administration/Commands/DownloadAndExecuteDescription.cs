using System;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library.Command;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Commands
{
    public class DownloadAndExecuteDescription : ICommandDescription
    {
        private readonly VisualStudioIcons _icons;

        public DownloadAndExecuteDescription(VisualStudioIcons icons)
        {
            _icons = icons;
        }

        public string Name { get; } = Tx.T("TasksCommon:Commands.DownloadAndExecute");
        public string Summary { get; } = Tx.T("TasksCommon:Commands.DownloadAndExecute.Summary");
        public UIElement Icon => _icons.CloudRun;
        public Type DtoType { get; } = typeof(DownloadAndExecuteCommandInfo);
    }
}