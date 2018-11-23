using System;
using System.Windows.Controls;
using Orcus.Administration.Library.Views;

namespace Tasks.Common.Administration.Resources
{
    public class VisualStudioIcons : ResourceDictionaryProvider
    {
        public VisualStudioIcons() : base(new Uri("/Tasks.Common.Administration;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox ServerRunTest => GetIcon();
        public Viewbox ShutDown => GetIcon();
        public Viewbox Event => GetIcon();
        public Viewbox HistoryTable => GetIcon();
        public Viewbox ComputerSystem => GetIcon();
        public Viewbox Timer => GetIcon();
        public Viewbox Calendar => GetIcon();
        public Viewbox CloudConnectedServices => GetIcon();
        public Viewbox CloudRun => GetIcon();
        public Viewbox DownloadFile => GetIcon();
        public Viewbox StartupProject => GetIcon();
    }
}