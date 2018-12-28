namespace Maze.Modules.Api.Routing
{
    public class MazePutAttribute : MazeMethodAttribute
    {
        private const string _method = "PUT";

        public MazePutAttribute(string path) : base(_method, path)
        {
        }

        public MazePutAttribute() : base(_method)
        {
        }
    }
}