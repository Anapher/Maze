using Autofac;
using ClipboardManager.Administration.Utilities;
using ClipboardManager.Shared.Utilities;

namespace ClipboardManager.Administration
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ClipboardWatcher>().SingleInstance();
            builder.RegisterType<ClipboardSynchronizer>();
        }
    }
}