using System;

namespace Orcus.Server.Connection.Tasks
{
    public class TaskParsingException : Exception
    {
        public TaskParsingException(string message) : base(message)
        {
        }
    }
}