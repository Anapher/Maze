namespace Maze.Modules.Api.Routing
{
    public class MazePatchAttribute : MazeMethodAttribute
    {
        private const string _method = "PATCH";

        public MazePatchAttribute(string path) : base(_method, path)
        {
        }

        public MazePatchAttribute() : base(_method)
        {
        }
    }
}