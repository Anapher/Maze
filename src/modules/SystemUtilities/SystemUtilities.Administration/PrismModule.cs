using SystemUtilities.Administration.Resources;
using Orcus.Administration.Library.Services;
using Prism.Modularity;
using Unclassified.TxLib;

namespace SystemUtilities.Administration
{
    public class PrismModule : IModule
    {
        public const string ModuleName = "SystemUtilities";

        private readonly VisualStudioIcons _icons;
        private readonly IClientCommandRegistrar _registrar;

        public PrismModule(IClientCommandRegistrar registrar, VisualStudioIcons icons)
        {
            _registrar = registrar;
            _icons = icons;
        }

        public void Initialize()
        {
            Tx.LoadFromEmbeddedResource("SystemUtilities.Administration.Resources.SystemUtilities.Translation.txd");
        }
    }
}