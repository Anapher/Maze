using System;
using System.Windows;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Server.Filters;
using Tasks.Infrastructure.Administration.Library.Filter;
using Unclassified.TxLib;

namespace Tasks.Common.Administration.Filters
{
    public class OperatingSystemDescription : IFilterDescription
    {
        private readonly VisualStudioIcons _icons;

        public OperatingSystemDescription(VisualStudioIcons icons)
        {
            _icons = icons;
        }

        public string Name { get; } = Tx.T("TasksCommon:Filters.OperatingSystem");
        public string Namespace { get; } = null;
        public string Summary { get; } = Tx.T("TasksCommon:Filters.OperatingSystem.Summary");
        public UIElement Icon => _icons.ComputerSystem;
        public Type DtoType { get; } = typeof(OperatingSystemFilterInfo);
    }
}