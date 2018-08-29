using Orcus.Administration.Library.Menus;
using Orcus.Administration.Library.Services;
using Prism.Modularity;
using Unclassified.TxLib;
using UserInteraction.Administration.Resources;
using UserInteraction.Administration.ViewModels;
using UserInteraction.Administration.Views;

namespace UserInteraction.Administration
{
    public class PrismModule : IModule
    {
        private readonly IClientCommandRegistrar _registrar;
        private readonly VisualStudioIcons _icons;

        public PrismModule(IClientCommandRegistrar registrar, VisualStudioIcons icons)
        {
            _registrar = registrar;
            _icons = icons;
        }

        public void Initialize()
        {
            Tx.LoadFromEmbeddedResource("UserInteraction.Administration.Resources.UserInteraction.Translation.txd");

            _registrar.Register<MessageBoxViewModel>("UserInteraction:MessageBox", _icons.MessageBox,
                CommandCategory.Interaction);
        }
    }
}