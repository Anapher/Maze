using Console.Administration.Resources;
using Console.Administration.ViewModels;
using Orcus.Administration.Library.Menus;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Services;
using Prism.Modularity;
using Unclassified.TxLib;

namespace Console.Administration
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
            Tx.LoadFromEmbeddedResource("Console.Administration.Resources.Console.Translation.txd");

            _registrar.Register<ConsoleViewModel>("Console:Name", IconFactory.FromFactory(() => _icons.Icon), CommandCategory.System);
        }
    }
}