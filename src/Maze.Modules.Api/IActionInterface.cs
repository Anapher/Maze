using System.Threading.Tasks;

namespace Maze.Modules.Api
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