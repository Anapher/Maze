using Autofac;
using FileExplorer.Client.FileProperties;

namespace FileExplorer.Client
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IFilePropertyValueProvider>()
                .AsImplementedInterfaces();
        }
    }
}