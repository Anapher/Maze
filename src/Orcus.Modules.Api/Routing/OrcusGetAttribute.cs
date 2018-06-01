namespace Orcus.Modules.Api.Routing
{
    public class OrcusGetAttribute : OrcusMethodAttribute
    {
        private const string _method = "GET";

        public OrcusGetAttribute(string path) : base(_method, path)
        {
        }

        public OrcusGetAttribute() : base(_method)
        {
        }
    }
}