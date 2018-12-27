using Orcus.Administration.Core.Rest;

namespace Orcus.Administration.Core.Modules
{
    public class AppLoadContext
    {
        public IModulesCatalog ModulesCatalog { get; set; }
        public OrcusRestClient RestClient { get; set; }
    }
}