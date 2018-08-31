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
        private readonly FileSystemInfo _fileSystemInfo;
        private long _totalLength;
        private byte[] _buffer;
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(400);
        private DateTimeOffset _lastUpdate;
        private long _actualBytesTransferred;
        private long _lastUpdateActualBytesTransferred;
        private long _processedSize;

        public ZipContent(FileSystemInfo fileSystemInfo)
        {
            _fileSystemInfo = fileSystemInfo;

            Headers.ContentType = new MediaTypeHeaderValue("application/zip");
        }

        public event EventHandler<ZipContentProgressChangedEventArgs> ProgressChanged; 

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            _buffer = new byte[8192];
            _lastUpdate = DateTimeOffset.UtcNow;

            using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                if (_fileSystemInfo is FileInfo fileInfo)
                {
                    _totalLength = fileInfo.Length;

                    await CompressFile(fileInfo, zipArchive, fileInfo.Name);
                }
                else
                {
                    var directory = (DirectoryInfo)_fileSystemInfo;
                    var allFiles = directory.GetFiles("*", SearchOption.AllDirectories);

                    _totalLength = allFiles.Sum(info => info.Length);

                    int folderOffset = _fileSystemInfo.FullName.Length + (_fileSystemInfo.FullName.EndsWith("\\") ? 0 : 1);
                    await CompressFolder(directory, zipArchive, folderOffset);
                }
            }
        }

        private async Task CompressFolder(DirectoryInfo directory, ZipArchive zipStream, int folderOffset)
        {
            foreach (var fi in directory.EnumerateFiles())
            {
                var entryName = fi.FullName.Substring(folderOffset); // Makes the name in zip based on the folder
                await CompressFile(fi, zipStream, entryName);
            }

            foreach (var folder in directory.EnumerateDirectories())
            {
                await CompressFolder(folder, zipStream, folderOffset).ConfigureAwait(false);
            }
        }

        private async Task CompressFile(FileInfo fi, ZipArchive zipArchive, string entryName)
        {
            var entry = zipArchive.CreateEntry(entryName, CompressionLevel.Optimal);
            using (var reader = fi.OpenRead())
            using (var writer = entry.Open())
            {
                await StreamCopyToAsync(reader, writer, CancellationToken.None).ConfigureAwait(false);
            }
        }

        private async Task StreamCopyToAsync(Stream source, Stream destination, CancellationToken cancellationToken)
        {
            var buffer = _buffer;

            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                _actualBytesTransferred = destination.Position;
                _processedSize += bytesRead;

                UpdateProgressCallback();
            }
        }

        private void UpdateProgressCallback()
        {
            var diff = DateTimeOffset.UtcNow - _lastUpdate;
            if (diff > UpdateInterval)
            {
                if (ProgressChanged != null)
                {
                    var speed = (_actualBytesTransferred - _lastUpdateActualBytesTransferred) / diff.TotalSeconds;

                    var estimatedBytesToTransfer =
                        _processedSize / (double) _actualBytesTransferred * (_totalLength - _processedSize);
                    var estimatedTime = TimeSpan.FromSeconds(estimatedBytesToTransfer / speed);

                    var progress = _processedSize / (double) _totalLength;

                    ProgressChanged(this,
                        new ZipContentProgressChangedEventArgs(progress, _totalLength, _processedSize, speed,
                            estimatedTime));
                }

                _lastUpdate = DateTimeOffset.UtcNow;
                _lastUpdateActualBytesTransferred = _actualBytesTransferred;
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}