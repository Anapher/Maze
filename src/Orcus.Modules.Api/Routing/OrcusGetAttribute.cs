namespace Orcus.Modules.Api.Routing
{
    public class OrcusGetAttribute : OrcusAttribute
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