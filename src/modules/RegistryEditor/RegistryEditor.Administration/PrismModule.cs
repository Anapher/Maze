using RegistryEditor.Administration.Resources;
using RegistryEditor.Administration.ViewModels;
using Orcus.Administration.Library.Menus;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Services;
using Prism.Modularity;
using Unclassified.TxLib;

namespace RegistryEditor.Administration
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
            Tx.LoadFromEmbeddedResource("RegistryEditor.Administration.Resources.RegistryEditor.Translation.txd");

            _registrar.Register<RegistryEditorViewModel>("RegistryEditor:Name", IconFactory.FromFactory(() => _icons.Registry), CommandCategory.System);
        }
    }
}