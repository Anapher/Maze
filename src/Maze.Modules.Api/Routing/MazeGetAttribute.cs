namespace Maze.Modules.Api.Routing
{
    public class MazeGetAttribute : MazeMethodAttribute
    {
        private const string _method = "GET";

        public MazeGetAttribute(string path) : base(_method, path)
        {
        }

        public MazeGetAttribute() : base(_method)
        {
        }
    }
}