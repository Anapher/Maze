using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Administration.Models
{
    public class ZipContent : HttpContent
    {
        private static readonly TimeSpan FastUpdateInterval = TimeSpan.FromMilliseconds(50);
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(400);
        private readonly FileSystemInfo _fileSystemInfo;
        private EventHandler<TransferProgressChangedEventArgs> _progressChangedHandler;

        public ZipContent(FileSystemInfo fileSystemInfo)
        {
            _fileSystemInfo = fileSystemInfo;

            Headers.ContentEncoding.Add("zip");
            Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        }

        public CancellationToken CancellationToken { get; set; }
        public event EventHandler<TransferProgressChangedEventArgs> ProgressChanged
        {
            add => _progressChangedHandler += value;
            remove => _progressChangedHandler -= value;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var buffer = new byte[8192];
            var streamCopy = new StreamCopyUtil(FastUpdateInterval, UpdateInterval, buffer);
            streamCopy.ProgressChanged += _progressChangedHandler;

            using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                if (_fileSystemInfo is FileInfo fileInfo)
                {
                    streamCopy.TotalSize = fileInfo.Length;

                    await CompressFile(fileInfo, zipArchive, fileInfo.Name, streamCopy);
                }
                else
                {
                    var directory = (DirectoryInfo) _fileSystemInfo;
                    var allFiles = directory.GetFiles("*", SearchOption.AllDirectories);

                    streamCopy.TotalSize = allFiles.Sum(info => info.Length);

                    var folderOffset = _fileSystemInfo.FullName.Length +
                                       (_fileSystemInfo.FullName.EndsWith("\\") ? 0 : 1);

                    foreach (var fi in allFiles)
                    {
                        var entryName = fi.FullName.Substring(folderOffset); // Makes the name in zip based on the folder
                        await CompressFile(fi, zipArchive, entryName, streamCopy).ConfigureAwait(false);
                    }
                }
            }
        }

        private async Task CompressFile(FileInfo fi, ZipArchive zipArchive, string entryName, StreamCopyUtil streamCopy)
        {
            var entry = zipArchive.CreateEntry(entryName, CompressionLevel.Optimal);
            using (var reader = fi.OpenRead())
            using (var writer = entry.Open())
            {
                await streamCopy.CopyToAsync(reader, writer, CancellationToken).ConfigureAwait(false);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}