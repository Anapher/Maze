using System.Threading.Tasks;
using Orcus.Modules.Api.Extensions;
using Orcus.Modules.Api.Services;

namespace Orcus.Modules.Api.Response
{
    public class ObjectResult : IActionResult
    {
        public ObjectResult(object value)
        {
            Value = value;
        }

        public object Value { get; set; }
        public int? StatusCode { get; set; }

        public Task ExecuteResultAsync(IActionContext context)
        {
            return context.ServiceProvider.GetRequiredService<IObjectResultExecuter>().ExecuteAsync(context, this);
        }
    }
}