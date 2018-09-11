using Orcus.Administration.Library.Menus;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Services;
using Prism.Modularity;
using TaskManager.Administration.Resources;
using TaskManager.Administration.ViewModels;
using Unclassified.TxLib;

namespace TaskManager.Administration
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
            Tx.LoadFromEmbeddedResource("TaskManager.Administration.Resources.TaskManager.Translation.txd");

            _registrar.Register<TaskManagerViewModel>("TaskManager:TaskManager", IconFactory.FromFactory(() => _icons.Process),
                CommandCategory.System);
        }
    }
}