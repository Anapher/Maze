using Autofac;
using AutoMapper;

namespace TaskManager.Administration
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<AutoMapperProfile>().As<Profile>();
        }
    }
}