using Maze.Modules.Api;

namespace Maze.Service.Commander.Routing
{
    public interface IRouteResolver
    {
        ResolveResult Resolve(MazeContext context);
    }
}