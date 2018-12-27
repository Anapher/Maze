namespace Orcus.Modules.Api.Routing
{
    public class OrcusDeleteAttribute : OrcusMethodAttribute
    {
        private const string _method = "DELETE";

        public OrcusDeleteAttribute(string path) : base(_method, path)
        {
        }

        public OrcusDeleteAttribute() : base(_method)
        {
        }
    }
}