using Orcus.Administration.Library.Services;
using Prism.Modularity;
using Unclassified.TxLib;
using UserInteraction.Administration.Views;

namespace UserInteraction.Administration
{
    public class PrismModule : IModule
    {
        private readonly IClientCommandRegistrar _registrar;

        public PrismModule(IClientCommandRegistrar registrar)
        {
            _registrar = registrar;
        }

        public void Initialize()
        {
            Tx.LoadFromEmbeddedResource("UserInteraction.Administration.Resources.UserInteraction.Translation.txd");

            _registrar.RegisterView(typeof(MessageBoxView), "UserInteraction:MessageBox", null, CommandCategory.Interaction);
        }
    }
}