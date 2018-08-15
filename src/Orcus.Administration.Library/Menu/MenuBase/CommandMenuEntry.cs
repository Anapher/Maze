using System.Collections.Generic;
using Prism.Commands;

namespace Orcus.Administration.Library.Menu.MenuBase
{
    public class CommandMenuEntry<TItem> : IVisibleMenuitem<TItem>
    {
        public DelegateCommand<TItem> Command { get; set; }
        public DelegateCommand<IList<TItem>> MultipleCommand { get; set; }

        public object Header { get; set; }
        public object Icon { get; set; }
    }
}