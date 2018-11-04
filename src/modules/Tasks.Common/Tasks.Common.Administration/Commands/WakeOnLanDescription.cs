using System;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library.Command;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Commands
{
    public class WakeOnLanDescription : ICommandDescription
    {
        private readonly VisualStudioIcons _icons;

        public WakeOnLanDescription(VisualStudioIcons icons)
        {
            _icons = icons;
        }

        public string Name { get; } = Tx.T("TasksCommon:Commands.WakeOnLan");
        public string Summary { get; } = Tx.T("TasksCommon:Commands.WakeOnLan.Summary");
        public UIElement Icon => _icons.ServerRunTest;
        public Type DtoType { get; } = typeof(WakeOnLanCommandInfo);
    }
}