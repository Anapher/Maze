using System;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Triggers;
using Tasks.Infrastructure.Administration.Library.Trigger;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Triggers
{
    public class OnJoinDescription : ITriggerDescription
    {
        private readonly VisualStudioIcons _icons;

        public OnJoinDescription(VisualStudioIcons icons)
        {
            _icons = icons;
        }

        public string Name { get; } = Tx.T("TasksCommon:Triggers.OnJoin");
        public string Namespace { get; } = null;
        public string Summary { get; } = Tx.T("TasksCommon:Triggers.OnJoin.Summary");
        public UIElement Icon => _icons.CloudConnectedServices;
        public Type DtoType { get; } = typeof(OnJoinTriggerInfo);
    }
}