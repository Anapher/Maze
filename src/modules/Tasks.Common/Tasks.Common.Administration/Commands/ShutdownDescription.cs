using System;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library.Command;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Commands
{
    public class ShutdownDescription : ICommandDescription
    {
        private readonly VisualStudioIcons _icons;

        public ShutdownDescription(VisualStudioIcons icons)
        {
            _icons = icons;
        }

        public string Name { get; } = Tx.T("TasksCommon:Commands.Shutdown");
        public string Namespace { get; } = "Computer";
        public string Summary { get; } = Tx.T("TasksCommon:Commands.Shutdown.Summary");
        public UIElement Icon => _icons.ShutDown;
        public Type DtoType { get; } = typeof(ShutdownCommandInfo);
    }
}