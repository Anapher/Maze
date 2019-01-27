using System;

namespace Maze.Administration.Library.Deployment
{
    public class ClientDeploymentException : Exception
    {
        public ClientDeploymentException(string message) : base(message)
        {
        }
    }
}