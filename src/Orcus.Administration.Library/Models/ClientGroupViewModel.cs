using System.Collections.Generic;
using Prism.Mvvm;

namespace Orcus.Administration.Library.Models
{
    public abstract class ClientGroupViewModel : BindableBase
    {
        public abstract int ClientGroupId { get; }
        public abstract string Name { get; set; }

        public abstract IList<ClientViewModel> Clients { get; }
    }
}