using System;

namespace Maze.Modules.Api.Routing
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MazeMethodAttribute : Attribute, IRouteFragment, IActionMethodProvider
    {
        protected MazeMethodAttribute(string method) : this(method, null)
        {
        }

        protected MazeMethodAttribute(string method, string path)
        {
            Method = method;
            Path = path;
        }

        public string Method { get; }
        public string Path { get; }
    }
}