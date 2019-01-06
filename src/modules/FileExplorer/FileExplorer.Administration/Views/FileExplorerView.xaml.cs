using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FileExplorer.Administration.Menus;
using Maze.Administration.Library.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace FileExplorer.Administration.Views
{
    /// <summary>
    ///     Interaction logic for FileExplorerView.xaml
    /// </summary>
    public partial class FileExplorerView
    {
        private readonly IServiceProvider _scope;

        public FileExplorerView(IServiceProvider scope)
        {
            InitializeComponent();

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
                ContextMenuExtensions.SetSelectedItems(contextMenu, EntriesView.EntriesGrid.SelectedItems);
                ContextMenuExtensions.SetSelectedItems(fileContextMenu, EntriesView.EntriesGrid.SelectedItems);
                ContextMenuExtensions.SetSelectedItems(directoryContextMenu, EntriesView.EntriesGrid.SelectedItems);

                InitializeContextMenu(contextMenu, _scope.GetRequiredService<FileExplorerContextMenuManager>());
                InitializeContextMenu(fileContextMenu, _scope.GetRequiredService<ListFileContextMenuManager>());
                InitializeContextMenu(directoryContextMenu, _scope.GetRequiredService<ListDirectoryContextMenuManager>());
                ////InitializeStatusBar();
            }
        }

        private void InitializeContextMenu(ContextMenu contextMenu, ContextMenuManager manager)
        {
            manager.Fill(contextMenu, DataContext);
        }

        private void InitializeStatusBar()
        {
            var textBlock = new TextBlock
            {
                DataContext = DataContext,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            textBlock.SetBinding(TextBlock.TextProperty,
                new Binding("EntriesViewModel.View.Count") {Mode = BindingMode.OneWay});

            ViewManager.RightStatusBarContent = textBlock;
        }
    }
}