namespace Orcus.Modules.Api.Routing
{
    public class OrcusPostAttribute : OrcusMethodAttribute
    {
        private const string _method = "POST";

        public OrcusPostAttribute(string path) : base(_method, path)
        {
        }

        public OrcusPostAttribute() : base(_method)
        {
        }
    }
}