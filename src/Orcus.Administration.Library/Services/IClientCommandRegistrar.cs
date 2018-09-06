using Orcus.Administration.Library.Menus;
using Orcus.Administration.Library.Models;

namespace Orcus.Administration.Library.Services
{
    public interface IClientCommandRegistrar
    {
        void Register<TViewModel>(string txLibResource, IIconFactory iconFactory, CommandCategory category);
    }
}