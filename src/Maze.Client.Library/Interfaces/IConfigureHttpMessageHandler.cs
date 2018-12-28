using System.Net.Http;

namespace Maze.Client.Library.Interfaces
{
    /// <summary>
    ///     An action that is invoked to configure the <see cref="HttpMessageHandler"/> to connect to the server
    /// </summary>
    public interface IConfigureHttpMessageHandler
    {
        /// <summary>
        ///     Change settings of the <see cref="handler"/>
        /// </summary>
        /// <param name="handler">The current <see cref="HttpMessageHandler"/>.</param>
        /// <returns>Return the <see cref="HttpMessageHandler"/> that should be used.</returns>
        HttpMessageHandler Configure(HttpMessageHandler handler);
    }
}