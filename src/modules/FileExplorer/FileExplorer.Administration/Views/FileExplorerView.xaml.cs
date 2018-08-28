using System.Windows;
using System.Windows.Controls;
using Autofac;
using FileExplorer.Administration.Menus;
using FileExplorer.Administration.Resources;
using Orcus.Administration.Library.Menu;
using Orcus.Administration.Library.Views;

namespace FileExplorer.Administration.Views
{
    /// <summary>
    ///     Interaction logic for FileExplorerView.xaml
    /// </summary>
    public partial class FileExplorerView
    {
        private readonly IComponentContext _scope;

        public FileExplorerView(IWindowViewManager viewManager, IComponentContext scope) : base(viewManager)
        {
            InitializeComponent();
            Icon = scope.Resolve<VisualStudioIcons>().ListFolder;

            _scope = scope;
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var contextMenu = (ContextMenu) EntriesView.FindResource("DefaultContextMenu");
            var fileContextMenu = (ContextMenu) EntriesView.FindResource("FileContextMenu");
            var directoryContextMenu = (ContextMenu) EntriesView.FindResource("DirectoryContextMenu");

            if (e.OldValue != null)
            {
                contextMenu.Items.Clear();
                fileContextMenu.Items.Clear();
                directoryContextMenu.Items.Clear();
            }

            if (e.NewValue != null)
            {
                InitializeContextMenu(contextMenu, _scope.Resolve<FileExplorerContextMenuManager>());
                InitializeContextMenu(fileContextMenu, _scope.Resolve<ListFileContextMenuManager>());
                InitializeContextMenu(directoryContextMenu, _scope.Resolve<ListDirectoryContextMenuManager>());
            }
        }

        private void InitializeContextMenu(ContextMenu contextMenu, ContextMenuManager manager)
        {
            manager.Fill(contextMenu, DataContext);
        }
    }
}