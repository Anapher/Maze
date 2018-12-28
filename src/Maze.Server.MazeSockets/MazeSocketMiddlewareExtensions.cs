using Microsoft.AspNetCore.Builder;

namespace Maze.Server.MazeSockets
{
    /// <summary>
    ///     Extensions of the <see cref="IApplicationBuilder" /> to enqueue the <see cref="MazeSocketMiddleware" />
    /// </summary>
    public static class MazeSocketMiddlewareExtensions
    {
        /// <summary>
        ///     Enqueue the <see cref="MazeSocketMiddleware" /> and provide the <see cref="IMazeSocketFeature" /> in the context
        ///     if possible
        /// </summary>
        /// <param name="builder">The application builder of your server</param>
        public static IApplicationBuilder UseMazeSockets(this IApplicationBuilder builder) =>
            builder.UseMiddleware<MazeSocketMiddleware>();
    }
}