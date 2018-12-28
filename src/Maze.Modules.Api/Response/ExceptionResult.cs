using System;

namespace Maze.Modules.Api.Response
{
    public class ExceptionResult : ObjectResult
    {
        public ExceptionResult(Exception exception) : base(MazeError.FromException(exception))
        {
        }
    }
}