using Maze.Administration.Core.Rest;

namespace Maze.Administration.Core.Modules
{
    public class AppLoadContext
    {
        public IModulesCatalog ModulesCatalog { get; set; }
        public MazeRestClient RestClient { get; set; }
    }
}