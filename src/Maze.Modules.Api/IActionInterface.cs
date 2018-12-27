using System.Threading.Tasks;

namespace Orcus.Modules.Api
{
    public interface IActionInterface<in TContext>
    {
        Task Execute(TContext context);
    }

    public interface IActionInterface
    {
        Task Execute();
    }
}