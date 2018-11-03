using System;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Shared.StopEvents;
using Tasks.Infrastructure.Administration.Library.StopEvent;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.StopEvents
{
    public class DurationDescription : IStopEventDescription
    {
        private readonly VisualStudioIcons _icons;

        public DurationDescription(VisualStudioIcons icons)
        {
            _icons = icons;
        }

        public string Name { get; } = Tx.T("TasksCommon:StopEvents.Duration");
        public string Summary { get; } = Tx.T("TasksCommon:StopEvents.Duration.Summary");
        public UIElement Icon => _icons.Timer;
        public Type DtoType { get; } = typeof(DurationStopEventInfo);
        public string Describe(object dto) => throw new NotImplementedException();
    }
}