using RemoteDesktop.Administration.Resources;
using RemoteDesktop.Administration.ViewModels;
using Orcus.Administration.Library.Menus;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Services;
using Prism.Modularity;
using Unclassified.TxLib;

namespace RemoteDesktop.Administration
{
    public class PrismModule : IModule
    {
        private readonly VisualStudioIcons _icons;
        private readonly IClientCommandRegistrar _registrar;

        public PrismModule(IClientCommandRegistrar registrar, VisualStudioIcons icons)
        {
            _registrar = registrar;
            _icons = icons;
        }

        public void Initialize()
        {
            Tx.LoadFromEmbeddedResource("RemoteDesktop.Administration.Resources.Module.Translation.txd");

            _registrar.Register<ModuleViewModel>("Module:Name", IconFactory.FromFactory(() => _icons.Icon), CommandCategory.System);
        }
    }
}