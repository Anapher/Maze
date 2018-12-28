namespace Maze.Modules.Api.Routing
{
    public class MazeDeleteAttribute : MazeMethodAttribute
    {
        private const string _method = "DELETE";

        public MazeDeleteAttribute(string path) : base(_method, path)
        {
        }

        public MazeDeleteAttribute() : base(_method)
        {
        }
    }
}