using Orcus.Modules.Api;

namespace Orcus.Server.Service.Commanding
{
    public class ExecutionContext
    {
        public OrcusController Controller { get; set; }
        public ActionInvoker ActionInvoker { get; set; }
        public object[] Arguments { get; set; }
    }
}