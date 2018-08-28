using System;
using System.Windows.Controls;
using Orcus.Administration.Library.Views;

namespace FileExplorer.Administration.Resources
{
    public class VisualStudioIcons : ResourceDictionaryProvider
    {
        public VisualStudioIcons() : base(
            new Uri("/FileExplorer.Administration;component/Resources/VisualStudioIcons.xaml", UriKind.Relative))
        {
        }

        public Viewbox ListFolder => GetIcon();
        public Viewbox Refresh => GetIcon();
        public Viewbox UploadFile => GetIcon();
        public Viewbox DownloadFile => GetIcon();
        public Viewbox StartupProject => GetIcon();
        public Viewbox CopyToClipboard => GetIcon();
        public Viewbox ZipFile => GetIcon();
        public Viewbox Cancel => GetIcon();
        public Viewbox Rename => GetIcon();
        public Viewbox Property => GetIcon();
    }
}