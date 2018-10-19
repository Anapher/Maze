using System;

namespace Tasks.Infrastructure.Core
{
    public class TaskParsingException : Exception
    {
        public TaskParsingException(string message) : base(message)
        {
        }
    }
}