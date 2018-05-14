using System.Threading.Tasks;
using Orcus.Modules.Api.Response;

namespace Orcus.Modules.Api.Services
{
    public interface IObjectResultExecuter
    {
        Task ExecuteAsync(IActionContext context, ObjectResult result);
    }
}
