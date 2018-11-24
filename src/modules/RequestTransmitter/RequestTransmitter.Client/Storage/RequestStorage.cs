using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using RequestTransmitter.Client.Options;
using RequestTransmitter.Client.Utilities;

namespace RequestTransmitter.Client.Storage
{
    public class RequestStorage : IRequestStorage
    {
        private readonly IFileSystem _fileSystem;
        private readonly RequestTransmitterOptions _options;
        private long _currentNumber;
        private readonly AsyncReaderWriterLock _readerWriterLock;

        public RequestStorage(IFileSystem fileSystem, IOptions<RequestTransmitterOptions> options)
        {
            _fileSystem = fileSystem;
            _options = options.Value;
            _currentNumber = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _readerWriterLock = new AsyncReaderWriterLock();
        }

        public bool HasEntries
        {
            get
            {
                using (_readerWriterLock.ReaderLock())
                {
                    return _fileSystem.Directory.Exists(_options.RequestDirectory) &&
                           _fileSystem.Directory.EnumerateFiles(_options.RequestDirectory).Any();
                }
            }
        }

        public async Task<HttpRequestMessage> Peek()
        {
            using (await _readerWriterLock.ReaderLockAsync())
            {
                if (!_fileSystem.Directory.Exists(_options.RequestDirectory))
                    return null;

                var file = _fileSystem.Directory.EnumerateFiles(_options.RequestDirectory).OrderBy(x => x).FirstOrDefault();
                if (file == null)
                    return null;

                return await HttpRequestSerializer.Decode(_fileSystem.File.OpenRead(file));
            }
        }

        public async Task Pop()
        {
            using (await _readerWriterLock.WriterLockAsync())
            {
                if (!_fileSystem.Directory.Exists(_options.RequestDirectory))
                    return;

                var file = _fileSystem.Directory.EnumerateFiles(_options.RequestDirectory).OrderBy(x => x).FirstOrDefault();
                if (file == null)
                    return;

                _fileSystem.File.Delete(file);
            }
        }

        public async Task Push(HttpRequestMessage requestMessage)
        {
            var number = Interlocked.Increment(ref _currentNumber);
            Directory.CreateDirectory(_options.RequestDirectory);

            using (await _readerWriterLock.WriterLockAsync())
            using (var fileStream = _fileSystem.File.Create(_fileSystem.Path.Combine(_options.RequestDirectory, number.ToString())))
            {
                await HttpRequestSerializer.Format(requestMessage, fileStream);
            }
        }
    }
}