namespace Orcus.Modules.Api.Routing
{
    public class OrcusPutAttribute : OrcusMethodAttribute
    {
        private const string _method = "PUT";

        public OrcusPutAttribute(string path) : base(_method, path)
        {
        }

        public OrcusPutAttribute() : base(_method)
        {
        }
    }
}