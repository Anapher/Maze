using System.Threading.Tasks;
using Orcus.Modules.Api.Response;

namespace Orcus.Modules.Api.Services
{
    public interface IFileStreamResultExecutor
    {
        Task ExecuteAsync(IActionContext context, FileStreamResult result);
    }
}