namespace Maze.Modules.Api.Routing
{
    public class MazePostAttribute : MazeMethodAttribute
    {
        private const string _method = "POST";

        public MazePostAttribute(string path) : base(_method, path)
        {
        }

        public MazePostAttribute() : base(_method)
        {
        }
    }
}