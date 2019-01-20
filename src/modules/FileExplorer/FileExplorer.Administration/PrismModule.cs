using System.Collections.Generic;
using FileExplorer.Administration.Menus;
using FileExplorer.Administration.Resources;
using FileExplorer.Administration.Utilities;
using FileExplorer.Administration.ViewModels;
using Maze.Administration.Library.Menu;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Unclassified.TxLib;
using Unity;

namespace FileExplorer.Administration
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("FileExplorer.Administration.Resources.FileExplorer.Translation.txd");

            containerRegistry.RegisterSingleton<FileExplorerContextMenu>();
            //containerRegistry.RegisterSingleton<ContextMenuManager, FileExplorerContextMenuManager>();
            containerRegistry.GetContainer().RegisterSingleton<ContextMenuManager, FileExplorerContextMenuManager>(nameof(FileExplorerContextMenuManager));

            containerRegistry.RegisterSingleton<FileExplorerListDirectoryContextMenu>();
            containerRegistry.GetContainer().RegisterSingleton<ContextMenuManager, ListDirectoryContextMenuManager>(nameof(ListDirectoryContextMenuManager));
            //containerRegistry.RegisterSingleton<ContextMenuManager, ListDirectoryContextMenuManager>();

            containerRegistry.RegisterSingleton<FileExplorerListFileContextMenu>();
            containerRegistry.RegisterSingleton<ContextMenuManager, ListFileContextMenuManager>();

            containerRegistry.RegisterSingleton<VisualStudioIcons>();
            containerRegistry.RegisterSingleton<IImageProvider, ImageProvider>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var registar = containerProvider.Resolve<IClientCommandRegistrar>();
            var icons = containerProvider.Resolve<VisualStudioIcons>();
            var builders = containerProvider.Resolve<IEnumerable<ContextMenuManager>>();

            registar.Register<FileExplorerViewModel>("FileExplorer:FileExplorer", IconFactory.FromFactory(() => icons.ListFolder),
                CommandCategory.System);

            foreach (var contextMenuBuilder in builders)
                contextMenuBuilder.Build();
        }
    }
}