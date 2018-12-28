using System;

namespace Maze.Modules.Api
{
    public class MazeError
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public static MazeError FromException(Exception exception)
        {
            return new MazeError {Message = exception.Message, StackTrace = exception.StackTrace};
        }
    }
}