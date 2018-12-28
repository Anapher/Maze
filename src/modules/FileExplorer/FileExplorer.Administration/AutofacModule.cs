using Autofac;
using FileExplorer.Administration.Menus;
using FileExplorer.Administration.Resources;
using FileExplorer.Administration.Utilities;
using Maze.Administration.Library.Menu;

namespace FileExplorer.Administration
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<FileExplorerContextMenu>().SingleInstance();
            builder.RegisterType<FileExplorerContextMenuManager>().As<ContextMenuManager>().SingleInstance();

            builder.RegisterType<FileExplorerListDirectoryContextMenu>().SingleInstance();
            builder.RegisterType<ListDirectoryContextMenuManager>().As<ContextMenuManager>().SingleInstance();

            builder.RegisterType<FileExplorerListFileContextMenu>().SingleInstance();
            builder.RegisterType<ListFileContextMenuManager>().As<ContextMenuManager>().SingleInstance();

            builder.RegisterType<VisualStudioIcons>().SingleInstance();
            builder.RegisterType<ImageProvider>().As<IImageProvider>().SingleInstance();
        }
    }
}