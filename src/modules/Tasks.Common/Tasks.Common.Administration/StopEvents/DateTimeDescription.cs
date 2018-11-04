using System;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Shared.StopEvents;
using Tasks.Infrastructure.Administration.Library.StopEvent;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.StopEvents
{
    public class DateTimeDescription : IStopEventDescription
    {
        private readonly VisualStudioIcons _icons;

        public DateTimeDescription(VisualStudioIcons icons)
        {
            _icons = icons;
        }

        public string Name { get; } = Tx.T("TasksCommon:StopEvents.DateTime");
        public string Summary { get; } = Tx.T("TasksCommon:StopEvents.DateTime.Summary");
        public UIElement Icon => _icons.Calendar;
        public Type DtoType { get; } = typeof(DateTimeStopEventInfo);
    }
}