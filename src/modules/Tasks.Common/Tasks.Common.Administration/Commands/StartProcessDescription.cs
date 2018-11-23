using System;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library.Command;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Commands
{
    public class StartProcessDescription : ICommandDescription
    {
        private readonly VisualStudioIcons _icons;

        public StartProcessDescription(VisualStudioIcons icons)
        {
            _icons = icons;
        }

        public string Name => Tx.T("TasksCommon:Commands.StartProcess");
        public string Summary => Tx.T("TasksCommon:Commands.StartProcess.Summary");
        public UIElement Icon => _icons.StartupProject;
        public Type DtoType => typeof(StartProcessCommandInfo);
    }
}