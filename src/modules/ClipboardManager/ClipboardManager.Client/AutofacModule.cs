using Autofac;
using ClipboardManager.Client.Utilities;

namespace ClipboardManager.Client
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ClipboardWatcher>().SingleInstance();
        }
    }
}