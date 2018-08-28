using Autofac;

namespace FileExplorer.Administration
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<FileExplorerContextMenu>().SingleInstance();
            builder.RegisterType<FileExplorerListDirectoryContextMenu>().SingleInstance();
            builder.RegisterType<FileExplorerListFileContextMenu>().SingleInstance();
        }
    }
}