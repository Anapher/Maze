using System;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library.Command;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Commands
{
    public class DownloadDescription : ICommandDescription
    {
        private readonly VisualStudioIcons _icons;

        public DownloadDescription(VisualStudioIcons icons)
        {
            _icons = icons;
        }

        public string Name { get; } = Tx.T("TasksCommon:Commands.Download");
        public string Summary { get; } = Tx.T("TasksCommon:Commands.Download.Summary");
        public UIElement Icon => _icons.DownloadFile;
        public Type DtoType { get; } = typeof(DownloadCommandInfo);
    }
}