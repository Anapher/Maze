using Orcus.Administration.Library.Menus;

namespace Orcus.Administration.Library.Services
{
    public interface IClientCommandRegistrar
    {
        void Register<TViewModel>(string txLibResource, object icon, CommandCategory category);
    }
}