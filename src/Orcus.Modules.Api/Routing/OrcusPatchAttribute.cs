namespace Orcus.Modules.Api.Routing
{
    public class OrcusPatchAttribute : OrcusMethodAttribute
    {
        private const string _method = "PATCH";

        public OrcusPatchAttribute(string path) : base(_method, path)
        {
        }

        public OrcusPatchAttribute() : base(_method)
        {
        }
    }
}