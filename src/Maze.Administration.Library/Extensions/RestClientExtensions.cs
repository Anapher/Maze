using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Models;

namespace Maze.Administration.Library.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="IMazeRestClient"/>
    /// </summary>
    public static class RestClientExtensions
    {
        /// <summary>
        ///     Create a <see cref="ITargetedRestClient"/> based on a <see cref="clientId"/>
        /// </summary>
        /// <param name="mazeRestClient">The rest client that carries the core connection.</param>
        /// <param name="clientId">The id of the client that should be called by the new rest client.</param>
        /// <returns>Return the new rest client that will send all commands to the client.</returns>
        public static ITargetedRestClient CreateTargeted(this IMazeRestClient mazeRestClient, int clientId)
        {
            return new TargetedRestClient(mazeRestClient, clientId);
        }

        /// <summary>
        ///     Create a <see cref="ITargetedRestClient"/> based on a <see cref="client"/>
        /// </summary>
        /// <param name="mazeRestClient">The rest client that carries the core connection.</param>
        /// <param name="client">The client that should be called by the new rest client.</param>
        /// <returns>Return the new rest client that will send all commands to the client.</returns>
        public static ITargetedRestClient CreateTargeted(this IMazeRestClient mazeRestClient, ClientViewModel client)
        {
            return CreateTargeted(mazeRestClient, client.ClientId);
        }
    }
}