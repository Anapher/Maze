using System.Windows;
using System.Windows.Controls;
using Autofac;
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
            Icon = VisualStudioImages.ListFolder();

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
                InitializeContextMenu(_scope, contextMenu);
                InitializeFileContextMenu(_scope, fileContextMenu);
                InitializeDirectoryContextMenu(_scope, directoryContextMenu);
            }
        }

        private void InitializeContextMenu(IComponentContext scope, ContextMenu contextMenu)
        {
            var menuInfo = scope.Resolve<FileExplorerContextMenu>();
            var factory = scope.Resolve<IMenuFactory>();

            var items = factory.Create(menuInfo, DataContext);
            foreach (var item in items)
                contextMenu.Items.Add(item);
        }

        private void InitializeFileContextMenu(IComponentContext scope, ContextMenu contextMenu)
        {
            var menuInfo = scope.Resolve<FileExplorerListFileContextMenu>();
            var factory = scope.Resolve<IItemMenuFactory>();

            var items = factory.Create(menuInfo, DataContext);
            foreach (var item in items)
                contextMenu.Items.Add(item);
        }

        private void InitializeDirectoryContextMenu(IComponentContext scope, ContextMenu contextMenu)
        {
            var menuInfo = scope.Resolve<FileExplorerListDirectoryContextMenu>();
            var factory = scope.Resolve<IItemMenuFactory>();

            var items = factory.Create(menuInfo, DataContext);
            foreach (var item in items)
                contextMenu.Items.Add(item);
        }
    }
}