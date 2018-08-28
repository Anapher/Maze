using Autofac;
using UserInteraction.Administration.Resources;

namespace UserInteraction.Administration
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<VisualStudioIcons>().SingleInstance();
        }
    }
}