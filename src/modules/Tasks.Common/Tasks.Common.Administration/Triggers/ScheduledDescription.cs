using System;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Triggers;
using Tasks.Infrastructure.Administration.Library.Trigger;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Triggers
{
    public class ScheduledDescription : ITriggerDescription
    {
        private readonly VisualStudioIcons _icons;

        public ScheduledDescription(VisualStudioIcons icons)
        {
            _icons = icons;
        }

        public UIElement Icon => _icons.HistoryTable;

        public string Name { get; } = Tx.T("TasksCommon:Triggers.Scheduled");
        public string Summary { get; } = Tx.T("TasksCommon:Triggers.Scheduled.Summary");
        public Type DtoType { get; } = typeof(ScheduledTriggerInfo);
    }
}