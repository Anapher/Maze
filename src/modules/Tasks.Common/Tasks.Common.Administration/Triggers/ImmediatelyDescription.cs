using System;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Triggers;
using Tasks.Infrastructure.Administration.Library.Trigger;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Triggers
{
    public class ImmediatelyDescription : ITriggerDescription
    {
        private readonly VisualStudioIcons _icons;

        public ImmediatelyDescription(VisualStudioIcons icons)
        {
            _icons = icons;
        }

        public string Name { get; } = Tx.T("TasksCommon:Triggers.Immediately");
        public string Namespace { get; } = null;
        public string Summary { get; } = Tx.T("TasksCommon:Triggers.Immediately.Summary");
        public UIElement Icon => _icons.Event;
        public Type DtoType { get; } = typeof(ImmediatelyTriggerInfo);
    }
}