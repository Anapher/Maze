using SystemInformation.Administration.Resources;
using SystemInformation.Administration.ViewModels;
using Orcus.Administration.Library.Menus;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Services;
using Prism.Modularity;
using Unclassified.TxLib;

namespace SystemInformation.Administration
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
            Tx.LoadFromEmbeddedResource("SystemInformation.Administration.Resources.SystemInformation.Translation.txd");

            _registrar.Register<SystemInformationViewModel>("SystemInformation:SystemInformation", IconFactory.FromFactory(() => _icons.SystemInfo),
                CommandCategory.System);
        }
    }
}