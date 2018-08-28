using System.Windows.Input;

namespace Orcus.Administration.Library.Menu.Internal
{
    internal interface IContextAwareCommand : ICommand
    {
        object Context { get; set; }
    }
}