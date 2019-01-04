using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.Client.Extensions;
using FileExplorer.Client.FileProperties;
using FileExplorer.Client.Utilities;
using FileExplorer.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Maze.Modules.Api;
using Maze.Modules.Api.Parameters;
using Maze.Modules.Api.Routing;
using Maze.Utilities;

namespace FileExplorer.Client.Controllers
{
    [Route("fileSystem")]
    public class FileSystemController : MazeController
    {
        [MazeGet]
        public async Task<IActionResult> QueryPathEntries([FromQuery] string path, [FromQuery] bool directoriesOnly = false)
        {
            var directoryHelper = new DirectoryHelper();

            IList<FileExplorerEntry> entries;
            if (directoriesOnly)
            {
                entries = (await directoryHelper.GetDirectoryEntries(path, CancellationToken.None))
                    .ToList<FileExplorerEntry>();
            }
            else
            {
                entries = (await directoryHelper.GetEntries(path, CancellationToken.None)).ToList();
            }

            return Ok(entries);
        }

        [MazeGet("path")]
        public IActionResult ExpandEnvironmentVariables([FromQuery] string path) =>
            Ok(Environment.ExpandEnvironmentVariables(path));

        [MazeGet("directory")]
        public IActionResult GetDirectory([FromQuery] string path)
        {
            using (var directory = new DirectoryInfoEx(path))
            {
                var directoryEntry = new DirectoryHelper().GetDirectoryEntry(directory, null);
                return Ok(directoryEntry);
            }
        }

        [MazePost("directory")]
        public IActionResult CreateDirectory([FromQuery] string path)
        {
            Directory.CreateDirectory(path);
            return StatusCode(StatusCodes.Status201Created);
        }

        [MazeDelete("directory")]
        public IActionResult DeleteDirectory([FromQuery] string path)
        {
            Directory.Delete(path, true);
            return Ok();
        }

        [MazeDelete("file")]
        public IActionResult DeleteFile([FromQuery] string path)
        {
            System.IO.File.Delete(path);
            return Ok();
        }

        [MazePatch("file")]
        public IActionResult MoveFile([FromQuery] string path, [FromQuery] string newPath)
        {
            System.IO.File.Move(path, newPath);
            return Ok();
        }

        [MazePatch("directory")]
        public IActionResult MoveDirectory([FromQuery] string path, [FromQuery] string newPath)
        {
            Directory.Move(path, newPath);
            return Ok();
        }

        [MazeGet("directory/properties")]
        public IActionResult GetDirectoryProperties([FromQuery] string path)
        {
            return Ok();
        }

        [MazeGet("file/properties")]
        public async Task<IActionResult> GetFileProperties([FromQuery] string path, [FromServices] IEnumerable<IFilePropertyValueProvider> propertyValueProviders)
        {
            var result = new FilePropertiesDto();
            var fileInfo = new FileInfo(path);
            var properties = new ConcurrentBag<FileProperty>();

            await TaskCombinators.ThrottledCatchErrorsAsync(propertyValueProviders, (provider, token) => Task.Run(() =>
            {
                foreach (var fileProperty in provider.ProvideValues(fileInfo, result).ToList())
                    properties.Add(fileProperty);
            }), CancellationToken.None);

            result.Properties = properties.ToList();
            return Ok(result);
        }

        [MazePost("file/execute")]
        public IActionResult ExecuteFile([FromBody] ExecuteFileDto executeDto, [FromQuery] bool waitForExit)
        {
            var processStartInfo = new ProcessStartInfo(executeDto.FileName, executeDto.Arguments)
            {
                WorkingDirectory = executeDto.WorkingDirectory ?? string.Empty,
                UseShellExecute = executeDto.UseShellExecute,
                Verb = executeDto.Verb,
                CreateNoWindow = executeDto.CreateNoWindow
            };

            var process = Process.Start(processStartInfo);
            if (executeDto.UseShellExecute)
                return Ok();

            if (process == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            if (waitForExit)
                process.WaitForExit();

            return Ok();
        }

        [MazeGet("file/verbs")]
        public IActionResult GetFileVerbs([FromQuery] string path)
        {
            var info = new ProcessStartInfo(path);
            return Ok(info.Verbs);
        }
    }
}