using System;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Triggers;
using Tasks.Infrastructure.Administration.Library.Trigger;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Triggers
{
    public class OnAppStartupDescription : ITriggerDescription
    {
        private readonly VisualStudioIcons _icons;

        public OnAppStartupDescription(VisualStudioIcons icons)
        {
            _icons = icons;
        }

        public string Name { get; } = Tx.T("TasksCommon:Triggers.OnAppStartup");
        public string Summary { get; } = Tx.T("TasksCommon:Triggers.OnAppStartup.Summary");
        public UIElement Icon => _icons.VBWindowsService;
        public Type DtoType { get; } = typeof(OnAppStartupTriggerInfo);
    }
}