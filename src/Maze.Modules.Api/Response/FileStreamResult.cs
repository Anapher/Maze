using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Maze.Modules.Api.Extensions;
using Maze.Modules.Api.Services;

namespace Maze.Modules.Api.Response
{
    /// <summary>
    ///     Represents an <see cref="T:Microsoft.AspNetCore.Mvc.ActionResult" /> that when executed will
    ///     write a file from a stream to the response.
    /// </summary>
    public class FileStreamResult : FileResult
    {
        private Stream _fileStream;

        /// <summary>
        ///     Creates a new <see cref="T:Microsoft.AspNetCore.Mvc.FileStreamResult" /> instance with
        ///     the provided <paramref name="fileStream" /> and the
        ///     provided <paramref name="contentType" />.
        /// </summary>
        /// <param name="fileStream">The stream with the file.</param>
        /// <param name="contentType">The Content-Type header of the response.</param>
        public FileStreamResult(Stream fileStream, string contentType)
            : this(fileStream, MediaTypeHeaderValue.Parse(contentType))
        {
        }

        /// <summary>
        ///     Creates a new <see cref="T:Microsoft.AspNetCore.Mvc.FileStreamResult" /> instance with
        ///     the provided <paramref name="fileStream" /> and the
        ///     provided <paramref name="contentType" />.
        /// </summary>
        /// <param name="fileStream">The stream with the file.</param>
        /// <param name="contentType">The Content-Type header of the response.</param>
        public FileStreamResult(Stream fileStream, MediaTypeHeaderValue contentType)
            : base(contentType?.ToString())
        {
            FileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
        }

        /// <summary>
        ///     Gets or sets the stream with the file that will be sent back as the response.
        /// </summary>
        public Stream FileStream
        {
            get => _fileStream;
            set => _fileStream = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var executor = context.Context.RequestServices
                .GetRequiredService<IActionResultExecutor<FileStreamResult>>();
            return executor.ExecuteAsync(context, this);
        }
    }
}