using Orcus.Administration.Library.Menus;
using Orcus.Administration.Library.Models;
using Unclassified.TxLib;

namespace Orcus.Administration.Library.Services
{
    /// <summary>
    ///     This class simplified the process of creating a context menu item and opening the window with the view model.
    /// </summary>
    public interface IClientCommandRegistrar
    {
        /// <summary>
        ///     Register a view model as a client command meaning that it will be shown in the context menu and open on invoking.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the client command.</typeparam>
        /// <param name="txLibResource">The <see cref="Tx"/>Lib resource name of the command.</param>
        /// <param name="iconFactory">The factory that will create the icon for the context menu item and window</param>
        /// <param name="category">The category of the command.</param>
        void Register<TViewModel>(string txLibResource, IIconFactory iconFactory, CommandCategory category);
    }
}